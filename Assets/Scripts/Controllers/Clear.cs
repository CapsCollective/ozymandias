using Controllers;
using Managers_and_Controllers;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;
using static GameManager;
using UnityEngine.EventSystems;
using Utilities;
using static Controllers.CursorSelect;

public class Clear : UiUpdater
{
    public const float CostScale = 1.025f;
    
    private Cell[] highlighted = new Cell[1];
    private EventSystem eventSystem;  

    private BuildingStructure selectedBuilding;
    public static int ClearCount = 0;
    
    public int baseCost = 30;

    public Image icon;
    public Sprite deselected;
    public Sprite selected;
    
    public Toggle toggle;
    
    public Image costBadge;
    public Color gold, grey;
    public CanvasGroup canvasGroup;
    
    public int ScaledCost => Mathf.FloorToInt( baseCost * Mathf.Pow(CostScale, ClearCount));

    public TextMeshProUGUI cost;
    public override void UpdateUi()
    {
        cost.text = ScaledCost.ToString();
        bool active = Manager.Wealth >= ScaledCost;
        if (!active)
        {
            toggle.isOn = false;
            ExitClearMode();
        }
        toggle.interactable = active;
        costBadge.color = active ? gold : grey;
        canvasGroup.alpha = active ? 1 : 0.4f;
    }
    
    private void Start()
    {
        eventSystem = EventSystem.current;

        ClickManager.OnLeftClick += LeftClick;
    }

    // Update is called once per frame
    void Update()
    {
        selectedBuilding = null;
        if (!toggle.isOn) return;
        
        Manager.Map.Highlight(highlighted, Map.HighlightState.Inactive);
        highlighted = new Cell[1];
        
        if (eventSystem.IsPointerOverGameObject()) return;
        
        Cell closest = Manager.Map.GetCellFromMouse();

        selectedBuilding = closest.occupant;
        if (selectedBuilding) highlighted = Manager.Map.GetCells(selectedBuilding);
        else highlighted[0] = closest;
        
        Map.HighlightState state = selectedBuilding && !selectedBuilding.indestructable ? Map.HighlightState.Valid : Map.HighlightState.Invalid;
        Manager.Map.Highlight(highlighted, state);
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
        CursorSelect.Cursor.Select(CursorType.Destroy);
    }

    public void ExitClearMode()
    {
        Manager.Map.Highlight(highlighted, Map.HighlightState.Inactive);
        highlighted = new Cell[1];
        icon.sprite = deselected;
        if (CursorSelect.Cursor.currentCursor == CursorType.Destroy)
            CursorSelect.Cursor.Select(CursorType.Pointer);
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
