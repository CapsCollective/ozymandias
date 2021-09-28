using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities;
using static Managers.GameManager;

namespace Quests
{
    public class QuestButton : MonoBehaviour
    {
        public static Action OnClicked
        {
            get => _menuButton.OnClicked;
            set => _menuButton.OnClicked = value;
        }

        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private Image image;
        [SerializeField] private Color readColor, unreadColor;
        private static MenuButton _menuButton;

        private void Start()
        {
            _menuButton = GetComponent<MenuButton>();
            OnClicked += () =>
            {
                // Set the badge as read
                image.color = readColor;
            };

            Quests.OnQuestAdded += q => UpdateCounter(true);
            Quests.OnQuestRemoved += q => UpdateCounter();
            
            UpdateCounter();
        }

        private void UpdateCounter(bool markUnread = false)
        {
            var count = Manager.Quests.Count;
            _menuButton.Interactable = count > 0;
            image.gameObject.SetActive(count != 0);
            text.text = count.ToString();
            if (markUnread) image.color = unreadColor;
        }
    }
}
