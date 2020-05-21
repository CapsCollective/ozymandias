using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Hover : MonoBehaviour, IPointerExitHandler, IPointerEnterHandler
{
    public GameObject helperPrefab;
    private GameObject helper;
    private bool isHovered;

    private void Awake()
    {
        ClickManager.OnRightClick += RightClick;
    }
    
    public void RightClick()
    {
        if (!isHovered) return;
        if (helper) Destroy(helper);
        else InfoBox();
    }
    
    public void InfoBox()
    {
        if (!helperPrefab) return;
        helper = Instantiate(helperPrefab, transform, false);
        helper.transform.localPosition = new Vector3(0, 150, 0);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Destroy(helper);
        isHovered = false;
    }
}
