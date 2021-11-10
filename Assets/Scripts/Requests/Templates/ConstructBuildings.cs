using Structures;
using Utilities;

namespace Requests.Templates
{
    public sealed class ConstructBuildings : Request
    {
        public bool allowAny;
        public BuildingType buildingType;
        public override string Description
        {
            get
            {
                var buildingName = allowAny ? "Building" : buildingType.ToString();
                return $"Build {Required} {String.Pluralise(buildingName)}";
            }
        }

        protected override int RequiredScaled => (allowAny ? 20 : 2) * Tokens;

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
