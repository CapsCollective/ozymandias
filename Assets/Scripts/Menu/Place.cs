using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static GameManager;

public class Place : MonoBehaviour
{
    EventSystem eventSystem;
    public Image image;
    public Map map;
    public Click selectedObject;
    public Hover hoverObject;
    public GameObject buildingInstantiated;
    private RaycastHit hit;
    private Camera cam;

    // HIGHLIGHTING
    private Cell[] highlighted = new Cell[0];

    // ROTATION
    private int rotation = 0;

    private void Awake()
    {
        cam = Camera.main;
        eventSystem = EventSystem.current;
    }

    void Update()
    {
        // Clear previous highlights
        map.Highlight(highlighted, Map.HighlightState.Inactive);
        highlighted = new Cell[0];

        if (selectedObject)
        {
            Ray ray = cam.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y, cam.nearClipPlane));
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
                rotation++;

            }

            // Check if new cells need to be highlighted
            Cell closest = map.GetCell(hit.point);
            BuildingStructure building = selectedObject.building.GetComponent<BuildingStructure>();
            Cell[] cells = map.GetCells(closest, building, rotation);

            // Check if cells are valid
            bool valid = map.IsValid(cells);

            // Highlight cells
            highlighted = cells;

            Map.HighlightState state = valid ? Map.HighlightState.Valid : Map.HighlightState.Invalid;
            map.Highlight(highlighted, state);
        }


        if (Input.GetMouseButtonDown(0))
        {
            if (selectedObject && Manager.CurrentWealth >= selectedObject.building.GetComponent<BuildingStats>().baseCost)
            {
                Ray ray = Camera.main.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane));
                Physics.Raycast(ray, out hit);
                if (hit.collider && !EventSystem.current.IsPointerOverGameObject())
                {
                    Destroy(buildingInstantiated);

                    map.CreateBuilding(selectedObject.building, hit.point, rotation);

                    selectedObject = null;
                }
            }
        }

        if (Input.GetMouseButtonDown(1)) {
            if (hoverObject && hoverObject.isHovered && !hoverObject.instantiatedHelper)
            {
                hoverObject.InfoBox();
            }
            else if (hoverObject && hoverObject.isHovered && hoverObject.instantiatedHelper)
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
