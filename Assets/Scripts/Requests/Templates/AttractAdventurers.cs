using Adventurers;

namespace Requests.Templates
{
    public sealed class AttractAdventurers : Request
    {
        public override string Description => $"Attract {Required} {guild}s";
        protected override int RequiredScaled => 5;

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
