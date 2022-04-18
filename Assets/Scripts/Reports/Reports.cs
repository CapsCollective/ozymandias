using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;
using Utilities;
using static Managers.GameManager;

namespace Reports
{
    public class Reports : UiUpdater
    {
        [SerializeField] private TextMeshProUGUI
            cardUnlocks, longestRun, greatestPopulation, campsCleared, townsDestroyed, ruinsDemolished, buildingsBuilt,
            petDogText, fishingText, worldEdgeText, guildHallDemolishedText, waterfallText, birdsText;
        [SerializeField] private Image
            petDogImage, fishingImage, worldEdgeImage, guildHallDemolishedImage, waterfallImage, birdsImage;
        [SerializeField] private Sprite
            petDogIcon, fishingIcon, worldEdgeIcon, guildHallDemolishedIcon, waterfallIcon, birdsIcon, lockedIcon;

        protected override void UpdateUi()
        {
            cardUnlocks.text = $"{Manager.Cards.UnlockedCards}/15 Cards unlocked ({Manager.Upgrades.GetLevel(UpgradeType.Discoveries)} discoverable from ruins)"; 
            
            longestRun.text = "Longest Run\n" + Manager.Achievements.Milestones[Milestone.Turn] + " Turns";
            greatestPopulation.text = "Greatest Population\n" + Manager.Achievements.Milestones[Milestone.Population] + " Adventurers";
            campsCleared.text = "Enemy Camps Cleared\n" + Manager.Achievements.Milestones[Milestone.CampsCleared] + " Camps";
            townsDestroyed.text = "Towns Destroyed\n" + Manager.Achievements.Milestones[Milestone.TownsDestroyed] + " Towns";
            ruinsDemolished.text = "Ruins Demolished\n" + Manager.Achievements.Milestones[Milestone.RuinsDemolished] + " Ruins";
            buildingsBuilt.text = "Buildings Built\n" + Manager.Achievements.Milestones[Milestone.BuildingsBuilt] + " Buildings";

            petDogText.text = Manager.Achievements.Unlocked.Contains(Achievement.PetDog) ? "You Can Pet the Dog" : "???";
            fishingText.text = Manager.Achievements.Unlocked.Contains(Achievement.CaughtFish) ? "Fishing Simulator" : "???";
            worldEdgeText.text = Manager.Achievements.Unlocked.Contains(Achievement.WorldEdgeFound) ? "Edge of the World" : "???";
            guildHallDemolishedText.text = Manager.Achievements.Unlocked.Contains(Achievement.GuildHallDemolished) ? "Destructive Tendencies" : "???";
            waterfallText.text = Manager.Achievements.Unlocked.Contains(Achievement.FoundWaterfall) ? "Behind the Waterfall" : "???";
            //birdsText.text = Manager.Achievements.Unlocked.Contains(Achievement.Birds) ? "Swooping Bird" : "???";
            
            petDogImage.sprite = Manager.Achievements.Unlocked.Contains(Achievement.PetDog) ? petDogIcon : lockedIcon;
            fishingImage.sprite = Manager.Achievements.Unlocked.Contains(Achievement.CaughtFish) ? fishingIcon : lockedIcon;
            worldEdgeImage.sprite = Manager.Achievements.Unlocked.Contains(Achievement.WorldEdgeFound) ? worldEdgeIcon : lockedIcon;
            guildHallDemolishedImage.sprite = Manager.Achievements.Unlocked.Contains(Achievement.GuildHallDemolished) ? guildHallDemolishedIcon : lockedIcon;
            waterfallImage.sprite = Manager.Achievements.Unlocked.Contains(Achievement.FoundWaterfall) ? waterfallIcon : lockedIcon;
            //birdsImage.sprite = Manager.Achievements.Unlocked.Contains(Achievement.Birds) ? birdsIcon : lockedIcon;
        }
    }
}
