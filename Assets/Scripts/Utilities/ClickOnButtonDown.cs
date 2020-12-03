using UnityEngine;
using UnityEngine.UI;
using Controllers;

namespace Utilities
{
    public class ClickOnButtonDown : MonoBehaviour
    {
        private void Awake()
        {
            var target = GetComponent<Button>();
            if (target != null)
            {
                target.onClick.AddListener(() => {
                    Jukebox.Instance.PlayClick();
                });
            }
            else
            {
                GetComponent<Toggle>().onValueChanged.AddListener((v) => {
                    Jukebox.Instance.PlayClick();
                });
            }
        }
    }
}
