using System;
using System.Collections.Generic;
using Events;
using UnityEngine;
using Utilities;
using WalkingAdventurers;
using Steam = Achievements.SteamAchievementManager;
using static Managers.GameManager;

namespace Achievements
{
    public enum GameStat
    {
        Population = 0,
        Turn = 1,
        Cards = 2,
    }
    
    public enum Achievement
    {
        TurnWeek = 0,
        BuildOneBuilding = 1,

        PetDog = 2,
        CaughtFish = 3,
        FoundWaterfall = 4,

        PopulationHamlet = 5,
        PopulationVillage = 6,
        PopulationCity = 7,
        PopulationKingdom = 8,
        
        SeasonSummer = 9,
        SeasonAutumn = 10,
        SeasonWinter = 11,
        SeasonSpring = 12,
        
        Card1 = 13,
        Card5 = 14,
        Card10 = 15,
        Card15 = 16,
    }

    public class AchievementManager : MonoBehaviour
    {
        private List<Achievement> Unlocked { get; } = new List<Achievement>();

        public void Start()
        {
            // Log Steam connectivity - running this check performs initialisation
            Debug.Log($"Steamworks initialised: {Steam.Initialised}");
            
            Dog.OnDogPet += () =>
            {
                UnlockAchievement(Achievement.PetDog);
            };
            
            Fishing.OnFishCaught += () =>
            {
                UnlockAchievement(Achievement.CaughtFish);
            };
            
            Waterfall.OnOpened += () =>
            {
                UnlockAchievement(Achievement.FoundWaterfall);
            };
            
            Structures.Structures.OnBuild += structure =>
            {
                // Ignore terrain, quests, ruins and guild halls
                if (structure.IsBuildingType(BuildingType.GuildHall) ||
                    !structure.IsBuilding) return;
                UnlockAchievement(Achievement.BuildOneBuilding);
            };

            Cards.Cards.OnUnlock += _ =>
            {
                // Card-based progress achievements 
                var currentCards = Manager.Cards.UnlockedCards;
                UpdateStat(GameStat.Cards, currentCards);
                UpdateProgress(GameStat.Cards, Achievement.Card1, currentCards);
                UpdateProgress(GameStat.Cards, Achievement.Card5, currentCards);
                UpdateProgress(GameStat.Cards, Achievement.Card10, currentCards);
                UpdateProgress(GameStat.Cards, Achievement.Card15, currentCards);
            };

            Newspaper.OnClosed += () =>
            {
                // Turn-based progress achievements 
                var currentTurn = Manager.Stats.TurnCounter;
                UpdateStat(GameStat.Turn, currentTurn);
                UpdateProgress(GameStat.Turn, Achievement.TurnWeek, currentTurn);
                UpdateProgress(GameStat.Turn, Achievement.SeasonSummer, currentTurn);
                UpdateProgress(GameStat.Turn, Achievement.SeasonAutumn, currentTurn);
                UpdateProgress(GameStat.Turn, Achievement.SeasonWinter, currentTurn);
                UpdateProgress(GameStat.Turn, Achievement.SeasonSpring, currentTurn);
                
                // Population-based progress achievements 
                var currentPopulation = Manager.Adventurers.Count;
                UpdateStat(GameStat.Population, currentPopulation);
                UpdateProgress(GameStat.Population, Achievement.PopulationHamlet, currentPopulation);
                UpdateProgress(GameStat.Population, Achievement.PopulationVillage, currentPopulation);
                UpdateProgress(GameStat.Population, Achievement.PopulationCity, currentPopulation);
                UpdateProgress(GameStat.Population, Achievement.PopulationKingdom, currentPopulation);
            };
        }

        private void UnlockAchievement(Achievement achievement)
        {
            if (Unlocked.Contains(achievement)) return;
            Unlocked.Add(achievement);
            
            // Handle Steam unlock if Steam API is active
            Steam.Unlock(achievement);
        }
        
        private static void UpdateStat(GameStat stat, int value)
        {
            Steam.Update(stat, value);
        }
        
        private static void UpdateProgress(GameStat stat, Achievement achievement, int value)
        {
            Steam.UpdateProgress(stat, achievement, value);
        }
        
        public static void ResetAll()
        {
            Steam.ResetAll();
        }
    }
}
    
