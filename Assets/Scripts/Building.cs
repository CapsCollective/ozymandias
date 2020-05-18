using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;

public class Building : MonoBehaviour
{
    public bool operational = false;
    
    public int  
        baseCost,
        satisfaction,
        effectiveness,
        spending,
        accommodation,
        defense;

    public void Build()
    {
        transform.parent = GameObject.Find("Buildings").transform;
        Manager.Build(this);
        operational = true;
    }

}
