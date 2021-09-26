using Managers;
using static Managers.GameManager;

namespace Requests.Templates
{
    public sealed class KeepHappy : Request
    {
        public override string Description => $"Keep {guild}s Happy for {Required} Turns";
        protected override int RequiredScaled => 3;

        public override void Start()
        {
            State.OnNextTurnEnd += CheckHappy;
        }
        
        public override void Complete()
        {
            State.OnNextTurnEnd -= CheckHappy;
        }

        private void CheckHappy()
        {
            if (Manager.Stats.GetSatisfaction(guild) >= 5) Completed++;
            else Completed = 0;
        }
    }
}
