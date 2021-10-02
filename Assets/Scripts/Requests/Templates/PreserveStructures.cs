﻿using Structures;
using Utilities;

namespace Requests.Templates
{
    public sealed class PreserveStructures : Request
    {
        public StructureType structureType;
        public override string Description => $"Build {Required} Buildings Without Clearing {(structureType == StructureType.Terrain ? "Forest" : "Ruins")}";
        protected override int RequiredScaled => structureType == StructureType.Terrain ? 15 : 30;

        public override void Start()
        {
            Structures.Structures.OnBuild += CheckBuilt;
            Select.OnClear += CheckCleared;
        }
        
        public override void Complete()
        {
            Structures.Structures.OnBuild -= CheckBuilt;
            Select.OnClear  -= CheckCleared;
        }

        private void CheckBuilt(Structure structure)
        {
            if (structure.StructureType == StructureType.Building) Completed++;
        }
        
        private void CheckCleared(Structure structure)
        {
            if (structure.StructureType == structureType) Completed = 0;
        }
    }
}