using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Achievements
{
    public class AchievementDisplay : MonoBehaviour
    {
        [SerializeField] private Color lockedColor, unlockedColor;
        [SerializeField] private TextMeshProUGUI titleText, descriptionText;
        [SerializeField] private Image iconImage, unlockBadge;
        
        public void Display(AchievementConfig config, bool unlocked)
        {
            titleText.text = config.title;
            iconImage.sprite = config.icon;
            if (unlocked)
            {
                unlockBadge.color = unlockedColor;
                descriptionText.text = config.unlockedDescription;
            }
            else
            {
                unlockBadge.color = lockedColor;
                descriptionText.text = config.lockedDescription != "" ? config.lockedDescription : config.unlockedDescription;
            }
        }
    }
}
