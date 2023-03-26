using Managers;
using static Managers.GameManager;

namespace Requests.Templates
{
    public sealed class MaxStability : Request
    {
        public override string Description => $"Keep Stability at 100% for {Required} Turns";
        protected override int RequiredScaled => 2 * Tokens;

        public override void Start()
        {
            State.OnNewTurn += CheckStability;
        }
        
        public override void Complete()
        {
            State.OnNewTurn -= CheckStability;
        }

        private void CheckStability()
        {
            if (Manager.Stats.Stability >= 100) Completed++;
            else Completed = 0;
        }
    }
}
