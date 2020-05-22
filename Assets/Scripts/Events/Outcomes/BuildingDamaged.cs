using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;

[CreateAssetMenu]
public class BuildingDamaged : Outcome
{
    public BuildingType type;

    public override bool Execute()
    {
        foreach (BuildingStats building in Manager.buildings)
        {
            if(building.type == type)
            {
                Manager.Demolish(building);
                return true;
            }
        }
        return false;
    }
}