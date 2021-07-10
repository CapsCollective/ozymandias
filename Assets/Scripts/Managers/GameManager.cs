using System;
using System.Collections.Generic;
using Controllers;
using NaughtyAttributes;
using UI;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.UI;
using Utilities;
using Event = Entities.Event;
using Random = UnityEngine.Random;

namespace Managers
{
    public class GameManager : MonoBehaviour
    {
        public static Action OnNewTurn;
        public static Action OnNextTurn;
        public static Action OnUpdateUI;
    
        private static GameManager _instance;
        public static GameManager Manager
        {
            get
            {
                if (!_instance) _instance = FindObjectOfType<GameManager>();
                return _instance;
            }
        }

        public bool isLoading;

        // All Managers/ Universal Controllers
        public Adventurers Adventurers { get; private set; }
        public Buildings Buildings { get; private set; }
        public Achievements Achievements { get; private set; }
        public BuildingCards BuildingCards { get; private set; }
        public Quests Quests { get; private set; }
        public EventQueue EventQueue { get; private set; }
        public Settings Settings { get; private set; }
        public Map Map { get; private set; }
        public Newspaper Newspaper { get; private set; }
        public BuildingPlacement BuildingPlacement { get; private set; }
        public Tooltip Tooltip { get; private set; }

        private void Awake()
        {
            Random.InitState((int)DateTime.Now.Ticks);
            Adventurers = FindObjectOfType<Adventurers>();
            Buildings = FindObjectOfType<Buildings>();
            Achievements = FindObjectOfType<Achievements>();
            BuildingCards = FindObjectOfType<BuildingCards>();
            Quests = FindObjectOfType<Quests>();
            EventQueue = FindObjectOfType<EventQueue>();
            Settings = FindObjectOfType<Settings>();
            Map = FindObjectOfType<Map>();

            BuildingPlacement = FindObjectOfType<BuildingPlacement>();
            Newspaper = FindObjectOfType<Newspaper>();
            Tooltip = FindObjectOfType<Tooltip>();
            
            Load();
        }

        [Serializable]
        public class Modifier
        {
            public int amount;
            public int turnsLeft;
            public string reason;
        }

        public Dictionary<Stat, List<Modifier>> Modifiers = new Dictionary<Stat, List<Modifier>>();
        public readonly Dictionary<Stat, int> ModifiersTotal = new Dictionary<Stat, int>();
        
        public readonly Dictionary<AdventurerCategory, int> SpawnCounters = new Dictionary<AdventurerCategory, int>
        {
            { AdventurerCategory.Brawler, 0 }, { AdventurerCategory.Outrider, 0 }, { AdventurerCategory.Performer, 0 },
            { AdventurerCategory.Diviner, 0 }, { AdventurerCategory.Arcanist, 0 }
        };
        
        public int GetStat(Stat stat)
        {
            int mod = stat == Stat.Food || stat == Stat.Housing ? 4 : 1;
            int foodMod = (int) stat < 5 ? FoodModifier : 0;
            return mod * Buildings.GetStat(stat) + ModifiersTotal[stat] + foodMod;
        }

        private int GetSatisfaction(AdventurerCategory category)
        {
            return GetStat((Stat)category) - Adventurers.GetCount(category);
        }
        
        public int GetSatisfaction(Stat stat)
        {
            if ((int) stat < 5) // If the stat is for an adventuring category
                return GetSatisfaction((AdventurerCategory)stat);
            return GetStat(stat) - Adventurers.Count;
        }

        public int TurnsToSpawn(AdventurerCategory category)
        {
            return Mathf.Clamp((5 - GetSatisfaction(category))/2 + 2, 2, 8);
        }
        
        public int RandomSpawnChance => Mathf.Clamp(GetSatisfaction(Stat.Housing)/10 + 1, -1, 3);
        
        public int FoodModifier => Mathf.Clamp(GetSatisfaction(Stat.Food)/10, -2, 2);

        public int WealthPerTurn => (100 + GetStat(Stat.Spending)) * Adventurers.Available / 20; //5 gold per adventurer times spending
    
        public int Wealth { get;  set; }

        public int Defense => Adventurers.Available + GetStat(Stat.Defense);

        public int Threat => 9 + (3 * TurnCounter) + ModifiersTotal[Stat.Threat];

        public int ChangePerTurn => Defense - Threat; // How much the top bar shifts each turn
    
        public int Stability { get; set; } // Percentage of how far along the threat is.

        public int TurnCounter { get; set; }
        
        public bool Spend(int amount)
        {
            if (SaveFile.loading) return true; //Don't spend money when loading
            if (Wealth < amount) return false;
            Wealth -= amount;
            return true;
        }

        [Button("StartGame")]
        public void StartGame()
        {
            /*// Clear out all adventurers and buildings
            foreach (Transform child in adventurersContainer.transform) Destroy(child.gameObject);
            foreach (Transform child in buildingsContainer.transform) Destroy(child.gameObject);
            Adventurers.Clear();
            Buildings.Clear();*/

            // Start game with 5 Adventurers
            for (int i = 0; i < 10; i++) Manager.Adventurers.Add();

            Stability = 100;
            Wealth = 100;
        
            EventQueue.Add(openingEvent, true);
        
            // Run the tutorial video
            /*if (PlayerPrefs.GetInt("tutorial_video_basics", 0) == 0)
        {
            PlayerPrefs.SetInt("tutorial_video_basics", 1);
            TutorialPlayerController.Instance.PlayClip(0);
        }*/

            Analytics.EnableCustomEvent("New Turn", true);
            Analytics.enabled = true;
        }

        [Button("Next Turn")]
        public void NextTurn()
        {
            OnNextTurn?.Invoke();
        }

        [SerializeField] private GenericDictionary<int, Event> manualEvents;
        public void NewTurn()
        {
            Buildings.placedThisTurn = 0;
            Stability += ChangePerTurn;
            if (Stability > 100) Stability = 100;
            Wealth += WealthPerTurn;
            TurnCounter++;
            
            if(Buildings.GetCount(BuildingType.GuildHall) > 0 && manualEvents.ContainsKey(TurnCounter)) // Check guild hall still exists
                EventQueue.Add(manualEvents[TurnCounter], true);

            // Spawn adventurers based on satisfaction
            foreach (AdventurerCategory category in Enum.GetValues(typeof(AdventurerCategory)))
            {
                SpawnCounters[category]++;
                int turnsToSpawn = TurnsToSpawn(category);
                if (SpawnCounters[category] < turnsToSpawn) continue;
                Adventurers.Add(category);
                SpawnCounters[category] -= turnsToSpawn;
            }
            
            if (Stability <= 0)
                foreach (Event e in supportWithdrawnEvents) EventQueue.Add(e, true);
        
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

            EventQueue.Process();
        
            OnNewTurn?.Invoke();
            //if (turnCounter % 5 == 0)
            //{
            //    var ev = Analytics.CustomEvent("Turn Counter", new Dictionary<string, object>
            //    {
            //        { "turn_number", turnCounter }
            //    });
            //}

            Save();
            EnterMenu();
            UpdateUi();
        }

        public void UpdateUi()
        {
            /*if (AvailableAdventurers >= 20 && Effectiveness == 100) Achievements.Unlock("Top of Their Game");
            if (AvailableAdventurers >= 20 && Satisfaction == 100) Achievements.Unlock("A Jolly Good Show");*/
        
            OnUpdateUI?.Invoke();
        }

        public void Save()
        {
            if(_gameOver) return;
            new SaveFile().Save();
        }
        
        private async void Load()
        {
            isLoading = true;
            await new SaveFile().Load();
            UpdateUi();
            isLoading = false;
        }

        private bool _gameOver;
        public void GameOver()
        {
            _gameOver = true;
            Settings.ClearSave();
            Newspaper.GameOver();
        }

        public bool inMenu;
        public void EnterMenu()
        {
            inMenu = true;
            Shade.Instance.SetDisplay(true);
            BuildingPlacement.GetComponent<ToggleGroup>().SetAllTogglesOff();
        }
    
        public void ExitMenu()
        {
            inMenu = false;
            Shade.Instance.SetDisplay(false);
        }
    
        public Event openingEvent;
        public Event[] supportWithdrawnEvents;
    
        private void OnDestroy()
        {
            _instance = null;
            OnNewTurn = null;
            OnNextTurn = null;
            OnUpdateUI = null;
        }
    }
}
