using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;

public class BuildingStats : MonoBehaviour
{
    public bool operational = false;
    public bool terrain;
    
    public int  
        baseCost,
        satisfaction,
        effectiveness,
        spending,
        accommodation,
        defense;

    public void Build()
    {
        //transform.parent = GameObject.Find("Buildings").transform;
        operational = true;
        if(!terrain) Manager.Build(this);
    }
}
