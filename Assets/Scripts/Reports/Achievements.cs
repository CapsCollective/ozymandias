using System;
using System.Collections.Generic;
using Characters;
using Events;
using Inputs;
using Managers;
using Platform;
using Structures;
using UnityEngine;
using Utilities;
using static Managers.GameManager;

namespace Reports
{
    public enum Milestone
    {
        Population = 0,
        Turn = 1,
        Cards = 2,
        CampsCleared = 3,
        TownsDestroyed = 4,
        BuildingsBuilt = 5,
        RuinsDemolished = 6,
        UpgradesPurchased = 7,
    }
    
    public enum Achievement
    {
        TurnWeek = 0,
        BuildOneBuilding = 1,

        PetDog = 2,
        CaughtFish = 3,
        FoundWaterfall = 4,
        GuildHallDemolished = 17,
        WorldEdgeFound = 18,

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
        
        PurchaseOneUpgrade = 19,
        PurchaseAllUpgrades = 20,
    }

    public class Achievements : MonoBehaviour
    {
        public HashSet<Achievement> Unlocked { get; private set; }
        public Dictionary<Milestone, int> Milestones { get; private set; }

        public void Start()
        {
            PlatformManager.Instance.Achievements.Initialise();
            
            Dog.OnDogPet += () => UnlockAchievement(Achievement.PetDog);
            Fishing.OnFishCaught += () => UnlockAchievement(Achievement.CaughtFish);
            Waterfall.OnOpened += () => UnlockAchievement(Achievement.FoundWaterfall);
            CentreButton.OnWorldEdge += () => UnlockAchievement(Achievement.WorldEdgeFound);
            Structures.Structures.OnGuildHallDemolished += () => UnlockAchievement(Achievement.GuildHallDemolished);

            State.OnGameEnd += () => UpdateStat(Milestone.TownsDestroyed, Milestones[Milestone.TownsDestroyed] + 1);
            
            Structures.Structures.OnBuild += structure =>
            {
                // Ignore terrain, quests, ruins and guild halls
                if (structure.IsBuildingType(BuildingType.GuildHall) || !structure.IsBuilding) return;
                UpdateStat(Milestone.BuildingsBuilt, Milestones[Milestone.BuildingsBuilt] + 1);
                UnlockAchievement(Achievement.BuildOneBuilding);
            };
            
            Select.OnClear += structure =>
            {
                if (structure.IsRuin) UpdateStat(Milestone.RuinsDemolished, Milestones[Milestone.RuinsDemolished] + 1);
            };
            
            Quests.Quests.OnQuestRemoved += quest =>
            {
                if (quest.IsRadiant) UpdateStat(Milestone.CampsCleared, Milestones[Milestone.CampsCleared] + 1);
            };

            Cards.Cards.OnUnlock += _ =>
            {
                // Card-based progress achievements 
                int currentCards = Manager.Cards.UnlockedCards;
                if (currentCards <= Milestones[Milestone.Cards]) return;
                UpdateStat(Milestone.Cards, currentCards);
                UpdateProgress(Milestone.Cards, Achievement.Card1, currentCards);
                UpdateProgress(Milestone.Cards, Achievement.Card5, currentCards);
                UpdateProgress(Milestone.Cards, Achievement.Card10, currentCards);
                UpdateProgress(Milestone.Cards, Achievement.Card15, currentCards);
            };

            Newspaper.OnClosed += () =>
            {
                // Turn-based progress achievements 
                int currentTurn = Manager.Stats.TurnCounter;
                if (currentTurn > Milestones[Milestone.Turn])
                {
                    UpdateStat(Milestone.Turn, currentTurn);
                    UpdateProgress(Milestone.Turn, Achievement.TurnWeek, currentTurn);
                    UpdateProgress(Milestone.Turn, Achievement.SeasonSummer, currentTurn);
                    UpdateProgress(Milestone.Turn, Achievement.SeasonAutumn, currentTurn);
                    UpdateProgress(Milestone.Turn, Achievement.SeasonWinter, currentTurn);
                    UpdateProgress(Milestone.Turn, Achievement.SeasonSpring, currentTurn);
                }
                
                // Population-based progress achievements 
                int currentPopulation = Manager.Adventurers.Count;
                if (currentPopulation > Milestones[Milestone.Population]) 
                {
                    UpdateStat(Milestone.Population, currentPopulation);
                    UpdateProgress(Milestone.Population, Achievement.PopulationHamlet, currentPopulation);
                    UpdateProgress(Milestone.Population, Achievement.PopulationVillage, currentPopulation);
                    UpdateProgress(Milestone.Population, Achievement.PopulationCity, currentPopulation);
                    UpdateProgress(Milestone.Population, Achievement.PopulationKingdom, currentPopulation);
                }
            };

            Upgrades.Upgrades.OnUpgradePurchased += _ =>
            {
                int upgradesPurchased = Manager.Upgrades.UpgradesPurchased;
                
                UpdateStat(Milestone.UpgradesPurchased, upgradesPurchased);
                UnlockAchievement(Achievement.PurchaseOneUpgrade);
                if (upgradesPurchased == Manager.Upgrades.TotalUpgrades) UnlockAchievement(Achievement.PurchaseAllUpgrades);
            };
        }

        private void UnlockAchievement(Achievement achievement)
        {
            if (Unlocked.Contains(achievement)) return;
            Unlocked.Add(achievement);
            
            // Handle Steam unlock if Steam API is active
            PlatformManager.Instance.Achievements.UnlockAchievement(achievement);
        }
        
        private void UpdateStat(Milestone stat, int value)
        {
            Milestones[stat] = value;
            PlatformManager.Instance.Achievements.UpdateStat(stat, value);
        }
        
        private static void UpdateProgress(Milestone stat, Achievement achievement, int value)
        {
            PlatformManager.Instance.Achievements.UpdateProgress(stat, achievement, value);
        }
        
        public static void ResetAll()
        {
            PlatformManager.Instance.Achievements.ResetAll();
        }

        public AchievementDetails Save()
        {
            return new AchievementDetails
            {
                unlocked = Unlocked,
                milestones = Milestones
            };
        }
        public void Load(AchievementDetails details)
        {
            Milestones = details.milestones ?? new Dictionary<Milestone, int>();
            Unlocked = details.unlocked ?? new HashSet<Achievement>();
            
            foreach (Milestone milestone in Enum.GetValues(typeof(Milestone)))
            {
                if (Milestones.ContainsKey(milestone)) continue;
                Milestones.Add(milestone, 0);
            }
        }
    }
}
    
