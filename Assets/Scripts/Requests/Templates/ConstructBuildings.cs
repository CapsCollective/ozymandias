using Structures;
using Utilities;

namespace Requests.Templates
{
    public sealed class ConstructBuildings : Request
    {
        public bool allowAny;
        public BuildingType buildingType;
        public override string Description => $"Build {Required} {(allowAny ? "Building" : buildingType.ToString())}s";
        protected override int RequiredScaled => (allowAny ? 25 : 3) * Tokens;

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
            if (allowAny || structure.Blueprint.type == buildingType) Completed++;
        }
    }
}
