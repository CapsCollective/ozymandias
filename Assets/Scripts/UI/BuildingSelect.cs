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
    public Image costBadge;
    public Color gold, grey;
    
    public override void UpdateUi()
    {
        BuildingStats building = buildingPrefab.GetComponent<BuildingStats>();
        title.text = building.name;
        cost.text = building.ScaledCost.ToString();
        icon.sprite = building.icon;
        Color colour = building.IconColour;
        icon.color = colour;
        bool active = building.ScaledCost <= Manager.Wealth;
        if (toggle.isOn && !active)
        {
            toggle.isOn = false;
            PlacementController.Selected = Deselected;
        }
        bool interactable = building.ScaledCost <= Manager.Wealth;
        toggle.interactable = interactable;
        costBadge.color = interactable ? gold : grey;
    }

    public void ToggleSelect()
    {
        if (toggle.isOn) PlacementController.Selected = position;
        else PlacementController.Selected = Deselected;
    }
}
