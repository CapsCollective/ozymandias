using System.Collections.Generic;
using Achievements;
using Steamworks;
using Steamworks.NET;
using UnityEngine;

namespace Platform
{
    public class SteamAchievementsDelegate : AchievementsDelegate
    {
        protected override PlatformID GetPlatformId()
        {
            return PlatformID.Steam;
        }
        
        private static readonly Dictionary<GameStat, string> StatIDs = 
            new Dictionary<GameStat, string>
            {
                {GameStat.Population, "GREATEST_POPULATION"},
                {GameStat.Turn, "GREATEST_TURN"},
                {GameStat.Cards, "UNLOCKED_CARDS"}
            };

        private static readonly Dictionary<Achievement, string> AchievementIDs = 
            new Dictionary<Achievement, string>
            {
                {Achievement.TurnWeek, "TURN_WEEK"},
                {Achievement.BuildOneBuilding, "BUILD_FIRST_BUILDING"},
                
                {Achievement.PetDog, "PET_THE_DOG"},
                {Achievement.CaughtFish, "GONE_FISHING"},
                {Achievement.FoundWaterfall, "FIND_WATERFALL"},
                
                {Achievement.PopulationHamlet, "POPULATION_HAMLET"},
                {Achievement.PopulationVillage, "POPULATION_VILLAGE"},
                {Achievement.PopulationCity, "POPULATION_CITY"},
                {Achievement.PopulationKingdom, "POPULATION_KINGDOM"},
                
                {Achievement.SeasonSummer, "SEASON_SUMMER"},
                {Achievement.SeasonAutumn, "SEASON_AUTUMN"},
                {Achievement.SeasonWinter, "SEASON_WINTER"},
                {Achievement.SeasonSpring, "SEASON_SPRING"},
            };

        public override void Initialise()
        {
            // Log Steam connectivity - running this check performs initialisation
            Debug.Log($"Steamworks initialised: {SteamManager.Initialized}");
        }
        
        public override void UnlockAchievement(Achievement achievement)
        {
            if (AchievementManager.Unlocked.Contains(achievement)) return;
            AchievementManager.Unlocked.Add(achievement);
            
            // Handle Steam unlock if Steam API is active
            if (!SteamManager.Initialized) return;
            SteamUserStats.SetAchievement(AchievementIDs[achievement]);
            SteamUserStats.StoreStats();
        }
        
        public override void UpdateStat(GameStat stat, int value)
        {
            if (!SteamManager.Initialized) return; 
            SteamUserStats.SetStat(StatIDs[stat], value);
            SteamUserStats.StoreStats();
        }
        
        public override void UpdateProgress(GameStat stat, Achievement achievement, int value)
        {
            // if (!Initialised) return;
            // var achievementID = AchievementIDs[achievement];
            
            // TODO  ISteamUserStats_GetAchievementProgressLimitsInt32 fails... why??
            // var ret = SteamUserStats.GetAchievementProgressLimits(achievementID, out _, out int max);
            // SteamUserStats.GetStat(StatIDs[stat], out int upstreamStat);
            // Debug.Log($"progress for {achievementID} {value} of {max} with upstream {upstreamStat} status {ret}");
            //
            // if (value < upstreamStat || value >= max) return;
            // SteamUserStats.IndicateAchievementProgress(achievementID, (uint) value, (uint) max);
        }
        
        public override void ResetAll()
        {
            if (!SteamManager.Initialized) return;
            SteamUserStats.ResetAllStats(true);
            SteamUserStats.StoreStats();
        }
    }
}

