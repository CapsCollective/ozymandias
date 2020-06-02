using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static GameManager;
/*
public class BuildingSelect : MonoBehaviour
{
    
    private Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
        toggle = GetComponent<toggle>();
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
    
    
}


*/