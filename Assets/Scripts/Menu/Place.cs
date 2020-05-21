using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static GameManager;

public class Place : MonoBehaviour
{
    private Map map;
    public Click selectedObject;
    private RaycastHit hit;
    private Camera cam;
    private EventSystem eventSystem;
    
    // HIGHLIGHTING
    private Cell[] highlighted = new Cell[0];

    private void Awake()
    {
        cam = Camera.main;
        map = Manager.map;
        eventSystem = EventSystem.current;
        
        ClickManager.OnLeftClick += LeftClick;
        ClickManager.OnRightClick += RightClick;
    }

    void Update()
    {
        // Clear previous highlights
        map.Highlight(highlighted, Map.HighlightState.Inactive);
        highlighted = new Cell[0];

        if (!selectedObject || eventSystem.IsPointerOverGameObject()) return;
        
        Highlight();
    }
    
    private bool Highlight()
    {
        Ray ray = cam.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y, cam.nearClipPlane));
        Physics.Raycast(ray, out hit);
        
        // Check if new cells need to be highlighted
        Cell closest = map.GetCell(hit.point);
        
        BuildingStructure building = selectedObject.building.GetComponent<BuildingStructure>();
        Cell[] cells = map.GetCells(closest, building);

        // Check if cells are valid
        bool valid = map.IsValid(cells);

        // Highlight cells
        highlighted = cells;

        Map.HighlightState state = valid ? Map.HighlightState.Valid : Map.HighlightState.Invalid;
        map.Highlight(highlighted, state);
        
        return valid;
    }
    
    private void LeftClick()
    {
        if (!selectedObject) return;
        if (eventSystem.currentSelectedGameObject &&
            eventSystem.currentSelectedGameObject.gameObject != selectedObject.gameObject)
        {
            Deselect();
            return;
        }
        //Don't actually know if this check is nessessary considering it limits selecting otherwise, but we'll keep it here for now
        if (Manager.CurrentWealth < selectedObject.building.GetComponent<BuildingStats>().baseCost) return;
        
        Ray ray = cam.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y, cam.nearClipPlane));
        Physics.Raycast(ray, out hit);

        if (!hit.collider || EventSystem.current.IsPointerOverGameObject()) return; // No placing through ui
        map.CreateBuilding(selectedObject.building, hit.point);
        Deselect();
    }

    public void Deselect()
    {
        selectedObject = null;
    }
    
    private void RightClick()
    {
        if (!selectedObject) return;
        //TODO: Add rotation
    }
}
