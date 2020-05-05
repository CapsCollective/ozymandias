using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class BuildingDamaged : Outcome
{
    public GameObject BuildingType;

    public override bool Execute()
    {
        for (int i = 0; i < GameManager.Instance.buildings.Count; i++)
        {
            if (i == GameManager.Instance.buildings.Count)
                return false;
            if(GameManager.Instance.buildings[i].gameObject.name.Contains(BuildingType.name))
            {
                var curBuilding = GameManager.Instance.buildings[i];
                GameManager.Instance.buildings.RemoveAt(i);
                Destroy(curBuilding.gameObject);
                return true;
            }
        }
        return false;
    }
}