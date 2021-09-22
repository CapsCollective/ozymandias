using Structures;
using Utilities;

namespace Requests.Templates
{
    public sealed class DestroyStructures : Request
    {
        public StructureType structureType;
        public override string Description => $"Destroy {Required} TODOs";
        protected override int RequiredScaled => 3;

        public override void Start()
        {
            Structures.Structures.OnCleared += CheckCleared;
        }
        
        public override void Complete()
        {
            Structures.Structures.OnCleared -= CheckCleared;
        }

        private void CheckCleared(Structure structure)
        {
            if (structure.StructureType == structureType) Completed++;
        }
    }
}
