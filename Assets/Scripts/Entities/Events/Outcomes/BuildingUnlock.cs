using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;

[CreateAssetMenu(fileName = "Building Unlock", menuName = "Outcomes/Building Unlock")]
public class BuildingUnlock : Outcome
{
    public GameObject Building;

    [Button]
    public override bool Execute()
    {
        if (!Building)
            return false;

        if (!BuildingManager.BuildManager.AllBuildings.Contains(Building))
        {
            BuildingManager.BuildManager.AllBuildings.Add(Building);
            Debug.Log($"Building Unlocked: {Building}");
            return true;
        }

        Debug.LogError("Building already unlocked");
        // Add some spending instead?
        return false;
    }

    public override string Description => "<color=#007000ff>Building Type Unlocked: " + Building.name + "!</color>";
}
