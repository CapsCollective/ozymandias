#pragma warning disable 0649
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class QuestCounter : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private Image image;
        [SerializeField] private Color readColor, unreadColor;

        private void Start()
        {
            UpdateCounter(0);
        }

        public void UpdateCounter(int count, bool markUnread = false)
        {
            gameObject.SetActive(count != 0);
            text.text = count.ToString();
            if (markUnread) image.color = unreadColor;
        }

        public void Read()
        {
            image.color = readColor;
        }
    }
}
