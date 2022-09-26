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
        //[SerializeField] private TextMeshProUGUI defenceCounts;
        protected override void UpdateUi()
        {
            if (Manager.State.InGame) return;
            
            foreach (Guild guild in Enum.GetValues(typeof(Guild)))
            {
                int difference = Manager.Adventurers.GetCount(guild) - Manager.Stats.AdventurerHistory[guild].DefaultIfEmpty(0).Last();
                adventurerCounts[guild].text = (difference >= 0 ? "+" : Colors.RedText) + difference + (Math.Sign(difference) >= 0 ? "" : Colors.EndText);
            }
            //defenceCounts.text = "= " + (Manager.Stats.Defence - Manager.Stats.StatHistory[Stat.Defence].DefaultIfEmpty(0).Last());
        }
    }
}
