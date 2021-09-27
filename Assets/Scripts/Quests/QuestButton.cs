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
        private Button _button;

        private void Start()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(() =>
            {
                Read();
                OnClicked?.Invoke();
            });
            
            Quests.OnQuestAdded += (quest) => UpdateCounter(true);
            Quests.OnQuestRemoved += (quest) => UpdateCounter();
            
            UpdateCounter();
        }

        private void UpdateCounter(bool markUnread = false)
        {
            var count = Manager.Quests.Count;
            image.gameObject.SetActive(count != 0);
            text.text = count.ToString();
            if (markUnread) image.color = unreadColor;
        }

        private void Read()
        {
            image.color = readColor;
        }
    }
}
