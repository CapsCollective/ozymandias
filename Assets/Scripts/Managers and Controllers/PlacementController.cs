using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static GameManager;

public class PlacementController : MonoBehaviour
{
    public static BuildingSelect selectedObject;

    private Map map;
    private RaycastHit hit;
    private Camera cam;

    private int rotation;
    private Cell[] highlighted = new Cell[0];

    private void Awake()
    {
        cam = Camera.main;
        map = Manager.map;
        
        ClickManager.OnLeftClick += LeftClick;
        ClickManager.OnRightClick += RightClick;
    }

    void Update()
    {
        // Clear previous highlights
        map.Highlight(highlighted, Map.HighlightState.Inactive);
        highlighted = new Cell[0];

        if (!selectedObject || EventSystem.current.IsPointerOverGameObject()) return;
        
        Cell closest = map.GetCellFromMouse();
        
        BuildingStructure building = selectedObject.buildingPrefab.GetComponent<BuildingStructure>();
        
        highlighted = map.GetCells(closest, building, rotation);

        Map.HighlightState state = map.IsValid(highlighted) ? Map.HighlightState.Valid : Map.HighlightState.Invalid;
        map.Highlight(highlighted, state);
    }
    
    private void LeftClick()
    {
        if (!selectedObject) return;

        Ray ray = cam.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y, cam.nearClipPlane));
        Physics.Raycast(ray, out hit);

        if (!hit.collider || EventSystem.current.IsPointerOverGameObject()) return; // No placing through ui
        if (map.CreateBuilding(selectedObject.buildingPrefab, hit.point, rotation)) Deselect();
    }

    public void Deselect()
    {
        selectedObject.toggle.Select();
        selectedObject = null;
    }
    
    private void RightClick()
    {
        if (!selectedObject) return;
        rotation++;
    }
}