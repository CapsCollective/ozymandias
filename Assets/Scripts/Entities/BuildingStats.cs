using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;

public class BuildingStats : MonoBehaviour
{
    [TextArea(3,5)]
    public string description;
    public float costScale = 1.15f; // Set for each building based on how many you're expecting player to buy
    public bool operational = false;
    public bool terrain;

    public BuildingType type;

    
    
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
