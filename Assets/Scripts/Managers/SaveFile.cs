using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Reports;
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
        public Dictionary<Stat, List<int>> statHistory;
        public Dictionary<Guild, List<int>> adventurerHistory;
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
        public Dictionary<Flag, bool> flags;
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
    
    [Serializable]
    public struct TerrainDetails
    {
        public int rootId;
        public int sectionCount;
    }
    
    [Serializable]
    public struct CardDetails
    {
        public List<BuildingType> deck, unlocked, playable;
        public int discoveriesRemaining;
    }
    
    [Serializable]
    public struct AchievementDetails
    {
        public HashSet<Achievement> unlocked;
        public Dictionary<Milestone, int> milestones;
    }
    
    [Serializable]
    public struct RequestDetails
    {
        public string name;
        public int completed, required, tokens;
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
        public class SaveData
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
            public string version;
        }

        protected static string SaveFilePath() => Manager.PlatformManager.FileSystem.GetSaveFilePath();
        protected static string BackupFilePath() => Manager.PlatformManager.FileSystem.GetBackupFilePath();

        public static void SaveState(bool overwriteBackup = true)
        {
            if (Tutorial.Tutorial.Active) return; // No saving during tutorial
            if (overwriteBackup) Manager.Notifications.Display("Game Saved", Manager.saveIcon);
            Manager.PlatformManager.FileSystem.SaveFile.Save(overwriteBackup);
        }
        
        public static IEnumerator LoadState()
        {
            return Manager.PlatformManager.FileSystem.SaveFile.Load();
        }

        public static void DeleteState()
        {
            Manager.PlatformManager.FileSystem.SaveFile.Delete();
        }

        public virtual void Delete()
        {
            File.Delete(SaveFilePath());
        }

        public virtual void Save(bool overwriteBackup)
        {
            if (File.Exists(SaveFilePath()) && overwriteBackup) File.WriteAllLines(BackupFilePath(), File.ReadAllLines(SaveFilePath()));
            var data = new SaveData();

            data.version = Application.version;
            data.structures = Manager.Structures.Save();
            data.cards = Manager.Cards.Save();
            data.achievements = Manager.Achievements.Save();
            
            if (Manager.State.IsGameOver)
            {
                data.stats = new StatDetails
                {
                    turnCounter = 0 // Reset game
                };
            }
            else
            {
                data.stats = Manager.Stats.Save();
                data.adventurers = Manager.Adventurers.Save();
                data.quests = Manager.Quests.Save();
                data.eventQueue = Manager.EventQueue.Save();
            }

            data.achievements = Manager.Achievements.Save();
            data.requests = Manager.Requests.Save();
            data.upgrades = Manager.Upgrades.Save();
            File.WriteAllText(SaveFilePath(), JsonConvert.SerializeObject(data));
        }

        public virtual IEnumerator Load()
        {
            SaveData data = new SaveData();
            if (File.Exists(SaveFilePath()))
            {
                string saveJson = File.ReadAllText(SaveFilePath());
                JsonConvert.PopulateObject(saveJson, data);
                
                if (data.version != Application.version)
                {
                    Debug.LogWarning("Save file from previous version " + data.version);
                }
            }
            else
            {
                Debug.LogWarning("Save.json not found, starting tutorial");
                
#if UNITY_EDITOR
                if (!Manager.skipTutorial)
#endif
                {
                    Tutorial.Tutorial.Active = true;
                    Tutorial.Tutorial.DisableSelect = true;
                    Tutorial.Tutorial.DisableNextTurn = true;
                    Manager.Structures.SpawnTutorialRuins();
                }
            }
            
            Manager.Achievements.Load(data.achievements);
            Manager.Upgrades.Load(data.upgrades);
            Manager.Cards.Load(data.cards);
            Manager.Stats.Load(data.stats);
            yield return Manager.Structures.Load(data.structures);
            if (Manager.Structures.Count == 0) yield return Manager.Map.FillGrid();
            Manager.EventQueue.Load(data.eventQueue);
            Manager.Requests.Load(data.requests);
            
            // If continuing a game
            if (Manager.Stats.TurnCounter != 0)
            {
                Manager.Adventurers.Load(data.adventurers);
                Manager.Quests.Load(data.quests);
            }
        }
    }
}
