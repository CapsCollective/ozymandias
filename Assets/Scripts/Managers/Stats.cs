using System;
using System.Collections.Generic;
using System.Linq;
using Buildings;
using UnityEngine;
using Utilities;
using Random = UnityEngine.Random;
using static Managers.GameManager;

namespace Managers
{
    public class Stats : MonoBehaviour
    {
        [Serializable]
        public class Modifier
        {
            public int amount;
            public int turnsLeft;
            public string reason;
        }

        public Dictionary<Stat, List<Modifier>> Modifiers = new Dictionary<Stat, List<Modifier>>();
        public readonly Dictionary<Stat, int> ModifiersTotal = new Dictionary<Stat, int>();

        public int SkillPoints { get; set; }
        
        private void Start()
        {
            State.OnNewGame += OnNewGame;
            State.OnNextTurnEnd += OnNextTurnEnd;
        }

        public int GetStat(Stat stat)
        {
            int mod = stat == Stat.Food || stat == Stat.Housing ? 4 : 1;
            int foodMod = (int) stat < 5 ? FoodModifier : 0;
            return mod * Manager.Buildings.GetStat(stat) + ModifiersTotal[stat] + foodMod;
        }

        private int GetSatisfaction(AdventurerType type)
        {
            return GetStat((Stat)type) - Manager.Adventurers.GetCount(type);
        }
        
        public int GetSatisfaction(Stat stat)
        {
            if ((int) stat < 5) // If the stat is for an adventuring category
                return GetSatisfaction((AdventurerType)stat);
            return GetStat(stat) - Manager.Adventurers.Count;
        }

        public float SpawnChance(AdventurerType type)
        {
            return Mathf.Clamp((GetSatisfaction(type)+10) * 2.5f, 0, 50);
        }
        
        public int RandomSpawnChance => Mathf.Clamp(GetSatisfaction(Stat.Housing)/10 + 1, -1, 3);
        
        public int FoodModifier => Mathf.Clamp(GetSatisfaction(Stat.Food)/10, -2, 2);

        public int WealthPerTurn => (100 + GetStat(Stat.Spending)) * Manager.Adventurers.Available / 20; //5 gold per adventurer times spending
    
        public int Wealth { get;  set; }

        public int Defence => Manager.Adventurers.Available + GetStat(Stat.Defence);

        // TODO: How do we make a more engaging way to determine threat than just a turn counter?
        public int Threat => 9 + (3 * TurnCounter) + ModifiersTotal[Stat.Threat];
        
        public int Stability { get; set; } // Percentage of how far along the threat is.

        public int TurnCounter { get; set; }
        
        public bool Spend(int amount)
        {
            if (Manager.State.Loading) return true; //Don't spend money when loading
            if (Wealth < amount) return false;
            Wealth -= amount;
            return true;
        }

        #region State Callbacks
        
        public void OnNewGame()
        {
            // Start game with 10 Adventurers
            for (int i = 0; i < 10; i++) Manager.Adventurers.Add();

            TurnCounter = 1;
            Stability = 100;
            Wealth = 100;
        }

        private void OnNextTurnEnd()
        {
            Stability += Defence - Threat;
            if (Stability > 100) Stability = 100;
            Wealth += WealthPerTurn;
            TurnCounter++;

            // Spawn adventurers based on satisfaction
            foreach (AdventurerType category in Enum.GetValues(typeof(AdventurerType)))
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

        public StatDetails Save()
        {
            return new StatDetails
            {
                wealth = Wealth,
                turnCounter = TurnCounter,
                stability = Stability,
                terrainClearCount = Clear.TerrainClearCount,
                ruinsClearCount = Clear.RuinsClearCount,
                modifiers = Modifiers
            };
        }

        public void Load(StatDetails details)
        {
            TurnCounter = details.turnCounter;
            Clear.TerrainClearCount = details.terrainClearCount;
            Clear.RuinsClearCount = details.ruinsClearCount;
            Wealth = details.wealth;
            Stability = details.stability;
            
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
    }
}
