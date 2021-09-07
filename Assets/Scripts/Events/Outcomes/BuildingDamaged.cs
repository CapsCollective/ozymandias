using UnityEngine;
using Utilities;
using static Managers.GameManager;

namespace Events.Outcomes
{
    [CreateAssetMenu(fileName = "Building Damaged Outcome", menuName = "Outcomes/Building Damaged")]

    public class BuildingDamaged : Outcome
    {
        public BuildingType type;

        public string buildingName;
    
        public override bool Execute()
        {
            buildingName = Manager.Structures.Remove(type);
            return buildingName != null;
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
