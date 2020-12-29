using System;
using System.Collections.Generic;
using System.Linq;
using Controllers;
using Entities;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.UI;
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
    
        // All Managers/ Universal Controllers
        public Adventurers Adventurers { get; private set; }
        public Achievements Achievements { get; private set; }
        public BuildingCards BuildingCards { get; private set; }
        public Quests Quests { get; private set; }
        public EventQueue EventQueue { get; private set; }
        public Settings Settings { get; private set; }
        public Map Map { get; private set; }
        public Newspaper Newspaper { get; private set; }
        public BuildingPlacement BuildingPlacement { get; private set; }
    
        private void Awake()
        {
            Random.InitState((int)DateTime.Now.Ticks);
            Adventurers = FindObjectOfType<Adventurers>();
            Achievements = FindObjectOfType<Achievements>();
            BuildingCards = FindObjectOfType<BuildingCards>();
            Quests = FindObjectOfType<Quests>();
            EventQueue = FindObjectOfType<EventQueue>();
            Settings = FindObjectOfType<Settings>();
            Map = FindObjectOfType<Map>();

            BuildingPlacement = FindObjectOfType<BuildingPlacement>();
            Newspaper = FindObjectOfType<Newspaper>();

            Load();
        }

        [Serializable]
        public class Modifier
        {
            public int amount;
            public int turnsLeft;
            public string reason;
        }

        public Dictionary<Metric, List<Modifier>> Modifiers = new Dictionary<Metric, List<Modifier>>();
        public readonly Dictionary<Metric, int> ModifiersTotal = new Dictionary<Metric, int>();
    
        public readonly List<BuildingStats> Buildings = new List<BuildingStats>();
        public readonly List<BuildingStats> Terrain = new List<BuildingStats>();
    
       
    
        public int Accommodation => Buildings.Where(x => x.operational).Sum(x => x.accommodation);
    
        public int Weaponry => Mathf.Clamp(100 * Buildings.Where(x => x.operational).Sum(x => x.weaponry) / Adventurers.Available, 0, 100);
    
        public int Magic => Mathf.Clamp(100 * Buildings.Where(x => x.operational).Sum(x => x.magic) / Adventurers.Available, 0, 100);
    
        public int Equipment =>  Mathf.Clamp(100 * Buildings.Where(x => x.operational).Sum(x => x.equipment) / Adventurers.Available,0, 100);

        //public int Training => training = 0; // TODO: work out the specifics of this

        public int Effectiveness => Mathf.Clamp(1 + Equipment/3 + Weaponry/3 + Magic/3  + LowThreatMod + ModifiersTotal[Metric.Effectiveness], 0, 100);
    
        public int Food => Mathf.Clamp(100 * Buildings.Where(x => x.operational).Sum(x => x.food) / Adventurers.Available, 0, 100);

        public int Entertainment => Mathf.Clamp(100 * Buildings.Where(x => x.operational).Sum(x => x.entertainment) / Adventurers.Available, 0, 100);
    
        public int Luxury => Mathf.Clamp(100 * Buildings.Where(x => x.operational).Sum(x => x.luxury) / Adventurers.Available, 0, 100);

        public int OvercrowdingMod => Mathf.Min(0, (Accommodation - Adventurers.Available) * 5); //lose 5% satisfaction per adventurer over capacity
    
        public int LowThreatMod => Mathf.Min(0, (ThreatLevel - 20) * 2); //lose up to 40% effectiveness from low threat
    
        public int Satisfaction => Mathf.Clamp(1 + Food/3 + Entertainment/3 + Luxury/3 + OvercrowdingMod + ModifiersTotal[Metric.Satisfaction], 0, 100);
    
        public int Spending => 100 + Buildings.Where(x => x.operational).Sum(x => x.spending) + ModifiersTotal[Metric.Spending];
    
        public int WealthPerTurn => Spending * Adventurers.Available / 10; //10 gold per adventurer times spending
    
        public int Wealth { get;  set; }

        public int Defense => 
            Adventurers.Available * Effectiveness / 30 +
            Buildings.Where(x => x.operational).Sum(x => x.defense) +
            ModifiersTotal[Metric.Defense];
    
        public int Threat => 12 + (3 * TurnCounter) + ModifiersTotal[Metric.Threat];

        public int ChangePerTurn => Threat - Defense; // How much the top bar shifts each turn
    
        public int ThreatLevel { get; set; } // Percentage of how far along the threat is.

        public int TurnCounter { get; set; }
        
        public bool Spend(int amount)
        {
            if (_loading) return true; //Don't spend money when loading
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
            for (int i = 0; i < 5; i++) Manager.Adventurers.Add();

            ThreatLevel = 40;
            Wealth = 50;
        
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

        public void NewTurn()
        {
            _placedThisTurn = 0;
            ThreatLevel += ChangePerTurn;
            if (ThreatLevel < 0) ThreatLevel = 0;
            Wealth += WealthPerTurn;
            TurnCounter++;

            if (ThreatLevel >= 100)
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

        private int _placedThisTurn;
        public void Build(BuildingStats building)
        {
            if (building.terrain) Terrain.Add(building);
            else Buildings.Add(building);

            if(!_loading && ++_placedThisTurn >= 5) Achievements.Unlock("I'm Saving Up!");
            if (Buildings.Count >= 30 && Clear.ClearCount == 0) Achievements.Unlock("One With Nature");
        
            //var analyticEvent = Analytics.CustomEvent("Building Built", new Dictionary<string, object>
            //{
            //    {"building_type", building.name},
            //});
            if(!_loading) UpdateUi();
        }

        public void Demolish(BuildingStats building)
        {
            if (building.type == BuildingType.GuildHall)
            {
                //TODO: Add an 'are you sure?' dialogue
                Achievements.Unlock("Now Why Would You Do That?");
                foreach (Event e in guildHallDestroyedEvents) EventQueue.Add(e, true);
                NextTurn();
            }
        
            Map.Clear(building.GetComponent<BuildingStructure>());
            if (building.terrain) Terrain.Remove(building);
            else Buildings.Remove(building);
            //Destroy(building.gameObject);
            UpdateUi();
        }
    
        public void UpdateUi()
        {
            /*if (AvailableAdventurers >= 20 && Effectiveness == 100) Achievements.Unlock("Top of Their Game");
            if (AvailableAdventurers >= 20 && Satisfaction == 100) Achievements.Unlock("A Jolly Good Show");*/
        
            OnUpdateUI?.Invoke();
        }

        public int GetMetric(Metric metric)
        {
            switch (metric)
            {
                case Metric.Accommodation: return Accommodation;
                case Metric.Satisfaction: return Satisfaction;
                case Metric.Effectiveness: return Effectiveness;
                case Metric.Spending: return Spending;
                case Metric.Defense: return Defense;
                default: return 0;
            }
        }

        public int BuildingCount(BuildingType type)
        {
            return Buildings.Count(x => x.type == type);
        }
        
        public void Save()
        {
            if(_gameOver) return;
            new SaveFile().Save();
        }

        private bool _loading;
        private async void Load()
        {
            _loading = true;
            await new SaveFile().Load();
            _loading = false;
            UpdateUi();
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
    
        public GameObject buildingsContainer;
        public Event openingEvent;
        public Event[] guildHallDestroyedEvents;
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
