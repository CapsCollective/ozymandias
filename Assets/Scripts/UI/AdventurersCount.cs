#pragma warning disable 0649
using TMPro;
using UnityEngine;
using static Managers.GameManager;

namespace UI
{
    public class AdventurersCount : UiUpdater
    {
        [SerializeField] private TextMeshProUGUI adventurerText;

        protected override void UpdateUi()
        {
            bool overCapacity = Manager.Adventurers.Available - Manager.Accommodation > 0;
            adventurerText.text = "Adventurers: " + (overCapacity ? "<color=red>" : "") + Manager.Adventurers.Available + (overCapacity ? "</color>" : "") + " / " + Manager.Accommodation;
        }
    }
}
