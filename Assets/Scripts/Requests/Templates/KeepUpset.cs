using Managers;
using Utilities;
using static Managers.GameManager;

namespace Requests.Templates
{
    public sealed class KeepUpset : Request
    {
        public Guild targetGuild;
        public override string Description => $"Keep {targetGuild}s Upset for {Required} Turns";
        protected override int RequiredScaled => 3 * Tokens;

        public override void Start()
        {
            State.OnNextTurnEnd += CheckUpset;
        }
        
        public override void Complete()
        {
            State.OnNextTurnEnd -= CheckUpset;
        }

        private void CheckUpset()
        {
            if (Manager.Stats.GetSatisfaction(targetGuild) < 0) Completed++;
            else Completed = 0;
        }
    }
}
