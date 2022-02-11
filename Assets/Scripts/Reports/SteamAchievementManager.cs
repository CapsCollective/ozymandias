using System.Collections.Generic;
using Steamworks;
using Steamworks.NET;

namespace Reports
{
    public static class SteamAchievementManager
    {
        public static bool Initialised => SteamManager.Initialized;

        private static readonly Dictionary<Milestone, string> StatIDs = 
            new Dictionary<Milestone, string>
            {
                {Milestone.Population, "GREATEST_POPULATION"},
                {Milestone.Turn, "GREATEST_TURN"},
                {Milestone.Cards, "UNLOCKED_CARDS"},
                {Milestone.BuildingsBuilt, "BUILDINGS_BUILT"},
                {Milestone.CampsCleared, "CAMPS_CLEARED"},
                {Milestone.RuinsDemolished, "RUINS_DEMOLISHED"},
                {Milestone.TownsDestroyed, "TOWNS_DESTROYED"},
            };

        private static readonly Dictionary<Achievement, string> AchievementIDs = 
            new Dictionary<Achievement, string>
            {
                {Achievement.TurnWeek, "TURN_WEEK"},
                {Achievement.BuildOneBuilding, "BUILD_FIRST_BUILDING"},
                
                {Achievement.PetDog, "PET_THE_DOG"},
                {Achievement.CaughtFish, "GONE_FISHING"},
                {Achievement.FoundWaterfall, "FIND_WATERFALL"},
                {Achievement.GuildHallDemolished, "DEMOLISH_GUILD_HALL"},
                {Achievement.WorldEdgeFound, "FIND_WORLD_EDGE"},
                
                {Achievement.PopulationHamlet, "POPULATION_HAMLET"},
                {Achievement.PopulationVillage, "POPULATION_VILLAGE"},
                {Achievement.PopulationCity, "POPULATION_CITY"},
                {Achievement.PopulationKingdom, "POPULATION_KINGDOM"},
                
                {Achievement.SeasonSummer, "SEASON_SUMMER"},
                {Achievement.SeasonAutumn, "SEASON_AUTUMN"},
                {Achievement.SeasonWinter, "SEASON_WINTER"},
                {Achievement.SeasonSpring, "SEASON_SPRING"},
            };

        public static void Unlock(Achievement achievement)
        {
            if (!Initialised) return;
            SteamUserStats.SetAchievement(AchievementIDs[achievement]);
            SteamUserStats.StoreStats();
        }
        
        public static void Update(Milestone stat, int value)
        {
            if (!Initialised) return; 
            SteamUserStats.SetStat(StatIDs[stat], value);
            SteamUserStats.StoreStats();
        }
        
        public static void UpdateProgress(Milestone stat, Achievement achievement, int value)
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

