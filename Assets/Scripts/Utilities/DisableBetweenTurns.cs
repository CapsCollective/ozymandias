using UnityEngine;
using UnityEngine.UI;

namespace Utilities
{
    public class DisableBetweenTurns : MonoBehaviour
    {
        private void Start()
        {
            var button = GetComponent<Button>();
            Managers.GameManager.OnNextTurn += () =>
            {
                button.interactable = false;
            };
            Managers.GameManager.OnNewTurn += () => {
                button.interactable = true;
            };
        }
    }
}
