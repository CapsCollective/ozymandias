using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Controllers;
using Newtonsoft.Json;
using UnityEngine;
using Utilities;
using static Managers.GameManager;
using EventType = Utilities.EventType;

// ReSharper disable InconsistentNaming

namespace Managers
{
    [Serializable]
    public struct QuestDetails
    {
        public string name;
        public int turnsLeft;
        public int cost;
        public List<string> assigned;
    }
    
    [Serializable]
    public struct EventQueueDetails
    {
        public int nextBuildingUnlock;
        public List<string> headliners, others;
        public Dictionary<EventType, List<string>> used, discarded;
    }
    
    [Serializable]
    public struct AdventurerDetails
    {
        public string name;
        public AdventurerCategory category;
        public bool isSpecial;
        public int turnJoined;
    }
    
    [Serializable]
    public struct BuildingDetails
    {
        public string name;
        public float x, y;
        public int rotation;
    }
    
    [Serializable]
    public class SaveFile
    {
        public static bool loading;
        
        public int wealth, turnCounter, threatLevel, clearCount;
        public List<string> buildings, unlockedBuildings;
        public List<AdventurerDetails> adventurers; 
        public Dictionary<Stat, List<Modifier>> modifiers;
        public List<QuestDetails> quests;
        public EventQueueDetails eventQueue;

        public void Save()
        {
            wealth = Manager.Wealth;
            turnCounter = Manager.TurnCounter;
            threatLevel = Manager.Stability;
            clearCount = Clear.TerrainClearCount;

            buildings = Manager.Buildings.Save();

            adventurers = Manager.Adventurers.Save();

            quests = Manager.Quests.Save();

            eventQueue = Manager.EventQueue.Save();
            
            unlockedBuildings = Manager.BuildingCards.Save();
            
            modifiers = Manager.Modifiers;

            PlayerPrefs.SetString("Save", JsonConvert.SerializeObject(this));
        }

        public async Task Load()
        {
            loading = true;
            string saveJson = PlayerPrefs.GetString("Save", File.ReadAllText(Application.streamingAssetsPath + "/StartingLayout.json"));
            JsonConvert.PopulateObject(saveJson, this);

            Manager.Wealth = wealth;
            Manager.TurnCounter = turnCounter;
            Manager.Stability = threatLevel;
            Clear.TerrainClearCount = clearCount;
            
            if (turnCounter == 0)
                Manager.StartGame();
            
            Manager.Adventurers.Load(adventurers);
            
            Manager.Modifiers = modifiers;

            foreach (Stat metric in Enum.GetValues(typeof(Stat)))
            {
                Manager.ModifiersTotal.Add(metric, 0);
                if (Manager.Modifiers.ContainsKey(metric)) continue;
                modifiers.Add(metric, new List<Modifier>());
            }
            
            foreach (var metricPair in Manager.Modifiers)
                Manager.ModifiersTotal[metricPair.Key] = metricPair.Value.Sum(x => x.amount);

            await Manager.Buildings.Load(buildings);

            await Manager.Quests.Load(quests);
            
            await Manager.BuildingCards.Load(unlockedBuildings);

            await Manager.EventQueue.Load(eventQueue);
            
            //TODO: Reshuffle buildings
            
            loading = false;
        }
    }
    
}
