using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static GameManager;

public class AdventurersUi : UiUpdater
{
    public TextMeshProUGUI adventurerText;
    public override void UpdateUi()
    {
        bool overCapacity = Manager.AvailableAdventurers - Manager.Accommodation > 0;
        adventurerText.text = "Adventurers: " + (overCapacity ? "<color=red>" : "") + Manager.AvailableAdventurers + (overCapacity ? "</color>" : "") + " / " + Manager.Accommodation;
    }
}
