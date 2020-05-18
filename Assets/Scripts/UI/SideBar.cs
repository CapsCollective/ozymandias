using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static GameManager;

public class SideBar : UiUpdater
{
    public Text adventurers, satisfaction, effectiveness;//, threat, defense;
    // Update is called once per frame
    public override void UpdateUi()
    {
        adventurers.text = "Adventurers: " + 
                           Manager.AvailableAdventurers +
                           " / " +
                           Manager.Accommodation;
        
        satisfaction.text = "Satisfaction: " + Manager.Satisfaction + "%";

        effectiveness.text = "Effectiveness: " + Manager.Effectiveness + "%";

        //threat.text = "Threat: " + Manager.Threat;
        //defense.text = "Defense: " + Manager.Defense;
    }
}
