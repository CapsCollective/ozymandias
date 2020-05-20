using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static GameManager;
using UnityEngine.EventSystems;

public class Clear : MonoBehaviour
{
    private bool clearMode = false;
    private RaycastHit hit;
    private Cell[] highlighted = new Cell[1];
    public Map map;
    private Image image;
    public int clearCost = 5;
    EventSystem eventSystem;

    private void Awake()
    {
        image = GetComponent<Image>();
        image.color = Color.white;
        eventSystem = EventSystem.current;
    }

    // Update is called once per frame
    void Update()
    {
        if (clearMode)
        {
            map.Highlight(highlighted, Map.HighlightState.Inactive);
            highlighted = new Cell[1];

            Ray ray = Camera.main.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane));
            Physics.Raycast(ray, out hit, LayerMask.GetMask("Surface","UI"));
            
            // Check if new cells need to be highlighted
            Cell closest = map.GetCell(hit.point);
            
            bool canClear = !map.IsValid(closest);

            highlighted[0] = closest;

            
            Map.HighlightState state = canClear ? Map.HighlightState.Valid : Map.HighlightState.Invalid;
            map.Highlight(highlighted, state);

            ////////////TODO: Find the building in the cell, get the other cells it occupies, and highlight all of those cells //////////////

            //////////////////////////////////////////////////////////////////

            if (Input.GetMouseButtonDown(0))
            {
                if (eventSystem.IsPointerOverGameObject())
                {
                    if (eventSystem.currentSelectedGameObject.gameObject != gameObject)
                    {
                        ExitClearMode();
                        return;
                    }
                    
                }

                else if (canClear)
                {
                    ClearSpace(highlighted);
                }
            }
        }
    }

    public void EnterClearMode()
    {
        if (!clearMode)
        {
            clearMode = true;
            image.color = Color.gray;
        }
        else
        {
            ExitClearMode();
        }
        
    }

    public void ExitClearMode()
    {
        clearMode = false;
        map.Highlight(highlighted, Map.HighlightState.Inactive);
        highlighted = new Cell[1];
        image.color = Color.white;
    }


    public void ClearSpace(Cell[] cellsToClear)
    {
        if (Manager.Spend(clearCost))
        {
            map.Clear(cellsToClear);
        }

        // Currently clears buildings and terrain, but needs to only clear the latter

        //ExitClearMode();
    }
}
