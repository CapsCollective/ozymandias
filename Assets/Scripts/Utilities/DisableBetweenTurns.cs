using GameState;
using UnityEngine;
using UnityEngine.UI;

namespace Utilities
{
    public class DisableBetweenTurns : MonoBehaviour
    {
        private void Start()
        {
            var button = GetComponent<Button>();
            GameManager.OnNextTurnStart += () =>
            {
                button.interactable = false;
            };
            GameManager.OnNextTurnEnd += () => {
                button.interactable = true;
            };
        }
    }
}
