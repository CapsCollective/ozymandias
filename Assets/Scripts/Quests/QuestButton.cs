using System;
using Managers;
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
        private static Button _button;

        private void Start()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(() =>
            {
                OnClicked?.Invoke();
            });

            Quests.OnQuestAdded += q => UpdateCounter();
            Quests.OnQuestRemoved += q => UpdateCounter();
            State.OnLoadingEnd += UpdateCounter;
        }

        private void UpdateCounter()
        {
            text.text = Manager.Quests.Count.ToString();
        }
    }
}
