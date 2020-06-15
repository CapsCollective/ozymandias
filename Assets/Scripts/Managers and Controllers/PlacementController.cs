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
    public List<GameObject> allBuildings = new List<GameObject>();
    private List<GameObject> remainingBuildings = new List<GameObject>();

    private Map map;
    private RaycastHit hit;
    private Camera cam;

    private int rotation;
    private Cell[] highlighted = new Cell[0];
    private static int _previousSelected = Selected;

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
            var cursor = (Selected != Deselected)
                ? CursorController.CursorType.Build
                : CursorController.CursorType.Pointer;
            CursorController.Instance.SwitchCursor(cursor);
            _previousSelected = Selected;
        }


        if (Selected == Deselected || EventSystem.current.IsPointerOverGameObject()) return;
        /*
        {
            if (rotateIconInstantiation) Destroy(rotateIconInstantiation);
            return;
        }*/

        /* Removing for now because i don't think this is the right approach
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane));
        Physics.Raycast(ray, out hit);
        if (!rotateIconInstantiation)
        {
            if (hit.collider) rotateIconInstantiation = Instantiate(rotateIcon, hit.point, transform.rotation);

        }
        else rotateIconInstantiation.transform.position = hit.point;
        */

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
        Physics.Raycast(ray, out hit);

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
        for (int j = 0; j < 30; j++)
        {
            transform.anchoredPosition -= new Vector2(0,2.5f);
            yield return null;
        }
        if (remainingBuildings.Count == 0) remainingBuildings = new List<GameObject>(BuildingManager.BuildManager.AllBuildings);
        cards[i].buildingPrefab = remainingBuildings.PopRandom();
        Manager.UpdateUi();
        for (int j = 0; j < 30; j++)
        {
            transform.anchoredPosition += new Vector2(0,2.5f);
            yield return null;
        }
    }
}