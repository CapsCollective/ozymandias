using UnityEngine;
using UnityEngine.UI;

namespace Managers_and_Controllers
{
    public class ClickOnButtonDown : MonoBehaviour
    {
        private void Awake()
        {
            var target = GetComponent<Button>();
            if (target != null)
            {
                target.onClick.AddListener(() => {
                    JukeboxController.Instance.PlayClick();
                });
            }
            else
            {
                GetComponent<Toggle>().onValueChanged.AddListener((v) => {
                    JukeboxController.Instance.PlayClick();
                });
            }
        }
    }
}