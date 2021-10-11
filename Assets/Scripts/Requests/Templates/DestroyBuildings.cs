using Structures;
using Utilities;

namespace Requests.Templates
{
    public sealed class DestroyBuildings : Request
    {
        public BuildingType buildingType;
        public override string Description => $"Destroy {Required} {buildingType}s";
        protected override int RequiredScaled => Tokens;

        public override void Start()
        {
            Select.OnClear += CheckClear;
        }
        
        public override void Complete()
        {
            Select.OnClear -= CheckClear;
        }

        private void CheckClear(Structure structure)
        {
            if (structure.IsBuilding && structure.Blueprint.type == buildingType) Completed++;
        }
    }
}
