using System;
using UnityEngine;
using UnityEngine.UI;
using static Managers.GameManager;
namespace UI
{
    public class BookButton : MonoBehaviour
    {
        public static Action<bool> OnClicked;

        private static Button _button;
        public GameObject notification;

        private void Start()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(() =>
            {
                OnClicked?.Invoke(notification.activeSelf);
                notification.SetActive(false);
            });
            
            // Controller and keyboard input case
            Book.OnOpened += () => notification.SetActive(false);

            Requests.Requests.OnRequestCompleted += guild =>
            {
                if (Manager.Upgrades.GuildTokens[guild] >= 2)
                {
                    notification.SetActive(true);
                }
            };
        }
    }
}
