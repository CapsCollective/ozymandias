using UnityEngine;
using UnityEngine.EventSystems;

public class HighlightOnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private Vector3 startScale;
    private Vector3 startPos;

    private void Start()
    {
        startScale = transform.localScale;
        startPos = transform.localPosition;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.localScale = startScale * 1.1f;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.localScale = startScale;
        transform.localPosition = startPos;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        transform.localScale = startScale * 4;
        transform.localPosition = Vector3.zero;
        transform.SetSiblingIndex(10);
    }
}