using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;

public class BuildingUnlock : Outcome
{
    public GameObject Building;

    public override bool Execute()
    {
        if (!BuildingManager.BuildManager.AllBuildings.Contains(Building))
        {
            BuildingManager.BuildManager.AllBuildings.Add(Building);
            return true;
        }

        // Add some spending instead?
        return false;
    }
}
