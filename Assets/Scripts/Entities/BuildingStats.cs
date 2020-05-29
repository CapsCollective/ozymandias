using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;

public class BuildingStats : MonoBehaviour
{
    [TextArea(3,5)]
    public string description;
    public ScaleSpeed scaleSpeed;
    private float costScale = 1.15f; // Set for each building based on how many you're expecting player to buy
    public bool operational = false;
    public bool terrain;

    public BuildingType type;

    public enum ScaleSpeed
    {
        Slow,
        Medium,
        Fast,
        Special
    }

    public void Awake()
    {
        switch (scaleSpeed)
        {
            case ScaleSpeed.Slow: costScale = 1.15f; break;
            case ScaleSpeed.Medium: costScale = 1.20f; break;
            case ScaleSpeed.Fast: costScale = 1.25f; break;
            case ScaleSpeed.Special: costScale = 1.5f; break;
        }
    }

    public int  
        baseCost,
        accommodation,
        weaponry,
        magic,
        equipment,
        food,
        entertainment,
        luxury,
        spending,
        defense;

    public int ScaledCost => Mathf.FloorToInt( baseCost * Mathf.Pow(costScale, Manager.BuildingCount(type)));

    public void Build()
    {
        operational = true;
        if(!terrain) Manager.Build(this);
    }
}
