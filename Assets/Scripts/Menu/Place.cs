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

    private int rotation;
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
        
        Cell closest = map.GetCellFromMouse();
        
        BuildingStructure building = selectedObject.building.GetComponent<BuildingStructure>();
        
        highlighted = map.GetCells(closest, building, rotation);

        Map.HighlightState state = map.IsValid(highlighted) ? Map.HighlightState.Valid : Map.HighlightState.Invalid;
        map.Highlight(highlighted, state);
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
        
        Ray ray = cam.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y, cam.nearClipPlane));
        Physics.Raycast(ray, out hit);

        if (!hit.collider || EventSystem.current.IsPointerOverGameObject()) return; // No placing through ui
        if (map.CreateBuilding(selectedObject.building, hit.point, rotation)) Deselect();
    }

    public void Deselect()
    {
        selectedObject = null;
    }
    
    private void RightClick()
    {
        if (!selectedObject) return;
        rotation++;
    }
}
