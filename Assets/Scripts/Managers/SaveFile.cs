using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Controllers;
using Entities;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.AddressableAssets;
using static Managers.GameManager;
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
    public class AdventurerDetails
    {
        public string name;
        public AdventurerCategory category;
        public bool isSpecial;
        public int turnJoined;
    }
    
    [Serializable]
    public class SaveFile
    {
        public int wealth, turnCounter, threatLevel, clearCount;
        public List<string> buildings, unlockedBuildings;
        public List<AdventurerDetails> adventurers; 
        public Dictionary<Metric, List<Modifier>> modifiers;
        public List<QuestDetails> quests;
        public EventQueueDetails eventQueue;

        public void Save()
        {
            wealth = Manager.Wealth;
            turnCounter = Manager.TurnCounter;
            threatLevel = Manager.ThreatLevel;
            clearCount = Clear.ClearCount;

            buildings = new List<string>();
            
            foreach (BuildingStats building in Manager.Buildings)
                buildings.Add(building.Serialize());
            
            foreach (BuildingStats terrain in Manager.Terrain)
                buildings.Add(terrain.Serialize());

            adventurers = Manager.Adventurers.Save();

            quests = Manager.Quests.Save();

            eventQueue = Manager.EventQueue.Save();
            
            unlockedBuildings = Manager.BuildingCards.Save();
            
            modifiers = Manager.Modifiers;

            PlayerPrefs.SetString("Save", JsonConvert.SerializeObject(this));
        }

        public async Task Load()
        {
            string saveJson = PlayerPrefs.GetString("Save", File.ReadAllText(Application.streamingAssetsPath + "/StartingLayout.json"));
            JsonConvert.PopulateObject(saveJson, this);
            
            Manager.Wealth = wealth;
            Manager.TurnCounter = turnCounter;
            Manager.ThreatLevel = threatLevel;
            Clear.ClearCount = clearCount;
            
            if (turnCounter == 0)
                Manager.StartGame();
            
            Manager.Adventurers.Load(adventurers);
            
            foreach (string building in buildings)
            {
                string[] details = building.Split(',');
                Vector3 worldPosition = new Vector3(float.Parse(details[1]), 0, float.Parse(details[2]));
                GameObject buildingInstance = await Addressables.InstantiateAsync(details[0], GameObject.Find("Buildings").transform).Task;
                Manager.Map.CreateBuilding(buildingInstance, worldPosition, int.Parse(details[3]));
            }
            
            Manager.Modifiers = modifiers;
            
            Metric[] modsMetrics = {Metric.Defense, Metric.Threat, Metric.Spending, Metric.Effectiveness, Metric.Satisfaction};

            foreach (Metric metric in modsMetrics)
            {
                Manager.ModifiersTotal.Add(metric, 0);
                if (Manager.Modifiers.ContainsKey(metric)) continue;
                modifiers.Add(metric, new List<Modifier>());
            }
            
            foreach (var metricPair in Manager.Modifiers)
                Manager.ModifiersTotal[metricPair.Key] = metricPair.Value.Sum(x => x.amount);

            await Manager.Quests.Load(quests);
            
            await Manager.BuildingCards.Load(unlockedBuildings);

            await Manager.EventQueue.Load(eventQueue);
            
            //TODO: Reshuffle buildings
        }
    }
    
}
