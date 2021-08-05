using Managers;
using UnityEngine;

namespace Controllers
{
    public class ShowAfterTurn : MonoBehaviour
    {
        private void Start()
        {
            gameObject.SetActive(false);
            GameManager.OnNewTurn += ShowGameObject;

        }

        private void ShowGameObject()
        {
            gameObject.SetActive(true);
            GameManager.OnNewTurn -= ShowGameObject;
        }
    }
}
