﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UI;

public class BuildingHelper : MonoBehaviour, TooltipHelper
{
    public TextMeshProUGUI title, description;

    public void UpdateTooltip()
    {
        BuildingStats buildingStats = transform.parent.GetComponent<BuildingSelect>().buildingPrefab.GetComponent<BuildingStats>();
        title.text = "~ " + buildingStats.name + " ~";
        description.text = buildingStats.description;
    }
}
