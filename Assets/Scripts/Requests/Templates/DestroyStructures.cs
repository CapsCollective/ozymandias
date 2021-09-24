using Structures;
using Utilities;

namespace Requests.Templates
{
    public sealed class DestroyStructures : Request
    {
        public StructureType structureType;
        public override string Description => $"Destroy {Required} {(structureType == StructureType.Terrain ? "Forest Spaces" : "Ruins")}";
        protected override int RequiredScaled => structureType == StructureType.Terrain ? 40 : 4;

        public override void Start()
        {
            Select.OnClear += CheckCleared;
        }
        
        public override void Complete()
        {
            Select.OnClear -= CheckCleared;
        }

        private void CheckCleared(Structure structure)
        {
            // Increase count by spaces cleared if Terrain
            if (structure.StructureType == structureType)
                Completed += structureType == StructureType.Terrain ? structure.SectionCount : 1;
        }
    }
}
