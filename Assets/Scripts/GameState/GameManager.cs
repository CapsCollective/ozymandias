using System;
using System.Collections.Generic;
using Buildings;
using Events;
using NaughtyAttributes;
using Tooltip;
using UnityEngine;
using Utilities;
using Random = UnityEngine.Random;

namespace GameState
{
    public class GameManager : MonoBehaviour
    {
        public static Action OnNextTurnStart;
        public static Action OnNextTurnEnd;
        public static Action OnUpdateUI;
        public static Action OnGameStart;
        public static Action OnGameEnd;
        public static Action OnEnterMenu;
        public static Action OnExitMenu;
        
        public static GameManager Manager { get; private set; }
        public bool InMenu { get; private set; }
        public bool TurnTransitioning { get; private set; }
        public bool IsLoading { get; private set; }
        public bool IsGameOver { get; set; }
        
        // All Managers/ Universal Controllers
        public Adventurers.Adventurers Adventurers { get; private set; }
        public Buildings.Buildings Buildings { get; private set; }
        public Achievements.Achievements Achievements { get; private set; }
        public Cards.Cards Cards { get; private set; }
        public Quests.Quests Quests { get; private set; }
        public EventQueue EventQueue { get; private set; }
        public Map.Map Map { get; private set; }
        public Jukebox Jukebox { get; private set; }
        public Inputs.Inputs Inputs { get; private set; }
        public Newspaper Newspaper { get; private set; }
        public TooltipDisplay Tooltip { get; private set; }

        private void Awake()
        {
            Manager = this;
            Random.InitState((int)DateTime.Now.Ticks);
            Adventurers = FindObjectOfType<Adventurers.Adventurers>();
            Buildings = FindObjectOfType<Buildings.Buildings>();
            Achievements = FindObjectOfType<Achievements.Achievements>();
            Cards = FindObjectOfType<Cards.Cards>();
            Quests = FindObjectOfType<Quests.Quests>();
            EventQueue = FindObjectOfType<EventQueue>();
            Map = FindObjectOfType<Map.Map>();
            Jukebox = FindObjectOfType<Jukebox>();
            Inputs = new Inputs.Inputs();
            Newspaper = FindObjectOfType<Newspaper>();
            Tooltip = FindObjectOfType<TooltipDisplay>();

            Inputs.IA_NextTurn.performed += (e) => NextTurnStart();

            Newspaper.OnClosed += CheckGameEnd;

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

        public int GetStat(Stat stat)
        {
            int mod = stat == Stat.Food || stat == Stat.Housing ? 4 : 1;
            int foodMod = (int) stat < 5 ? FoodModifier : 0;
            return mod * Buildings.GetStat(stat) + ModifiersTotal[stat] + foodMod;
        }

        private int GetSatisfaction(AdventurerType type)
        {
            return GetStat((Stat)type) - Adventurers.GetCount(type);
        }
        
        public int GetSatisfaction(Stat stat)
        {
            if ((int) stat < 5) // If the stat is for an adventuring category
                return GetSatisfaction((AdventurerType)stat);
            return GetStat(stat) - Adventurers.Count;
        }

        public float SpawnChance(AdventurerType type)
        {
            return Mathf.Clamp((GetSatisfaction(type)+10) * 2.5f, 0, 50);
        }
        
        public int RandomSpawnChance => Mathf.Clamp(GetSatisfaction(Stat.Housing)/10 + 1, -1, 3);
        
        public int FoodModifier => Mathf.Clamp(GetSatisfaction(Stat.Food)/10, -2, 2);

        public int WealthPerTurn => (100 + GetStat(Stat.Spending)) * Adventurers.Available / 20; //5 gold per adventurer times spending
    
        public int Wealth { get;  set; }

        public int Defence => Adventurers.Available + GetStat(Stat.Defence);

        // TODO: How do we make a more engaging way to determine threat than just a turn counter?
        public int Threat => 9 + (3 * TurnCounter) + ModifiersTotal[Stat.Threat];
        
        public int Stability { get; set; } // Percentage of how far along the threat is.

        public int TurnCounter { get; set; }
        
        public bool Spend(int amount)
        {
            if (IsLoading) return true; //Don't spend money when loading
            if (Wealth < amount) return false;
            Wealth -= amount;
            return true;
        }

        public void StartGame()
        {
            // Start game with 10 Adventurers
            for (int i = 0; i < 10; i++) Manager.Adventurers.Add();

            TurnCounter = 1;
            Stability = 100;
            Wealth = 100;
            
            OnGameStart.Invoke();
        }

        [Button("Next Turn")]
        public void NextTurnStart()
        {
            if (TurnTransitioning || InMenu) return;
            TurnTransitioning = true;
            OnNextTurnStart?.Invoke();
        }

        public void NextTurnEnd()
        {
            Buildings.placedThisTurn = 0;
            Stability += Defence - Threat;
            if (Stability > 100) Stability = 100;
            Wealth += WealthPerTurn;
            TurnCounter++;

            // Spawn adventurers based on satisfaction
            foreach (AdventurerType category in Enum.GetValues(typeof(AdventurerType)))
            {
                if (Random.Range(0, 100) < SpawnChance(category)) Adventurers.Add(category);
            }

            if (Stability <= 0) EventQueue.AddGameOverEvents();
        
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
        
            OnNextTurnEnd?.Invoke();
            
            SaveFile.SaveState();
            EnterMenu();
            UpdateUi();
            TurnTransitioning = false;
        }

        public void UpdateUi()
        {
            OnUpdateUI?.Invoke();
        }

        private async void Load()
        {
            IsLoading = true;
            await SaveFile.LoadState();
            UpdateUi();
            IsLoading = false;
        }

        private void CheckGameEnd()
        {
            if (!IsGameOver) return; 
            OnGameEnd.Invoke();
            
            Map.FillGrid();
            Manager.IsGameOver = false; //Reset for next game
            Manager.TurnCounter = 0;
            Clear.RuinsClearCount = 0;
            Clear.TerrainClearCount = 0;
            
            SaveFile.SaveState();
        }

        public void EnterMenu()
        {
            InMenu = true;
            OnEnterMenu.Invoke();
        }
    
        public void ExitMenu()
        {
            InMenu = false;
            OnExitMenu.Invoke();
        }
        
        #region Debug
        
        [Button("Print Save")]
        public void PrintSave()
        {
            Debug.Log(PlayerPrefs.GetString("Save"));
        }
        
        [Button("Extra Wealth")]
        public void ExtraWealth()
        {
            Wealth += 10000;
            UpdateUi();
        }
        
        #endregion
    }
}
