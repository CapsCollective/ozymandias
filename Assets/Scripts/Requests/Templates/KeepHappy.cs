using Managers;
using Utilities;
using static Managers.GameManager;

namespace Requests.Templates
{
    public sealed class KeepHappy : Request
    {
        public override string Description => $"Keep {String.Pluralise(guild.ToString())}" +
                                              $" Happy for {Required} Turns";
        protected override int RequiredScaled => 2 * Tokens;

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
            if (Manager.Stats.GetSatisfaction(guild) > 0) Completed++;
            else Completed = 0;
        }
    }
}
