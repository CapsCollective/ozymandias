using UnityEngine;
using static GameState.GameManager;

namespace Events.Outcomes
{
    [CreateAssetMenu(fileName = "Game Over Outcome", menuName = "Outcomes/Game Over")]
    public class GameOver : Outcome
    {
        public override bool Execute()
        {
            Manager.IsGameOver = true;
            return true;
        }
    }
}
