using Structures;
using Utilities;

namespace Requests.Templates
{
    public sealed class DestroyBuildings : Request
    {
        public BuildingType buildingType;
        public override string Description => $"Destroy {Required} {buildingType.ToString().Pluralise(Required)}";
        protected override int RequiredScaled => Tokens;

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
            if (structure.IsBuilding && structure.Blueprint.type == buildingType) Completed++;
        }
    }
}
