﻿using System;
using Events;
using NaughtyAttributes;
using Tooltip;
using UnityEngine;
using Utilities;
using Random = UnityEngine.Random;

namespace Managers
{

    public class GameManager : MonoBehaviour
    {
        public static GameManager Manager { get; private set; }
        
        // All Managers/ Universal Controllers
        public State State { get; private set; }
        public Stats Stats { get; private set; }
        public Adventurers.Adventurers Adventurers { get; private set; }
        public Buildings.Buildings Buildings { get; private set; }
        public Achievements.Achievements Achievements { get; private set; }
        public Cards.Cards Cards { get; private set; }
        public Quests.Quests Quests { get; private set; }
        public EventQueue EventQueue { get; private set; }
        public Map.Map Map { get; private set; }
        public Jukebox Jukebox { get; private set; }
        public Inputs.Inputs Inputs { get; private set; }
        public TooltipDisplay Tooltip { get; private set; }

        private void Awake()
        {
            Manager = this;
            Random.InitState((int)DateTime.Now.Ticks);
            State = FindObjectOfType<State>();
            Stats = FindObjectOfType<Stats>();
            Adventurers = FindObjectOfType<Adventurers.Adventurers>();
            Buildings = FindObjectOfType<Buildings.Buildings>();
            Achievements = FindObjectOfType<Achievements.Achievements>();
            Cards = FindObjectOfType<Cards.Cards>();
            Quests = FindObjectOfType<Quests.Quests>();
            EventQueue = FindObjectOfType<EventQueue>();
            Map = FindObjectOfType<Map.Map>();
            Jukebox = FindObjectOfType<Jukebox>();
            Inputs = new Inputs.Inputs();
            Tooltip = FindObjectOfType<TooltipDisplay>();

            State.EnterState(GameState.Loading);
        }

        public static Action OnUpdateUI;
        public static void UpdateUi()
        {
            OnUpdateUI.Invoke();
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
            Stats.Wealth += 10000;
            UpdateUi();
        }
        
        #endregion
    }
}
