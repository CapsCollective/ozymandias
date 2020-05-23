using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClearCostUi : UiUpdater
{
    public Text cost;
    public override void UpdateUi()
    {
        cost.text = "Cost: " + GetComponent<Clear>().ScaledCost;
    }
}
