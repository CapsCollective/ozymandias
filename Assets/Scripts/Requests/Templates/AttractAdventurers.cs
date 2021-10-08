﻿using Adventurers;

namespace Requests.Templates
{
    public sealed class AttractAdventurers : Request
    {
        public override string Description => $"Attract {Required} new {guild}s";
        protected override int RequiredScaled => 4 * Tokens;

        public override void Start()
        {
            Adventurers.Adventurers.OnAdventurerJoin += CheckJoin;
        }
        
        public override void Complete()
        {
            Adventurers.Adventurers.OnAdventurerJoin -= CheckJoin;
        }

        private void CheckJoin(Adventurer adventurer)
        {
            if (adventurer.guild == guild) Completed++;
        }
    }
}
