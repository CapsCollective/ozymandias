using System;
using UnityEngine;
using UnityEngine.UI;
using static GameState.GameManager;

namespace Utilities
{
    public class ClickOnButtonDown : MonoBehaviour
    {
        public static Action OnUIClick;
        
        private void Awake()
        {
            Button target = GetComponent<Button>();
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
