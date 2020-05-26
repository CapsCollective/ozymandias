using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static GameManager;

public class SideBarSmall : UiUpdater
{
    public TextMeshProUGUI adventurers, spending;

    public BarFill effectiveness, satisfaction;
    
    // Update is called once per frame
    public override void UpdateUi()
    {
        adventurers.text = Manager.AvailableAdventurers + " / " + Manager.Accommodation;
        spending.text = "x" + (Manager.Spending / 100f).ToString("0.00");
        
        satisfaction.SetBar(Manager.Satisfaction);
        effectiveness.SetBar(Manager.Effectiveness);

    }
}
