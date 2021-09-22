using Structures;
using Utilities;

namespace Requests.Templates
{
    public sealed class DestroyBuildings : Request
    {
        public BuildingType? buildingType;
        public override string Description => $"Destroy {Required} {(buildingType != null ? buildingType.ToString() : "building")}s";
        protected override int RequiredScaled => 3;

        public override void Start()
        {
            Structures.Structures.OnBuild += CheckBuilt;
        }
        
        public override void Complete()
        {
            Structures.Structures.OnBuild -= CheckBuilt;
        }

        private void CheckBuilt(Structure structure)
        {
            if (structure.Blueprint.type == buildingType) Completed++;
        }
    }
}
