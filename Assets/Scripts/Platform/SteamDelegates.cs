#if UNITY_STANDALONE

using System.Collections.Generic;
using Reports;
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
        
        private static readonly Dictionary<Milestone, string> StatIDs = 
            new Dictionary<Milestone, string>
            {
                {Milestone.Population, "GREATEST_POPULATION"},
                {Milestone.Turn, "GREATEST_TURN"},
                {Milestone.Cards, "UNLOCKED_CARDS"},
                {Milestone.CampsCleared, "CAMPS_CLEARED"},
                {Milestone.TownsDestroyed, "TOWNS_DESTROYED"},
                {Milestone.BuildingsBuilt, "BUILDINGS_BUILT"},
                {Milestone.RuinsDemolished, "RUINS_DEMOLISHED"},
                {Milestone.UpgradesPurchased, "UPGRADES_PURCHASED"}
            };

        private static readonly Dictionary<Achievement, string> AchievementIDs = 
            new Dictionary<Achievement, string>
            {
                {Achievement.TurnWeek, "TURN_WEEK"},
                {Achievement.BuildOneBuilding, "BUILD_FIRST_BUILDING"},
                
                {Achievement.PetDog, "PET_THE_DOG"},
                {Achievement.CaughtFish, "GONE_FISHING"},
                {Achievement.FoundWaterfall, "FIND_WATERFALL"},
                {Achievement.WorldEdgeFound, "WORLD_EDGE"},
                {Achievement.GuildHallDemolished, "DESTROY_GUILDHALL"},

                {Achievement.PopulationHamlet, "POPULATION_HAMLET"},
                {Achievement.PopulationVillage, "POPULATION_VILLAGE"},
                {Achievement.PopulationCity, "POPULATION_CITY"},
                {Achievement.PopulationKingdom, "POPULATION_KINGDOM"},
                
                {Achievement.SeasonSummer, "SEASON_SUMMER"},
                {Achievement.SeasonAutumn, "SEASON_AUTUMN"},
                {Achievement.SeasonWinter, "SEASON_WINTER"},
                {Achievement.SeasonSpring, "SEASON_SPRING"},
                
                {Achievement.PurchaseOneUpgrade, "PURCHASE_ONE_UPGRADE"},
                {Achievement.PurchaseAllUpgrades, "PURCHASE_ALL_UPGRADES"},
                
                {Achievement.TownsDestroyed10, "TOWNS_DESTROYED_10"},
                {Achievement.Ruins100, "RUINS_DEMOLISHED_100"},
                {Achievement.Buildings1000, "BUILDINGS_BUILT_1000"},
            };

        public override void Initialise()
        {
            // Log Steam connectivity - running this check performs initialisation
            Debug.Log($"Steamworks initialised: {SteamManager.Initialized}");
        }
        
        public override void UnlockAchievement(Achievement achievement)
        {
            // Handle Steam unlock if Steam API is active
            if (!SteamManager.Initialized || !AchievementIDs.ContainsKey(achievement)) return;
            SteamUserStats.SetAchievement(AchievementIDs[achievement]);
            SteamUserStats.StoreStats();
        }
        
        public override void UpdateStat(Milestone stat, int value)
        {
            if (!SteamManager.Initialized || !StatIDs.ContainsKey(stat)) return;
            SteamUserStats.SetStat(StatIDs[stat], value);
            SteamUserStats.StoreStats();
        }
        
        public override void UpdateProgress(Milestone stat, Achievement achievement, int value)
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
#endif

