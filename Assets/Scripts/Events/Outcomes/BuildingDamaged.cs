using Utilities;
using static Managers.GameManager;

namespace Events.Outcomes
{
    public class BuildingDamaged : Outcome
    {
        public BuildingType type;

        public string buildingName;

        protected override bool Execute()
        {
            buildingName = Manager.Structures.Remove(type);
            return buildingName != null;
        }

        protected override string Description => customDescription != "" ?
            $"{Colors.RedText}{customDescription}{Colors.EndText}" :
            $"{Colors.RedText}A {buildingName} has been destroyed.{Colors.EndText}";
    }
}
