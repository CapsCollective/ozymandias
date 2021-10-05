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
    }
}
