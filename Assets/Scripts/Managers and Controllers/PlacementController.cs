using System.Collections;
using System.Collections.Generic;
using Managers_and_Controllers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static GameManager;

public class PlacementController : MonoBehaviour
{
    public const int Deselected = -1;
    public static int Selected = Deselected;
    public BuildingSelect[] cards;
    private List<GameObject> remainingBuildings = new List<GameObject>();
    
    private Map map;
    private RaycastHit hit;
    private Camera cam;

    private int rotation;
    private Cell[] highlighted = new Cell[0];
    private static int _previousSelected = Selected;
    public LayerMask layerMask;

    private void Awake()
    {
        cam = Camera.main;
        map = Manager.map;
        
        ClickManager.OnLeftClick += LeftClick;
        ClickManager.OnRightClick += RightClick;

        OnNextTurn += NewCards;
        
        remainingBuildings = new List<GameObject>(BuildingManager.BuildManager.AllBuildings);
        for (int i = 0; i < 3; i++) cards[i].buildingPrefab = remainingBuildings.PopRandom();
    }

    void Update()
    {
        // Clear previous highlights
        map.Highlight(highlighted, Map.HighlightState.Inactive);
        highlighted = new Cell[0];

        if (_previousSelected != Selected)
        {
            if (CursorController.Instance.currentCursor != CursorController.CursorType.Destroy)
            {
                var cursor = (Selected != Deselected)
                    ? CursorController.CursorType.Build
                    : CursorController.CursorType.Pointer;
                CursorController.Instance.SwitchCursor(cursor);
                _previousSelected = Selected;
            }
        }

        if (Selected == Deselected || EventSystem.current.IsPointerOverGameObject()) return;

        Cell closest = map.GetCellFromMouse();
        
        BuildingStructure building = cards[Selected].buildingPrefab.GetComponent<BuildingStructure>();
        
        highlighted = map.GetCells(closest, building, rotation);

        Map.HighlightState state = map.IsValid(highlighted) ? Map.HighlightState.Valid : Map.HighlightState.Invalid;
        map.Highlight(highlighted, state);
    }
    
    private void LeftClick()
    {
        if (Selected == Deselected) return;
        Ray ray = cam.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y, cam.nearClipPlane));
        Physics.Raycast(ray, out hit, 200f,layerMask);
        
        if (!hit.collider || EventSystem.current.IsPointerOverGameObject()) return; // No placing through ui
        int i = Selected;
        if (map.CreateBuilding(cards[i].buildingPrefab, hit.point, rotation))
        {
            StartCoroutine(NewCard(i));
            cards[i].toggle.isOn = false;
            Selected = Deselected;
        }
        Manager.UpdateUi();
    }
    
    private void RightClick()
    {
        if (Selected == Deselected) return;
        rotation++;
    }

    public void NewCards()
    {
        GetComponent<ToggleGroup>().SetAllTogglesOff();
        for (int i = 0; i < 3; i++) StartCoroutine(NewCard(i));
    }

    public IEnumerator NewCard(int i)
    {
        RectTransform transform = cards[i].GetComponent<RectTransform>();
        for (int j = 0; j < 15; j++)
        {
            transform.anchoredPosition -= new Vector2(0,5f);
            yield return null;
        }
        if (remainingBuildings.Count == 0) remainingBuildings = new List<GameObject>(BuildingManager.BuildManager.AllBuildings);
        bool valid = false;
        
        // Confirm no duplicate buildings
        while (!valid)
        {
            valid = true;
            cards[i].buildingPrefab = remainingBuildings.PopRandom();
            for (int j = 0; j < 3; j++)
            {
                if (i == j) continue;
                if (cards[j].buildingPrefab == cards[i].buildingPrefab) valid = false;
            }
        }
        
        Manager.UpdateUi();
        for (int j = 0; j < 15; j++)
        {
            transform.anchoredPosition += new Vector2(0,5f);
            yield return null;
        }
    }
}