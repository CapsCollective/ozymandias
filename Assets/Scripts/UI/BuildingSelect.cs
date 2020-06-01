using System.Collections;
using System.Collections.Generic;
using System.Management.Instrumentation;
using TMPro;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UI;
using static GameManager;

public class BuildingSelect : UiUpdater
{
    public GameObject buildingPrefab;
    public TextMeshProUGUI title;
    public Image icon;
    public TextMeshProUGUI cost;
    public Toggle toggle;

    public override void UpdateUi()
    {
        BuildingStats building = buildingPrefab.GetComponent<BuildingStats>();
        title.text = building.name;
        cost.text = "Cost: " + building.ScaledCost;
        icon.sprite = building.icon;
        bool active = building.ScaledCost <= Manager.Wealth;
        if (toggle.isOn && !active)
        {
            toggle.isOn = false;
            PlacementController.selectedObject = null;
        }
        toggle.interactable = building.ScaledCost <= Manager.Wealth;
    }

    public void ToggleSelect()
    {
        if (toggle.isOn) PlacementController.selectedObject = this;
        else PlacementController.selectedObject = null;
    }
}
