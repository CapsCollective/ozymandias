using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Click : MonoBehaviour, IPointerClickHandler, IPointerUpHandler
{
    public GameObject building;
    private EventSystem eventSystem;
    private Place place;

    private void Start()
    {
        place = FindObjectOfType<Place>();
    }

    public void OnEnable()
    {
        eventSystem = EventSystem.current;
    }

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        if (pointerEventData.button == PointerEventData.InputButton.Left)
        {
            place.selectedObject = this;
            place.NewSelection();
            eventSystem.SetSelectedGameObject(gameObject);
        }
    }

    public void OnPointerUp(PointerEventData pointerEventData)
    {
        place.selectedObject = null;
        eventSystem.SetSelectedGameObject(null);
    }
}


