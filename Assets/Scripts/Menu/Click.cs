using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static GameManager;

public class Click : MonoBehaviour, IPointerClickHandler, IPointerUpHandler
{
    public GameObject building;
    private EventSystem eventSystem;
    private Place place;
    private Button button;
    private Image image;

    private void Awake()
    {
        place = FindObjectOfType<Place>();
        image = GetComponent<Image>();
        button = GetComponent<Button>();
        eventSystem = EventSystem.current;

    }

    public void Update()
    {
        //TODO: Move into a ui updater class
        if (building.GetComponent<BuildingStats>().ScaledCost > Manager.Wealth)
        {
            image.color = button.colors.disabledColor;
            GetComponent<CanvasGroup>().blocksRaycasts = false;
        }
        else
        {
            image.color = button.colors.normalColor;
            GetComponent<CanvasGroup>().blocksRaycasts = true;
        }
    }

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        if (pointerEventData.button == PointerEventData.InputButton.Left)
        {
            place.selectedObject = this;
            eventSystem.SetSelectedGameObject(gameObject);
        }
    }
    
    public void OnPointerUp(PointerEventData pointerEventData)
    {
        place.Deselect();
        eventSystem.SetSelectedGameObject(null);
    }
}


