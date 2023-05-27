using System;
using System.Linq;
using TMPro;
using UI;
using UnityEngine;
using Utilities;
using static Managers.GameManager;

namespace Events
{
    public class NewTurnAdventurers: UiUpdater
    {
        [SerializeField] private SerializedDictionary<Guild, TextMeshProUGUI> adventurerCounts;
        [SerializeField] private GameObject newAdventurersTitle, newAdventurersSeparator;
        protected override void UpdateUi()
        {
            if (Manager.State.InGame) return;

            bool anyChanged = false;
            foreach (Guild guild in Enum.GetValues(typeof(Guild)))
            {
                int previousAvailable = Manager.Stats.AdventurerHistory[guild].DefaultIfEmpty(0).Last();
                int currentAvailable = Manager.Adventurers.GetCount(guild);
                int difference = currentAvailable - previousAvailable;
                if (difference != 0) anyChanged = true;
                PopulateBadgeValue(guild, difference);
            }

            bool displayNewTurnAdventurers = anyChanged && !Manager.State.IsGameOver;
            newAdventurersTitle.SetActive(displayNewTurnAdventurers);
            gameObject.SetActive(displayNewTurnAdventurers);
            newAdventurersSeparator.SetActive(displayNewTurnAdventurers);
        }

        private void PopulateBadgeValue(Guild guild, int difference)
        {
            adventurerCounts[guild].text = difference switch
            {
                > 0 => "+" + difference,
                < 0 => difference.ToString().StatusColor(-1),
                _ => ""
            };
        }
    }
}
