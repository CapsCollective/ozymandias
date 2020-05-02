using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        GameManager.Instance.Build(this);
        operational = true;
    }

}
