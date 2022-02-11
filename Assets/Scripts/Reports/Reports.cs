using TMPro;
using UI;
using UnityEngine;
using Utilities;
using static Managers.GameManager;

namespace Reports
{
    public class Reports : UiUpdater
    {
        [SerializeField]
        private TextMeshProUGUI cardUnlocks, longestRun, greatestPopulation, campsCleared, townsDestroyed, ruinsDemolished, buildingsBuilt, petDog, fishingSimulator, worldEdge, guildHallDemolished, waterfall;

        protected override void UpdateUi()
        {
            cardUnlocks.text = $"{Manager.Cards.UnlockedCards}/15 Cards unlocked ({Manager.Upgrades.GetLevel(UpgradeType.Discoveries)} discoverable from ruins)"; 
            
            longestRun.text = "Longest Run\n" + Manager.Achievements.Milestones[Milestone.Turn] + " Turns";
            greatestPopulation.text = "Greatest Population\n" + Manager.Achievements.Milestones[Milestone.Turn] + " Adventurers";
            campsCleared.text = "Enemy Camps Cleared\n" + Manager.Achievements.Milestones[Milestone.Turn] + " Camps";
            townsDestroyed.text = "Towns Destroyed\n" + Manager.Achievements.Milestones[Milestone.TownsDestroyed] + " Towns";
            ruinsDemolished.text = "Ruins Demolished\n" + Manager.Achievements.Milestones[Milestone.Turn] + " Ruins";
            buildingsBuilt.text = "Buildings Built\n" + Manager.Achievements.Milestones[Milestone.Turn] + " Buildings";

            petDog.text = Manager.Achievements.Unlocked.Contains(Achievement.PetDog) ? "You Can Pet the Dog" : "???";
            fishingSimulator.text = Manager.Achievements.Unlocked.Contains(Achievement.CaughtFish) ? "Fishing Simulator" : "???";
            worldEdge.text = Manager.Achievements.Unlocked.Contains(Achievement.WorldEdgeFound) ? "Edge of the World" : "???";
            guildHallDemolished.text = Manager.Achievements.Unlocked.Contains(Achievement.GuildHallDemolished) ? "Destructive Tendencies" : "???";
            waterfall.text = Manager.Achievements.Unlocked.Contains(Achievement.FoundWaterfall) ? "Behind the Waterfall" : "???";
        }
    }
}
