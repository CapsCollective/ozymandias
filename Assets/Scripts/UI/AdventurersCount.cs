using TMPro;
using UnityEngine;
using Utilities;
using static Managers.GameManager;

namespace UI
{
    public class AdventurersCount : UiUpdater
    {
        [SerializeField] private TextMeshProUGUI adventurerText;

        protected override void UpdateUi()
        {
            bool overCapacity = Manager.Adventurers.Available - Manager.GetStat(Stat.Housing) > 0;
            adventurerText.text = "Adventurers: " + (overCapacity ? "<color=red>" : "") + 
                Manager.Adventurers.Available + (overCapacity ? "</color>" : "") + " / " + 
                Manager.GetStat(Stat.Housing);
        }
    }
}
