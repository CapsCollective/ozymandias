using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;

[CreateAssetMenu(fileName = "Building Damaged Outcome", menuName = "Outcomes/Building Damaged")]

public class BuildingDamaged : Outcome
{
    public BuildingType type;

    public string buildingName;
    
    public override bool Execute()
    {
        foreach (BuildingStats building in Manager.buildings)
        {
            if(building.type == type)
            {
                buildingName = building.name;
                Manager.Demolish(building);
                return true;
            }
        }
        return false;
    }
    
    public override string Description
    {
        get
        {
            if (customDescription != "") return customDescription;
            return   "A " + buildingName + " has been destroyed";
        }
    }
}