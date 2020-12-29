using UnityEngine;
using static Managers.GameManager;

namespace Entities.Outcomes
{
    [CreateAssetMenu(fileName = "Building Damaged Outcome", menuName = "Outcomes/Building Damaged")]

    public class BuildingDamaged : Outcome
    {
        public BuildingType type;

        public string buildingName;
    
        public override bool Execute()
        {
            foreach (BuildingStats building in Manager.Buildings)
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
                if (customDescription != "") return "<color=#820000ff>" + customDescription + "</color>";
                return "<color=#820000ff>A " + buildingName + " has been destroyed</color>";
            }
        }
    }
}
