using Managers;
using Utilities;
using static Managers.GameManager;

namespace Requests.Templates
{
    public sealed class DiscoverCards : Request
    {
        public Guild targetGuild;
        public override string Description => $"Discover {Required} Cards";
        protected override int RequiredScaled => 1 + Tokens;

        public override void Start()
        {
            State.OnNewTurn += CheckUpset;
        }
        
        public override void Complete()
        {
            State.OnNewTurn -= CheckUpset;
        }

        private void CheckUpset()
        {
            if (Manager.Stats.GetSatisfaction(targetGuild) < 0) Completed++;
            else Completed = 0;
        }
    }
}
