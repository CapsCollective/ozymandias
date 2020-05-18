using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Drop : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData pointerEvent)
    {
        if (pointerEvent.pointerDrag == null)
            return;
        //The dragged object's placeholder's parent becomes this board (not to confused with the actual object).
        //This is to make cards react to a card being hovered over it
        Drag d = pointerEvent.pointerDrag.GetComponent<Drag>();
        if (d != null)
        {
            d.parentPlaceholder = transform;
        }
    }

    public void OnPointerExit(PointerEventData pointerEvent)
    {
        if (pointerEvent.pointerDrag == null)
            return;

        Drag d = pointerEvent.pointerDrag.GetComponent<Drag>();
        if (d != null && d.parentPlaceholder == null)
        {
            d.parentPlaceholder = d.parent;
        }
    }

    public void OnDrop(PointerEventData pointerEvent)
    {
        //When dropped, the objects parent is the board it is on
        Drag d = pointerEvent.pointerDrag.GetComponent<Drag>();
        if (d != null)
        {
            d.parent = transform;
        }
    }
}
