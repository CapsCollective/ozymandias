using Managers;
using Utilities;
using static Managers.GameManager;

namespace Requests.Templates
{
    public sealed class HousingShortage : Request
    {
        public override string Description => $"Maintain a Housing Shortage Below -20 for {Required} Turns";
        protected override int RequiredScaled => Tokens * 2;

        public override void Start()
        {
            State.OnNewTurn += CheckShortage;
        }
        
        public override void Complete()
        {
            State.OnNewTurn -= CheckShortage;
        }

        private void CheckShortage()
        {
            if (Manager.Stats.GetSatisfaction(Stat.Housing) <= -20) Completed++;
            else Completed = 0;
        }
    }
}
