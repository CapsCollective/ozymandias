using Structures;
using Utilities;

namespace Requests.Templates
{
    public sealed class DestroyBuildings : Request
    {
        public bool allowAny;
        public BuildingType buildingType;
        public override string Description => 
            $"Destroy {Required} {(allowAny ? "Buildings": buildingType.ToString().Pluralise(Required))}";
        protected override int RequiredScaled => (allowAny ? 2 : 1) * Tokens;

        public override void Start()
        {
            Structures.Structures.OnDestroyed += CheckClear;
        }
        
        public override void Complete()
        {
            Structures.Structures.OnDestroyed -= CheckClear;
        }

        private void CheckClear(Structure structure)
        {
            if (structure.IsBuilding && (allowAny || structure.Blueprint.type == buildingType)) Completed++;
        }
    }
}
