using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Achievements;
using Newtonsoft.Json;
using UnityEngine;
using Utilities;
using static Managers.GameManager;
using EventType = Utilities.EventType;

namespace Managers
{
    [Serializable]
    public struct StatDetails
    {
        public int wealth, turnCounter, stability, baseThreat;
        public Dictionary<Stat, List<Stats.Modifier>> modifiers;
    }
    
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
        public bool storyActive;
        public List<string> headliners, others;
        public Dictionary<EventType, List<string>> used;
    }
    
    [Serializable]
    public struct AdventurerDetails
    {
        public string name;
        public Guild guild;
        public bool isSpecial;
        public int turnJoined;
    }

    [Serializable]
    public struct StructureDetails
    {
        public List<BuildingDetails> buildings;
        public List<TerrainDetails> terrain;
    }
    
    [Serializable]
    public struct BuildingDetails
    {
        public BuildingType type;
        public int rootId;
        public int rotation;
        public bool isRuin;
    }
    
    public struct TerrainDetails
    {
        public int rootId;
        public int sectionCount;
    }
    
    [Serializable]
    public struct CardDetails
    {
        public List<BuildingType> deck, unlocked, playable, discoverable;
    }
    
    [Serializable]
    public struct AchievementDetails
    {
        public List<Achievement> unlocked;
    }
    
    [Serializable]
    public struct RequestDetails
    {
        public string name;
        public int completed, required;
    }
    
    [Serializable]
    public struct UpgradeDetails
    {
        public Dictionary<Guild, int> guildTokens;
        public Dictionary<UpgradeType, int> upgradeLevels;
    }

    [Serializable]
    public class SaveFile
    {
        public StatDetails stats;
        public List<AdventurerDetails> adventurers; 
        public List<QuestDetails> quests;
        public EventQueueDetails eventQueue;
        public CardDetails cards;
        public StructureDetails structures;
        public AchievementDetails achievements;
        public Dictionary<Guild, RequestDetails> requests;
        public UpgradeDetails upgrades;
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
            structures = Manager.Structures.Save();
            cards = Manager.Cards.Save();
            
            if (Manager.State.IsGameOver)
            {
                stats = new StatDetails
                {
                    turnCounter = 0 // Reset game
                };
            }
            else
            {
                stats = Manager.Stats.Save();
                adventurers = Manager.Adventurers.Save();
                quests = Manager.Quests.Save();
                eventQueue = Manager.EventQueue.Save();
            }

            //achievements = Manager.Achievements.Save();
            requests = Manager.Requests.Save();
            upgrades = Manager.Upgrades.Save();
            
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
                Debug.LogError(e);
                //TODO: Need a proper migration procedure for if we every change the save files
                //Try again with a clear save
                JsonConvert.PopulateObject(File.ReadAllText(Application.streamingAssetsPath + "/StartingLayout.json"), this);
            }

            Manager.Stats.Load(stats);
            Manager.Structures.Load(structures);
            if(Manager.Structures.Count == 0) Manager.Map.FillGrid();
            Manager.Cards.Load(cards);
            await Manager.EventQueue.Load(eventQueue);
            await Manager.Requests.Load(requests);
            Manager.Upgrades.Load(upgrades);

            if (Manager.Stats.TurnCounter != 0) // Only for continuing a game
            {
                Manager.Adventurers.Load(adventurers);
                await Manager.Quests.Load(quests);
            }

            UpdateUi();
        }
    }
    
}
