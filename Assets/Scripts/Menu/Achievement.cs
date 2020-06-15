using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

    private bool unlocked;
    public bool Unlocked
    {
        get => unlocked;
        set
        {
            unlocked = value;
            titleText.text = title;
            iconImage.sprite = icon;
            if (unlocked)
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
