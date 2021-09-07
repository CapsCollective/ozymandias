using Events;
using Structures;
using UnityEngine;
using Utilities;

namespace Requests.Templates
{
    [CreateAssetMenu(fileName = "Construct Building Type", menuName = "Requests/Construct Building Type")]
    public sealed class ConstructBuildingType : Request
    {
        [SerializeField] private BuildingType buildingType;
        public override string Description => $"Build {Required} {buildingType}s";
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
        
#if (UNITY_EDITOR)

        public override void Configure(EventCreator.RequestConfig config)
        {
            buildingType = config.buildingType;
        }
#endif
    }
}
