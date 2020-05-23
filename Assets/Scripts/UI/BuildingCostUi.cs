using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingCostUi : UiUpdater
{
    public Text cost;
    public override void UpdateUi()
    {
        cost.text = "Cost: " + GetComponent<Click>().building.GetComponent<BuildingStats>().ScaledCost;
    }
}
