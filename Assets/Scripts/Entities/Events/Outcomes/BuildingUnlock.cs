using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;
using static AchievementManager;
using static BuildingManager;

[CreateAssetMenu(fileName = "Building Unlock", menuName = "Outcomes/Building Unlock")]
public class BuildingUnlock : Outcome
{
    public GameObject Building;

    [Button]
    public override bool Execute()
    {
        if (!Building)
            return false;

        if (!BuildManager.AllBuildings.Contains(Building))
        {
            BuildManager.AllBuildings.Add(Building);
            Debug.Log($"Building Unlocked: {Building}");
            Achievements.Unlock("A Helping Hand");
            if (BuildManager.AllBuildings.Count >= 14) //9 plus the 5 unlocked
                Achievements.Unlock("Modern Influences");
            return true;
        }

        Debug.LogError("Building already unlocked");
        // Add some spending instead?
        return false;
    }

    public override string Description => "<color=#007000ff>Building Type Unlocked: " + Building.name + "!</color>";
}
