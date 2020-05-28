using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static GameManager;
using UnityEngine.EventSystems;

public class Clear : MonoBehaviour
{
    public const float costScale = 1.15f;
    
    private Cell[] highlighted = new Cell[1];
    private Map map;
    private EventSystem eventSystem;  

    private bool clearMode = false;
    private BuildingStructure selectedBuilding;
    private int clearCount = 0;
    
    public int baseCost = 5;

    public Image icon;
    public Sprite deselected;
    public Sprite selected;

    public int ScaledCost => Mathf.FloorToInt( baseCost * Mathf.Pow(costScale, clearCount));

    private void Awake()
    {
        map = Manager.map;
        eventSystem = EventSystem.current;

        ClickManager.OnLeftClick += LeftClick;
    }

    // Update is called once per frame
    void Update()
    {
        selectedBuilding = null;
        if (!clearMode) return;
        
        map.Highlight(highlighted, Map.HighlightState.Inactive);
        highlighted = new Cell[1];
        
        if (Manager.Wealth < ScaledCost)
        {
            ExitClearMode();
            return;
        }

        if (eventSystem.IsPointerOverGameObject()) return;
        
        Cell closest = map.GetCellFromMouse();

        selectedBuilding = closest.occupant;
        if (selectedBuilding) highlighted = map.GetCells(selectedBuilding);
        else highlighted[0] = closest;
        
        Map.HighlightState state = selectedBuilding ? Map.HighlightState.Valid : Map.HighlightState.Invalid;
        map.Highlight(highlighted, state);
    }
    
    public void LeftClick()
    {
        if (clearMode && selectedBuilding) ClearBuilding();
        if (eventSystem.currentSelectedGameObject && eventSystem.currentSelectedGameObject.gameObject != gameObject) ExitClearMode();
    }
    
    public void ToggleClearMode()
    {
        if (clearMode) ExitClearMode();
        else EnterClearMode();
    }

    public void EnterClearMode()
    {
        clearMode = true;
        icon.sprite = selected;
    }

    public void ExitClearMode()
    {
        clearMode = false;
        map?.Highlight(highlighted, Map.HighlightState.Inactive);
        highlighted = new Cell[1];
        icon.sprite = deselected;
    }
    
    public void ClearBuilding()
    {
        if (!Manager.Spend(ScaledCost)) return;
        BuildingStats building = selectedBuilding.GetComponent<BuildingStats>();
        if (building.terrain) clearCount++;
        Manager.Demolish(building);
        //ExitClearMode();
    }
}
