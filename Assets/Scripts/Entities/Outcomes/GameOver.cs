using UnityEngine;
using static Managers.GameManager;

namespace Entities.Outcomes
{
    [CreateAssetMenu(fileName = "Game Over Outcome", menuName = "Outcomes/Game Over")]
    public class GameOver : Outcome
    {
        public override bool Execute()
        {
            Manager.GameOver();
            return true;
        }
    }
}
