using Managers;
using Utilities;
using static Managers.GameManager;

namespace Requests.Templates
{
    public sealed class KeepUpset : Request
    {
        public Guild targetGuild;
        public override string Description => $"Keep {targetGuild.ToString().Pluralise()} Upset for {Required} Turns";
        protected override int RequiredScaled => Tokens * 2;

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
