using DG.Tweening;
using Structures;
using Utilities;
using static Managers.GameManager;

namespace Events.Outcomes
{
    public class BuildingDamaged : Outcome
    {
        public BuildingType type;
        
        protected override bool Execute()
        {
            if (Manager.Structures.GetCount(type) == 0) return false;
            Newspaper.OnNextClosed += DestroyBuilding;
            return true;
        }

        private void DestroyBuilding()
        {
            Structure building = Manager.Structures.GetRandom(type);
            Manager.Camera.MoveTo(building.transform.position)
                .OnComplete(() => Manager.Structures.Remove(type));
        }

        protected override string Description => (
            customDescription != "" ? customDescription :
            $"A {type.ToString()} has been destroyed."
        ).StatusColor(-1);
    }
}
