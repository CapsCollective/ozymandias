using System.Collections.Generic;
using Steamworks;
using UnityEngine;

namespace Achievements
{
    public static class SteamAchievementManager
    {
        public static bool Initialised => SteamManager.Initialized;

        private static readonly Dictionary<GameStat, string> StatIDs = 
            new Dictionary<GameStat, string>
            {
                {GameStat.Population, "GREATEST_POPULATION"},
                {GameStat.Turn, "GREATEST_TURN"}
            };

        private static readonly Dictionary<Achievement, string> AchievementIDs = 
            new Dictionary<Achievement, string>
            {
                {Achievement.PetDog, "PET_THE_DOG"},
                {Achievement.BuildOneBuilding, "BUILD_FIRST_BUILDING"},
                {Achievement.PopulationVillage, "POPULATION_VILLAGE"},
                {Achievement.TurnMonth, "TURN_MONTH"}
            };

        public static void Unlock(Achievement achievement)
        {
            if (!Initialised) return;
            SteamUserStats.SetAchievement(AchievementIDs[achievement]);
            SteamUserStats.StoreStats();
        }
        
        public static void Update(GameStat stat, int value)
        {
            if (!Initialised) return; 
            SteamUserStats.SetStat(StatIDs[stat], value);
            SteamUserStats.StoreStats();
        }
        
        public static void UpdateProgress(GameStat stat, Achievement achievement, int value)
        {
            if (!Initialised) return;
            // var achievementID = AchievementIDs[achievement];
            
            // TODO  ISteamUserStats_GetAchievementProgressLimitsInt32 fails... why??
            // var ret = SteamUserStats.GetAchievementProgressLimits(achievementID, out _, out int max);
            // SteamUserStats.GetStat(StatIDs[stat], out int upstreamStat);
            // Debug.Log($"progress for {achievementID} {value} of {max} with upstream {upstreamStat} status {ret}");
            //
            // if (value < upstreamStat || value >= max) return;
            // SteamUserStats.IndicateAchievementProgress(achievementID, (uint) value, (uint) max);
        }

        public static void ResetAll()
        {
            if (!Initialised) return;
            SteamUserStats.ResetAllStats(true);
            SteamUserStats.StoreStats();
        }
    }
}

