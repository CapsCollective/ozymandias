using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Controllers;
using JetBrains.Annotations;
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
        public List<int> occupied;
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
        public AdventurerType type;
        public bool isSpecial;
        public int turnJoined;
    }
    
    [Serializable]
    public struct BuildingDetails
    {
        public string name;
        public int rootId;
        public int rotation;
        public int sectionCount;
        public bool isRuin;
    }
    
    [Serializable]
    public struct BuildingCardDetails
    {
        public List<string> all, current, discoverable;
    }
    
    [Serializable]
    public class SaveFile
    {
        public int wealth, turnCounter, stability, terrainClearCount, ruinsClearCount;
        
        public List<AdventurerDetails> adventurers; 
        public Dictionary<Stat, List<Modifier>> modifiers;
        public List<QuestDetails> quests;
        public EventQueueDetails eventQueue;
        public BuildingCardDetails buildingCards;
        public List<BuildingDetails> buildings;

        public static void SaveState()
        {
            new SaveFile().Save();
        }
        
        public static async Task LoadState()
        {
            await new SaveFile().Load();
        }

        public void Save()
        {
            buildings = Manager.Buildings.Save();
            buildingCards = Manager.BuildingCards.Save();
            
            if (Manager.IsGameOver)
            {
                turnCounter = 0; // Reset game
            }
            else
            {
                wealth = Manager.Wealth;
                turnCounter = Manager.TurnCounter;
                stability = Manager.Stability;
                terrainClearCount = Clear.TerrainClearCount;
                ruinsClearCount = Clear.RuinsClearCount;
                modifiers = Manager.Modifiers;
                adventurers = Manager.Adventurers.Save();
                quests = Manager.Quests.Save();
                eventQueue = Manager.EventQueue.Save();
            }
            PlayerPrefs.SetString("Save", JsonConvert.SerializeObject(this));
        }

        public async Task Load()
        {
            string saveJson = PlayerPrefs.GetString("Save", File.ReadAllText(Application.streamingAssetsPath + "/StartingLayout.json"));
            try
            {
                JsonConvert.PopulateObject(saveJson, this);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //TODO: Need a proper migration procedure for if we every change the save files
                //Try again with a clear save
                JsonConvert.PopulateObject(File.ReadAllText(Application.streamingAssetsPath + "/StartingLayout.json"), this);
            }

            Manager.TurnCounter = turnCounter;
            Clear.TerrainClearCount = terrainClearCount;
            Clear.RuinsClearCount = ruinsClearCount;

            await Manager.Buildings.Load(buildings);
            if(Manager.Buildings.Count == 0) Manager.Map.FillGrid();
            await Manager.BuildingCards.Load(buildingCards);
            await Manager.EventQueue.Load(eventQueue);

            Manager.Modifiers = modifiers ?? new Dictionary<Stat, List<Modifier>>();
            
            foreach (Stat stat in Enum.GetValues(typeof(Stat)))
            {
                Manager.ModifiersTotal.Add(stat, 0);
                if (Manager.Modifiers.ContainsKey(stat)) continue;
                Manager.Modifiers.Add(stat, new List<Modifier>());
            }
            
            foreach (var metricPair in Manager.Modifiers)
                Manager.ModifiersTotal[metricPair.Key] = metricPair.Value.Sum(x => x.amount);
            
            if (turnCounter != 0) // Only for continuing a game
            {
                Manager.Wealth = wealth;
                Manager.Stability = stability;
                Manager.Adventurers.Load(adventurers);
                await Manager.Quests.Load(quests);
                //TODO: Reshuffle buildings
            }
        }
    }
    
}
