using GameState;
using UnityEngine;

namespace Utilities
{
    public class ShowAfterTurn : MonoBehaviour
    {
        private void Start()
        {
            gameObject.SetActive(false);
            GameManager.OnNextTurnEnd += ShowGameObject;

        }

        private void ShowGameObject()
        {
            gameObject.SetActive(true);
            GameManager.OnNextTurnEnd -= ShowGameObject;
        }
    }
}
