using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;

public class Clear : MonoBehaviour
{
    private bool clearMode = false;
    private RaycastHit hit;
    private Cell[] highlighted = new Cell[1];
    public Map map;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (clearMode)
        {
            map.Highlight(highlighted, Map.HighlightState.Inactive);
            highlighted = new Cell[1];

            Ray ray = Camera.main.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane));
            Physics.Raycast(ray, out hit, LayerMask.GetMask("Surface"));

            // Check if new cells need to be highlighted
            Cell closest = map.GetCell(hit.point);
            
            bool canClear = !map.IsValid(closest);

            highlighted[0] = closest;

            
            Map.HighlightState state = canClear ? Map.HighlightState.Valid : Map.HighlightState.Invalid;
            map.Highlight(highlighted, state);

            ////////////TODO: Find the building in the cell, get the other cells it occupies, and highlight all of those cells //////////////

            //////////////////////////////////////////////////////////////////

            if (Input.GetMouseButtonDown(0) && canClear)
            {
                ClearSpace(highlighted);
            }
        }
    }

    public void EnterClearMode()
    {
        clearMode = !clearMode;
    }

    public void ExitClearMode()
    {
        clearMode = false;
        map.Highlight(highlighted, Map.HighlightState.Inactive);
        highlighted = new Cell[1];
    }

    public void ClearSpace(Cell[] cellsToClear)
    {
        map.Clear(cellsToClear);
        // cost money to clear (10g/cell)
        
        ExitClearMode();
    }
}
