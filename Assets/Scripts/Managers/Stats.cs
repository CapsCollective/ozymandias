using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utilities;
using Random = UnityEngine.Random;
using static Managers.GameManager;

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
            return GetStat((Stat)guild) - Manager.Adventurers.GetCount(guild);
        }
        
        public int GetSatisfaction(Stat stat)
        {
            if ((int) stat < 5) // If the stat is for an adventuring category
                return GetSatisfaction((Guild)stat);
            return GetStat(stat) - Manager.Adventurers.Available;
        }

        public int SpawnChance(Guild guild)
        {
            return Mathf.Clamp(
                (GetSatisfaction(guild) + 5) * 5, 
                0, 50 + 10 * Manager.Upgrades.GetLevel(UpgradeType.MaxAdventurerSpawn)
            );
        }
        
        public int RandomSpawnChance => Mathf.Clamp(GetSatisfaction(Stat.Housing)/10 + 1, -1, 3);
        
        public int FoodModifier => Mathf.Clamp(GetSatisfaction(Stat.Food)/10, -2, 2);

        public int WealthPerTurn => WealthPerAdventurer * Manager.Adventurers.Available + GetStat(Stat.Spending) + StartingSalary; // 5 gold per adventurer plus spending
    
        public int Wealth { get;  set; }

        public int Defence => Manager.Adventurers.Available + GetStat(Stat.Defence);

        public int BaseThreat { get; set; }
        public int Threat => BaseThreat + ModifiersTotal[Stat.Threat] + Manager.Quests.RadiantQuestCellCount + ScarecrowThreat;

        public int Stability { get; private set; } // Percentage of how far along the threat is.

        public int TurnCounter { get; set; }
        
        public bool Spend(int amount)
        {
            if (Manager.State.Loading) return true; //Don't spend money when loading
            if (Wealth < amount) return false;
            Wealth -= amount;
            return true;
        }

        public int ScarecrowThreat => Manager.EventQueue.Flags[Flag.Scarecrows] ? Manager.Structures.GetCount(BuildingType.Farm) * 3 : 0;
        
        #endregion

        #region State Callbacks
        private void Start()
        {
            State.OnNewGame += OnNewGame;
            State.OnNextTurnEnd += OnNextTurnEnd;
        }

        private void OnNewGame()
        {
            TurnCounter = 1;
            BaseThreat = 0;
            Stability = 50 + Manager.Upgrades.GetLevel(UpgradeType.Stability) * 10;
            Wealth = 100 + Manager.Upgrades.GetLevel(UpgradeType.Wealth) * 50;
            foreach (Stat stat in Enum.GetValues(typeof(Stat)))
            {
                ModifiersTotal[stat] = 0;
                Modifiers[stat] = new List<Modifier>();
            }
        }

        private void OnNextTurnEnd()
        {
            Stability += Defence - Threat;
            if (Stability > 100) Stability = 100;
            Wealth += WealthPerTurn;
            TurnCounter++;

            // Spawn adventurers based on satisfaction
            foreach (Guild category in Enum.GetValues(typeof(Guild)))
            {
                if (Random.Range(0, 100) < SpawnChance(category)) Manager.Adventurers.Add(category);
            }

            if (Stability <= 0) Manager.EventQueue.AddGameOverEvents();
        
            foreach (var stat in Modifiers)
            {
                for (int i = stat.Value.Count-1; i >= 0; i--)
                {
                    // -1 means infinite modifier
                    if (Modifiers[stat.Key][i].turnsLeft == -1 || --Modifiers[stat.Key][i].turnsLeft > 0) continue;
                    Debug.Log("Removing Stat: " + stat.Key);
                    ModifiersTotal[stat.Key] -= Modifiers[stat.Key][i].amount;
                    Modifiers[stat.Key].RemoveAt(i);
                }
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
                modifiers = Modifiers
            };
        }

        public void Load(StatDetails details)
        {
            TurnCounter = details.turnCounter;
            Wealth = details.wealth;
            Stability = details.stability;
            BaseThreat = details.baseThreat;

            Modifiers = details.modifiers ?? new Dictionary<Stat, List<Modifier>>();
            
            foreach (Stat stat in Enum.GetValues(typeof(Stat)))
            {
                ModifiersTotal.Add(stat, 0);
                if (Modifiers.ContainsKey(stat)) continue;
                Modifiers.Add(stat, new List<Modifier>());
            }
            
            foreach (var metricPair in Modifiers)
                ModifiersTotal[metricPair.Key] = metricPair.Value.Sum(x => x.amount);
        }
        #endregion
    }
}
