using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utilities;
using static Managers.GameManager;
using Event = Events.Event;
using Random = UnityEngine.Random;

namespace Managers
{
    public class Stats : MonoBehaviour
    {
        #region PrimaryStats
        [Serializable]
        public class Modifier
        {
            public int amount;
            public int turnsLeft;
            public string reason;
        }

        public Dictionary<Stat, List<Modifier>> Modifiers = new Dictionary<Stat, List<Modifier>>();
        public readonly Dictionary<Stat, int> ModifiersTotal = new Dictionary<Stat, int>();
        public Dictionary<Stat, List<int>> StatHistory = new Dictionary<Stat, List<int>>();
        public Dictionary<Guild, List<int>> AdventurerHistory = new Dictionary<Guild, List<int>>();

        private static readonly Dictionary<Stat, UpgradeType> UpgradeMap = new Dictionary<Stat, UpgradeType>
        {
            { Stat.Brawler, UpgradeType.Brawler },
            { Stat.Outrider, UpgradeType.Outrider },
            { Stat.Performer, UpgradeType.Performer },
            { Stat.Diviner, UpgradeType.Diviner },
            { Stat.Arcanist, UpgradeType.Arcanist },
            { Stat.Spending, UpgradeType.Spending },
            { Stat.Housing, UpgradeType.Housing},
            { Stat.Food, UpgradeType.Food }
        };
        
        private readonly List<Stat> _baseStats = new List<Stat>
        {
            Stat.Food,
            Stat.Housing,
            Stat.Spending
        };

        [SerializeField] private SerializedDictionary<Guild, Event> excessEvents;

        public int StatMultiplier(Stat stat) => _baseStats.Contains(stat) ? BaseStatMultiplier : 1;


        public int GetUpgradeMod(Stat stat)
        {
            return UpgradeMap.ContainsKey(stat) ? Manager.Upgrades.GetLevel(UpgradeMap[stat]) : 0;
        }
        
        public int GetStat(Stat stat)
        {
            int foodMod = (int) stat < 5 ? FoodModifier : 0;
            return StatMultiplier(stat) * (Manager.Structures.GetStat(stat) + GetUpgradeMod(stat)) + ModifiersTotal[stat] + foodMod;
        }

        public int GetSatisfaction(Guild guild)
        {
            return GetStat((Stat)guild) - Manager.Adventurers.GetCount(guild, true);
        }
        
        public int GetSatisfaction(Stat stat)
        {
            if ((int) stat < 5) // If the stat is for an adventuring category
                return GetSatisfaction((Guild)stat);
            return GetStat(stat) - Manager.Adventurers.Available;
        }

        public int MaxSpawnChance => 50 + 10 * Manager.Upgrades.GetLevel(UpgradeType.MaxAdventurerSpawn);
        
        public int SpawnChance(Guild guild) => Mathf.Clamp(GetSatisfaction(guild) * 10, -100, MaxSpawnChance);
        
        public int HousingSpawnChance => Mathf.Clamp(GetSatisfaction(Stat.Housing)/10 + 1, -1, 3);
        
        public int FoodModifier => GetSatisfaction(Stat.Food)/10;

        public int WealthPerTurn => (Manager.EventQueue.Flags[Flag.Cosmetics] ? 0 : Manager.Adventurers.Available) + GetStat(Stat.Spending) + StartingSalary;
    
        public int Wealth { get;  set; }

        public int Defence => Manager.Adventurers.Available + GetStat(Stat.Defence) + MineStrikePenalty;

        public int BaseThreat { get; set; }
        public int Threat => BaseThreat + ModifiersTotal[Stat.Threat] + Manager.Quests.RadiantQuestCellCount + ScarecrowThreat;

        public int Stability { get; set; } // Percentage of how far along the threat is.

        public int TurnCounter { get; set; }
        
        public bool Spend(int amount)
        {
            if (Manager.State.Loading) return true; //Don't spend money when loading
            if (Wealth < amount) return false;
            Wealth -= amount;
            return true;
        }

        public int ScarecrowThreat => Manager.EventQueue.Flags[Flag.Scarecrows] ? Manager.Structures.GetCount(BuildingType.Farm) * 3 : 0;
        public int MineStrikePenalty => Manager.EventQueue.Flags[Flag.MineStrike] ? -5 : 0;
        
        #endregion

        #region State Callbacks
        private void Start()
        {
            State.OnNewGame += OnNewGame;
            State.OnNextTurnBegin += OnNextTurnBegin;
            State.OnNextTurnEnd += OnNextTurnEnd;
            State.OnNewTurn += OnNewTurn;
            Structures.Structures.OnGuildHallDemolished += () => Stability = 0;
        }

        private void OnNewGame()
        {
            TurnCounter = 1;
            BaseThreat = 0;
            Stability = 50 + Manager.Upgrades.GetLevel(UpgradeType.Stability) * 10;
            Wealth = Tutorial.Tutorial.Active ? 0 : StartingWealth + Manager.Upgrades.GetLevel(UpgradeType.Wealth) * 10;
            foreach (Stat stat in Enum.GetValues(typeof(Stat)))
            {
                ModifiersTotal[stat] = 0;
                Modifiers[stat] = new List<Modifier>();
                StatHistory[stat] = new List<int>();
            }
            
            foreach (Guild guild in Enum.GetValues(typeof(Guild)))
            {
                AdventurerHistory[guild] = new List<int>();
            }
        }

        private void OnNextTurnBegin()
        {
            Stability += Defence - Threat;
            if (Stability > 100)
            {
                Manager.EventQueue.AddThreat();
                Stability = 100;
            }
            Wealth += WealthPerTurn;
            TurnCounter++;

            // Spawn adventurers based on satisfaction
            foreach (Guild guild in Enum.GetValues(typeof(Guild)))
            {
                int spawnRoll = Random.Range(0, 100);
                int spawnChance = SpawnChance(guild);
                if (spawnRoll < spawnChance) Manager.Adventurers.Add(guild);
                else if (spawnRoll < -spawnChance) Manager.Adventurers.Remove(false, guild);
                
                if (GetSatisfaction(guild) >= 20) Manager.EventQueue.Add(excessEvents[guild], true);
            }
            
            Debug.Log($"Stats: Starting turn {TurnCounter}");
            UpdateUi();
        }
        
        private void OnNextTurnEnd()
        {
            if (Stability <= 0) Manager.EventQueue.AddGameOverEvents();
        
            foreach ((Stat stat, List<Modifier> mods) in Modifiers)
            {
                for (int i = mods.Count-1; i >= 0; i--)
                {
                    Modifier mod = mods[i];
                    // -1 means infinite modifier
                    if (mod.turnsLeft == -1 || --mod.turnsLeft > 0) continue;
                    Debug.Log($"Stats: Removing modifier {mod.amount.WithSign()} {stat} {mod.reason}");
                    ModifiersTotal[stat] -= Modifiers[stat][i].amount;
                    Modifiers[stat].RemoveAt(i);
                }
            }
        }

        private void OnNewTurn()
        {
            foreach (Stat stat in Enum.GetValues(typeof(Stat)))
            {
                switch (stat)
                {
                    case Stat.Spending:
                        StatHistory[stat].Add(WealthPerTurn);
                        break;
                    case Stat.Defence:
                        StatHistory[stat].Add(Defence);
                        break;
                    case Stat.Threat:
                        StatHistory[stat].Add(Threat);
                        break;
                    case Stat.Stability:
                        StatHistory[stat].Add(Stability);
                        break;
                    default:
                        StatHistory[stat].Add(GetStat(stat));
                        break;
                }
            }
            
            foreach (Guild guild in Enum.GetValues(typeof(Guild)))
            {
                AdventurerHistory[guild].Add(Manager.Adventurers.GetCount(guild));
            }
        }
        
        #endregion

        #region Serialisation
        public StatDetails Save()
        {
            return new StatDetails
            {
                wealth = Wealth,
                turnCounter = TurnCounter,
                stability = Stability,
                baseThreat = BaseThreat,
                modifiers = Modifiers,
                statHistory = StatHistory,
                adventurerHistory = AdventurerHistory
            };
        }

        public void Load(StatDetails details)
        {
            TurnCounter = details.turnCounter;
            Wealth = details.wealth;
            Stability = details.stability;
            BaseThreat = details.baseThreat;

            Modifiers = details.modifiers ?? new Dictionary<Stat, List<Modifier>>();
            StatHistory = details.statHistory ?? new Dictionary<Stat, List<int>>();
            AdventurerHistory = details.adventurerHistory ?? new Dictionary<Guild, List<int>>();

            foreach (Stat stat in Enum.GetValues(typeof(Stat)))
            {
                ModifiersTotal.Add(stat, 0);
                if (!Modifiers.ContainsKey(stat)) Modifiers.Add(stat, new List<Modifier>());
                if (!StatHistory.ContainsKey(stat)) StatHistory.Add(stat, new List<int>());
            }

            foreach (Guild stat in Enum.GetValues(typeof(Guild)))
            {
                if (!AdventurerHistory.ContainsKey(stat)) AdventurerHistory.Add(stat, new List<int>());
            }

            foreach (var metricPair in Modifiers)
                ModifiersTotal[metricPair.Key] = metricPair.Value.Sum(x => x.amount);
        }
        #endregion
    }
}
