using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Quests
{
    public class QuestNavButton : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private Image icon;
    
        public void UpdateDisplay(Quest quest, UnityAction callback)
        {
            button.onClick.AddListener(callback);
            icon.sprite = quest.image;
        }
    }
}
