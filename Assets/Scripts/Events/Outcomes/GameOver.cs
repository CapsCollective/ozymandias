using Utilities;
using static Managers.GameManager;

namespace Events.Outcomes
{
    public class GameOver : Outcome
    {
        protected override bool Execute()
        {
            Manager.State.IsGameOver = true;
            return true;
        }

        protected override string Description => 
            $"Your town attracted {Manager.Adventurers.Count} adventurers before reaching its demise." +
            $"{String.ListEnd + String.ListStart}You generated {Manager.Stats.TotalWealth} {String.StatIcon(Stat.Spending)} total in corporate profits." + 
            (Manager.Stats.BuildingsDiscovered > 0 ? 
                $"{String.ListEnd + String.ListStart}You discovered {Manager.Stats.BuildingsDiscovered} {"building".Pluralise(Manager.Stats.BuildingsDiscovered)}." : ""
            ) + (Manager.Stats.RequestsCompleted > 0 ? 
                $"{String.ListEnd + String.ListStart}You completed {Manager.Stats.RequestsCompleted} guild {"request".Pluralise(Manager.Stats.RequestsCompleted)}." : ""
            ) + (Manager.Stats.CampsCleared > 0 ? 
                $"{String.ListEnd + String.ListStart}You fended off {Manager.Stats.CampsCleared} enemy {"camp".Pluralise(Manager.Stats.CampsCleared)}." : ""
            );
    }
}
