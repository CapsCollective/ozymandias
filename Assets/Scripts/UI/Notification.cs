using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace UI
{
    public class Notification : MonoBehaviour
    {
        public static Action<string, Sprite, float> OnNotification;

        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private Image icon;
        [SerializeField] private CanvasGroup canvasGroup;
        
        private void Awake()
        {
            OnNotification += (description, sprite, delay) =>
            {
                text.text = description;
                icon.sprite = sprite;
                canvasGroup.alpha = 1;
                canvasGroup.DOFade(0, 2f).SetDelay(delay);
            };
        }
    }
}
