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
    public CanvasGroup canvasGroup;
    
    public override void UpdateUi()
    {
        BuildingStats building = buildingPrefab.GetComponent<BuildingStats>();
        Color colour = building.IconColour;
        title.text = building.name;
        cost.text = building.ScaledCost.ToString();
        icon.sprite = building.icon;
        icon.color = colour;
        bool active = building.ScaledCost <= Manager.Wealth;
        if (toggle.isOn && !active)
        {
            toggle.isOn = false;
            PlacementManager.Selected = Deselected;
        }
        bool interactable = building.ScaledCost <= Manager.Wealth;
        toggle.interactable = interactable;
        canvasGroup.alpha = interactable ? 1 : 0.4f;
    }

    public void ToggleSelect()
    {
        if (toggle.isOn) PlacementManager.Selected = position;
        else PlacementManager.Selected = Deselected;
    }
}
