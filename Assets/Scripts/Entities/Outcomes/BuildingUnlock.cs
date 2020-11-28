using NaughtyAttributes;
using UnityEngine;
using static AchievementManager;
using static BuildingManager;

[CreateAssetMenu(fileName = "Building Unlock", menuName = "Outcomes/Building Unlock")]
public class BuildingUnlock : Outcome
{
    public GameObject building;

    [Button]
    public override bool Execute()
    {
        if (!building)
            return false;

        if (!BuildManager.unlockedBuildings.Contains(building))
        {
            BuildManager.unlockedBuildings.Add(building);
            Debug.Log($"Building Unlocked: {building}");
            Achievements.Unlock("A Helping Hand");
            if (BuildManager.unlockedBuildings.Count >= 5)
                Achievements.Unlock("Modern Influences");
            return true;
        }

        Debug.LogError("Building already unlocked");
        // Add some spending instead?
        return false;
    }

    public override string Description => "<color=#007000ff>Building Type Unlocked: " + building.name + "!</color>";
}
