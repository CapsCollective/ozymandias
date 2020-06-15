using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static GameManager;

public class BuildingSelect : UiUpdater
{
    public const int Deselected = -1;
    public int position;
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
        Color colour = building.IconColour;
        //title.color = colour;
        icon.color = colour;
        //cost.color = colour;
        bool active = building.ScaledCost <= Manager.Wealth;
        if (toggle.isOn && !active)
        {
            toggle.isOn = false;
            PlacementController.Selected = Deselected;
        }
        toggle.interactable = building.ScaledCost <= Manager.Wealth;
    }

    public void ToggleSelect()
    {
        if (toggle.isOn) PlacementController.Selected = position;
        else PlacementController.Selected = Deselected;
    }
}
