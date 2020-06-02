using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using static GameManager;

public class BuildingStats : MonoBehaviour
{
    [TextArea(3,5)]
    public string description;
    public Sprite icon;
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

    public float CostScale
    {
        get {
            switch (scaleSpeed)
            {
                case ScaleSpeed.Slow: return 1.15f; 
                case ScaleSpeed.Medium: return 1.20f;
                case ScaleSpeed.Fast: return 1.25f;
                case ScaleSpeed.Special: return 1.5f;
                default: return 1;
            }
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
        name = name.Replace("(Clone)", "");
        if(!terrain) Manager.Build(this);
    }
}
