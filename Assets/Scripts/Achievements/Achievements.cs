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
    }
    
    public enum Achievement
    {
        PetDog = 0,
        BuildOneBuilding = 1,
        PopulationVillage = 2,
        TurnMonth = 3,
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
            
            Structures.Structures.OnBuild += structure =>
            {
                // Ignore terrain, quests, ruins and guild halls
                if (structure.IsBuildingType(BuildingType.GuildHall) ||
                    !structure.IsBuilding) return;
                UnlockAchievement(Achievement.BuildOneBuilding);
            };

            Newspaper.OnClosed += () =>
            {
                // Turn-based progress achievements 
                var currentTurn = Manager.Stats.TurnCounter;
                UpdateStat(GameStat.Turn, currentTurn);
                UpdateProgress(GameStat.Turn, Achievement.TurnMonth, currentTurn);
                
                // Population-based progress achievements 
                var currentPopulation = Manager.Adventurers.Count;
                UpdateStat(GameStat.Population, currentPopulation);
                UpdateProgress(GameStat.Population, Achievement.PopulationVillage, currentPopulation);
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
    
