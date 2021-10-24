﻿using Inputs;
using Structures;
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
            if (Manager.Structures.GetCount(type) == 0) return false;
            Newspaper.OnClosed += DestroyBuilding;
            return true;
        }

        private void DestroyBuilding()
        {
            Structure building = Manager.Structures.GetRandom(type);
            Manager.Camera.MoveTo(building.transform.position);
            buildingName = Manager.Structures.Remove(type); 
            Newspaper.OnClosed += DestroyBuilding;
        }

        protected override string Description => customDescription != "" ?
            $"{Colors.RedText}{customDescription}{Colors.EndText}" :
            $"{Colors.RedText}A {buildingName} has been destroyed.{Colors.EndText}";
    }
}
