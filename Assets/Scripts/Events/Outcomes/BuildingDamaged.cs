using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;

[CreateAssetMenu]
public class BuildingDamaged : Outcome
{
    public GameObject BuildingType;

    public override bool Execute()
    {
        for (int i = 0; i < Manager.buildings.Count; i++)
        {
            if (i == Manager.buildings.Count)
                return false;
            if(Manager.buildings[i].gameObject.name.Contains(BuildingType.name))
            {
                var curBuilding = Manager.buildings[i];
                Manager.buildings.RemoveAt(i);
                Destroy(curBuilding.gameObject);
                return true;
            }
        }
        return false;
    }
}