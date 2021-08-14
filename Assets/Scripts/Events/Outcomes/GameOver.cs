using UnityEngine;
using static Managers.GameManager;

namespace Events.Outcomes
{
    [CreateAssetMenu(fileName = "Game Over Outcome", menuName = "Outcomes/Game Over")]
    public class GameOver : Outcome
    {
        public override bool Execute()
        {
            Manager.State.IsGameOver = true;
            return true;
        }
    }
}
