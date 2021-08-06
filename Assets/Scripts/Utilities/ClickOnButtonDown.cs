using System;
using UnityEngine;
using UnityEngine.UI;
using static Managers.GameManager;

namespace Utilities
{
    public class ClickOnButtonDown : MonoBehaviour
    {
        // Public fields
        public static Action OnUIClick;
        
        private void Awake()
        {
            var target = GetComponent<Button>();
            if (target != null)
            {
                target.onClick.AddListener(OnClick);
            }
            else
            {
                GetComponent<Toggle>().onValueChanged.AddListener((v) =>
                {
                    OnClick();
                });
            }
        }
        
        private static void OnClick()
        {
            Manager.Jukebox.PlayClick();
            OnUIClick?.Invoke();
        }
    }
}
