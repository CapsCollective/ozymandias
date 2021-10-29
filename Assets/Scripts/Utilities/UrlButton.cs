using UnityEngine;
using UnityEngine.UI;

namespace Utilities
{
    public class UrlButton : MonoBehaviour
    {
        [SerializeField] private string url;

        private void Start()
        {
            GetComponent<Button>().onClick.AddListener(() =>
            {
                Application.OpenURL(url);
            });
        }
    }
}
