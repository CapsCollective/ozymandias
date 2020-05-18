using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Place : MonoBehaviour
{
    EventSystem eventSystem;
    public GameManager gameManager;
    public Image image;
    public Map map;
    public Click selectedObject;
    public Hover hoverObject;
    private GameObject buildingInstantiated;
    private RaycastHit hit;

    // HIGHLIGHTING
    private Cell[] highlighted = new Cell[0];

    private void Start()
    {
        eventSystem = EventSystem.current;
    }

    void Update()
    {
        // Clear previous highlights
        map.Highlight(highlighted, Map.HighlightState.Inactive);
        highlighted = new Cell[0];

        if (selectedObject)
        {
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane));
            Physics.Raycast(ray, out hit);
            if (!buildingInstantiated)
            {
                if (hit.collider)
                {
                    buildingInstantiated = Instantiate(selectedObject.building, hit.point, transform.rotation);
                }
            }
            else
            {
                buildingInstantiated.transform.position = hit.point;
            }

            if (Input.GetMouseButtonDown(1))
            {
                buildingInstantiated.transform.Rotate(0,30,0);
            }

            // Check if new cells need to be highlighted
            Cell closest = map.GetClosest(hit.point);
            BuildingPlacement.Building building = selectedObject.building.GetComponent<BuildingPlacement.Building>();
            Cell[] cells = map.GetCells(closest, building);

            bool valid = building.sections.Count == cells.Length;
            for (int i = 0; valid && i < cells.Length; i++)
                valid = !cells[i].Occupied;

            // Highlight cells
            highlighted = cells;

            Map.HighlightState state = valid ? Map.HighlightState.Valid : Map.HighlightState.Invalid;
            map.Highlight(highlighted, state);
        }


        if (Input.GetMouseButtonDown(0))
        {
            if (selectedObject)
            {
                if (gameManager.CurrentWealth >= selectedObject.building.GetComponent<Building>().baseCost)
                {
                    Ray ray = Camera.main.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane));
                    Physics.Raycast(ray, out hit);
                    if (hit.collider)
                    {
                        gameManager.Build(selectedObject.building.GetComponent<Building>());
                        Destroy(buildingInstantiated);
                        map.Occupy(selectedObject.building, hit.point);
                        selectedObject = null;
                    }
                }
            }
        }

        if (Input.GetMouseButtonDown(1)) {
            if (hoverObject.isHovered && !hoverObject.instantiatedHelper)
            {
                hoverObject.InfoBox();
            }
            else if (hoverObject.isHovered && hoverObject.instantiatedHelper)
            {
                Destroy(hoverObject.instantiatedHelper);
            }
        }
    }

    public void NewSelection()
    {
        Destroy(buildingInstantiated);
    }
}
