using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Managers.GameManager;

namespace Quests
{
    public class QuestButton : MonoBehaviour
    {
        public static Action OnClicked;

        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private Image image;
        [SerializeField] private Color readColor, unreadColor;
        private static Button _button;

        private void Start()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(() =>
            {
                // Set the badge as read
                image.color = readColor;
                OnClicked?.Invoke();
            });

            Quests.OnQuestAdded += q => UpdateCounter(true);
            Quests.OnQuestRemoved += q => UpdateCounter();
            
            UpdateCounter();
        }

        private void UpdateCounter(bool markUnread = false)
        {
            var count = Manager.Quests.Count;
            _button.enabled = count > 0;
            image.gameObject.SetActive(count != 0);
            text.text = count.ToString();
            if (markUnread) image.color = unreadColor;
        }
    }
}
