using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class HighlightOnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public Action<GameObject> callbackMethod;
    private Vector3 startScale;
    private Vector3 startPos;
    private bool displaying;
    public bool mouseOver;

    private void Start()
    {
        startScale = transform.localScale;
        startPos = transform.localPosition;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        mouseOver = true;
        if (displaying) return;
        transform.localScale = startScale * 1.1f;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        mouseOver = false;
        if (displaying) return;
        ResetDisplay();
    }
    
    public void ResetDisplay()
    {
        displaying = false;
        gameObject.GetComponent<QuestDisplayManager>().SetDisplaying(false);
        transform.localScale = startScale;
        transform.localPosition = startPos;
    }

    public void DisplaySelected()
    {
        displaying = true;
        gameObject.GetComponent<QuestDisplayManager>().SetDisplaying(true);
        transform.localScale = startScale * 4;
        transform.localPosition = Vector3.zero;
        transform.SetSiblingIndex(10);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        callbackMethod(gameObject);
    }
}