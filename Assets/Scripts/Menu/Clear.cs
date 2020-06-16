using System.Collections;
using System.Collections.Generic;
using Managers_and_Controllers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static GameManager;
using UnityEngine.EventSystems;

public class Clear : UiUpdater
{
    public const float CostScale = 1.03f;
    
    private Cell[] highlighted = new Cell[1];
    private Map map;
    private EventSystem eventSystem;  

    private BuildingStructure selectedBuilding;
    public static int ClearCount = 0;
    
    public int baseCost = 30;

    public Image icon;
    public Sprite deselected;
    public Sprite selected;
    
    public Toggle toggle;
    
    public int ScaledCost => Mathf.FloorToInt( baseCost * Mathf.Pow(CostScale, ClearCount));

    public TextMeshProUGUI cost;
    public override void UpdateUi()
    {
        cost.text = GetComponent<Clear>().ScaledCost.ToString();
        bool active = Manager.Wealth >= ScaledCost;
        if (!active)
        {
            toggle.isOn = false;
            ExitClearMode();
        }
        toggle.interactable = active;
    }
    
    private void Start()
    {
        map = Manager.map;
        eventSystem = EventSystem.current;

        ClickManager.OnLeftClick += LeftClick;
    }

    // Update is called once per frame
    void Update()
    {
        selectedBuilding = null;
        if (!toggle.isOn) return;
        
        map.Highlight(highlighted, Map.HighlightState.Inactive);
        highlighted = new Cell[1];
        
        if (eventSystem.IsPointerOverGameObject()) return;
        
        Cell closest = map.GetCellFromMouse();

        selectedBuilding = closest.occupant;
        if (selectedBuilding) highlighted = map.GetCells(selectedBuilding);
        else highlighted[0] = closest;
        
        Map.HighlightState state = selectedBuilding && !selectedBuilding.indestructable ? Map.HighlightState.Valid : Map.HighlightState.Invalid;
        map.Highlight(highlighted, state);
    }
    
    public void LeftClick()
    {
        if (toggle.isOn && selectedBuilding) ClearBuilding();
    }
    
    public void ToggleClearMode()
    {
        if (!toggle.isOn) ExitClearMode();
        else EnterClearMode();
    }

    public void EnterClearMode()
    {
        icon.sprite = selected;
        CursorController.Instance.SwitchCursor(CursorController.CursorType.Destroy);
    }

    public void ExitClearMode()
    {
        map?.Highlight(highlighted, Map.HighlightState.Inactive);
        highlighted = new Cell[1];
        icon.sprite = deselected;
        if (CursorController.Instance.currentCursor == CursorController.CursorType.Destroy)
            CursorController.Instance.SwitchCursor(CursorController.CursorType.Pointer);
    }
    
    public void ClearBuilding()
    {
        if (selectedBuilding.indestructable) return;
        BuildingStats building = selectedBuilding.GetComponent<BuildingStats>();
        BuildingStructure buildingStructure = selectedBuilding.GetComponent<BuildingStructure>();
        if (!Manager.Spend(ScaledCost)) return;
        if (building.terrain) ClearCount++;
        buildingStructure.Clear();
        Manager.Demolish(building);
    }
    
    private void OnDestroy()
    {
        ClearCount = 0;
    }
    
}
