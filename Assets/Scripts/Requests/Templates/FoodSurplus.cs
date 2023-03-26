using Managers;
using Utilities;
using static Managers.GameManager;

namespace Requests.Templates
{
    public sealed class FoodSurplus : Request
    {
        public override string Description => $"Maintain a Food Surplus Above 20 for {Required} Turns";
        protected override int RequiredScaled => Tokens * 2;

        public override void Start()
        {
            State.OnNewTurn += CheckSurplus;
        }
        
        public override void Complete()
        {
            State.OnNewTurn -= CheckSurplus;
        }

        private void CheckSurplus()
        {
            if (Manager.Stats.GetStat(Stat.Food) >= 20) Completed++;
            else Completed = 0;
        }
    }
}
