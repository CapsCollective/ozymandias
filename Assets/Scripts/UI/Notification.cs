using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class Notification : MonoBehaviour
    {
        public static Action<string, Sprite> OnNotification;

        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private Image icon;
        [SerializeField] private CanvasGroup canvasGroup;
        
        private void Awake()
        {
            OnNotification += (description, sprite) =>
            {
                text.text = description;
                icon.sprite = sprite;
                canvasGroup.alpha = 1;
                canvasGroup.DOFade(0, 2f);
            };
        }
    }
}
