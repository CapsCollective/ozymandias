using Structures;

namespace Requests.Templates
{
    public sealed class DiscoverCards : Request
    {
        public override string Description => $"Discover {Required} Cards";
        protected override int RequiredScaled => 1 + Tokens;

        public override void Start()
        {
            Cards.Cards.OnUnlock += OnUnlock;
        }
        
        public override void Complete()
        {
            Cards.Cards.OnUnlock -= OnUnlock;
        }

        private void OnUnlock(Blueprint blueprint, bool fromRuin)
        {
            Completed++;
        }
    }
}
