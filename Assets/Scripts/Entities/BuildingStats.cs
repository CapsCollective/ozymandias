using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;

public class BuildingStats : MonoBehaviour
{
    public const float costScale = 1.15f;
    public bool operational = false;
    public bool terrain;

    public BuildingType type;

    public int  
        baseCost,
        satisfaction,
        effectiveness,
        spending,
        accommodation,
        defense;

    public int ScaledCost => Mathf.FloorToInt( baseCost * Mathf.Pow(costScale, Manager.BuildingCount(type)));

    public void Build()
    {
        operational = true;
        if(!terrain) Manager.Build(this);
    }
}
