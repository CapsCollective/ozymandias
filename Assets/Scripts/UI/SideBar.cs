using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static GameManager;

public class SideBar : UiUpdater
{
    public Text adventurers, satisfaction, effectiveness, spending;//, threat, defense;
    // Update is called once per frame
    public override void UpdateUi()
    {
        adventurers.text = Manager.AvailableAdventurers + " / " + Manager.Accommodation;
        satisfaction.text = Manager.Satisfaction + "%";
        effectiveness.text = Manager.Effectiveness + "%";
        spending.text = Manager.Spending + "%";
        //threat.text = "Threat: " + Manager.Threat;
        //defense.text = "Defense: " + Manager.Defense;
    }
}
