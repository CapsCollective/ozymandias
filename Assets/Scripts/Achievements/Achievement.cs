using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Achievements
{
    public class Achievement : MonoBehaviour
    {
        [SerializeField] private Color lockedColor, unlockedColor;
        [SerializeField] private TextMeshProUGUI titleText, descriptionText;
        [SerializeField] private Image iconImage, unlockBadge;
    
        [HorizontalLine]
        [SerializeField] private Sprite icon;
        [SerializeField] private string lockedDescription, unlockedDescription; // Only for secret achievements

        public string title;
        
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
