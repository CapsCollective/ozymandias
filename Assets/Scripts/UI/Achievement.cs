using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class Achievement : MonoBehaviour
    {
        public Color lockedColor;
        public Color unlockedColor;
        public TextMeshProUGUI titleText;
        public TextMeshProUGUI descriptionText;
        public Image iconImage;
        public Image unlockBadge;
    
        [HorizontalLine()]
        public string title;
        public Sprite icon;
        public string lockedDescription; // Only for secret achievements
        public string unlockedDescription;

        private bool _unlocked;
        public bool Unlocked
        {
            get => _unlocked;
            set
            {
                _unlocked = value;
                titleText.text = title;
                iconImage.sprite = icon;
                if (_unlocked)
                {
                    unlockBadge.color = unlockedColor;
                    descriptionText.text = unlockedDescription;
                }
                else
                {
                    unlockBadge.color = lockedColor;
                    descriptionText.text = lockedDescription != "" ? lockedDescription : unlockedDescription;
                }
            }
        }
    }
}
