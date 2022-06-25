using UnityEngine;
using UnityEngine.UI;
using static Managers.GameManager;

namespace Utilities
{
    public class UrlButton : MonoBehaviour
    {
        [SerializeField] private string url;

        private void Start()
        {
            GetComponent<Button>().onClick.AddListener(() =>
            {
                if (Manager.State.InIntro) Application.OpenURL(url);
            });
        }
    }
}
