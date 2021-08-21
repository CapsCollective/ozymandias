using Managers;
using UnityEngine;

namespace Utilities
{
    public class ShowAfterTurn : MonoBehaviour
    {
        private void Start()
        {
            gameObject.SetActive(false);
            State.OnNextTurnEnd += ShowGameObject;
        }

        private void ShowGameObject()
        {
            gameObject.SetActive(true);
            State.OnNextTurnEnd -= ShowGameObject;
        }
    }
}
