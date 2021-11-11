using Adventurers;
using Utilities;

namespace Requests.Templates
{
    public sealed class LoseAdventurers : Request
    {
        public bool requireKill;
        public override string Description => $"Kill {Required} {String.Pluralise("Adventurers", Required)}";
        protected override int RequiredScaled => 2 * Tokens;

        public override void Start()
        {
            Adventurers.Adventurers.OnAdventurerRemoved += CheckRemove;
        }
        
        public override void Complete()
        {
            Adventurers.Adventurers.OnAdventurerRemoved -= CheckRemove;
        }

        private void CheckRemove(Adventurer adventurer, bool kill)
        {
            if (!requireKill || kill) Completed++;
        }
    }
}
