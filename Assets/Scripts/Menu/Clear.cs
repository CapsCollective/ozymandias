using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static GameManager;
using UnityEngine.EventSystems;

public class Clear : MonoBehaviour
{
    private RaycastHit hit;
    private Cell[] highlighted = new Cell[1];
    private Map map;
    private Camera cam;
    private Image image;
    private EventSystem eventSystem;    

    private bool clearMode = false;
    private bool canClear = false;
    
    public int clearCost = 5;


    private void Awake()
    {
        cam = Camera.main;
        map = Manager.map;
        eventSystem = EventSystem.current;
        
        image = GetComponent<Image>();
        image.color = Color.white;

        ClickManager.OnLeftClick += LeftClick;
    }

    // Update is called once per frame
    void Update()
    {
        canClear = false;
        if (!clearMode) return;
        
        map.Highlight(highlighted, Map.HighlightState.Inactive);
        highlighted = new Cell[1];
        
        if (Manager.CurrentWealth < clearCost)
        {
            ExitClearMode();
            return;
        }

        if (eventSystem.IsPointerOverGameObject()) return;
        
        Highlight();
    }

    //Finds cells to highlight and checks validity
    public void Highlight()
    {
        //TODO: Refactor out as is a duplicate of the script in Clear
        Ray ray = cam.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y, cam.nearClipPlane));
        Physics.Raycast(ray, out hit, LayerMask.GetMask("Surface","UI"));
        // Check if new cells need to be highlighted
        Cell closest = map.GetCell(hit.point);
        
        canClear = !map.IsValid(closest);
        highlighted[0] = closest;
        
        Map.HighlightState state = canClear ? Map.HighlightState.Valid : Map.HighlightState.Invalid;
        map.Highlight(highlighted, state);

        ////////////TODO: Find the building in the cell, get the other cells it occupies, and highlight all of those cells //////////////

        //////////////////////////////////////////////////////////////////
    }

    public void LeftClick()
    {
        if (clearMode && canClear) ClearSpace(highlighted);
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
        image.color = Color.gray;
    }

    public void ExitClearMode()
    {
        clearMode = false;
        image.color = Color.white;
        map.Highlight(highlighted, Map.HighlightState.Inactive);
        highlighted = new Cell[1];
    }
    
    public void ClearSpace(Cell[] cellsToClear)
    {
        if (Manager.Spend(clearCost)) map.Clear(cellsToClear);
        // Currently clears buildings and terrain, but needs to only clear the latter
        //ExitClearMode();
    }
}
