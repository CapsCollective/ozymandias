using System.Collections.Generic;
using DG.Tweening;
using Managers;
using Structures;
using Utilities;
using static Managers.GameManager;

namespace Events.Outcomes
{
    public class BuildingDamaged : Outcome
    {
        public BuildingType buildingType;
        public bool anyBuildingType;
        public bool demolishAll;

        private Structure _toDestroy;
        private BuildingType _buildingType;
        
        protected override bool Execute()
        {
            if (!anyBuildingType && Manager.Structures.GetCount(buildingType) == 0)
            {
                UnityEngine.Debug.LogWarning($"No {(anyBuildingType ? "Building" : buildingType.ToString()).Pluralise()} to destroy");
                return false;
            }
            
            _toDestroy = anyBuildingType ? Manager.Structures.GetRandom() : Manager.Structures.GetRandom(buildingType);
            _buildingType = _toDestroy.Blueprint.type;
            Newspaper.OnNextClosed += DestroyBuilding;
            return true;
        }

        private void DestroyBuilding()
        {
            if (demolishAll)
            {
                List<Structure> buildings = Manager.Structures.GetAll(buildingType);
                Manager.Camera.MoveTo(buildings[0].transform.position)
                    .OnComplete(() =>
                    {
                        foreach (Structure building in buildings) Manager.Structures.Remove(building);
                        SaveFile.SaveState(false);
                    });
            }
            else
            {
                Manager.Camera.MoveTo(_toDestroy.transform.position)
                    .OnComplete(() =>
                    {
                        Manager.Structures.Remove(_toDestroy);
                        SaveFile.SaveState(false);
                    });
            }
        }

        protected override string Description => (
            customDescription != "" ? customDescription :
                demolishAll ? $"All {_buildingType.ToString().Pluralise()} have been destroyed." : $"A {_buildingType.ToString()} has been destroyed." 
        ).StatusColor(-1);
    }
}
