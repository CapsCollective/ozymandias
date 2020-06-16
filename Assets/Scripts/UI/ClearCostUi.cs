using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ClearCostUi : UiUpdater
{
    public TextMeshProUGUI cost;
    public override void UpdateUi()
    {
        cost.text = GetComponent<Clear>().ScaledCost.ToString();
    }
}
