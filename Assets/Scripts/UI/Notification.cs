using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class Notification : MonoBehaviour
    {
        private const float FadeInTime = 1f;
        private const float FadeOutTime = 2f;
        
        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private Image icon;
        [SerializeField] private CanvasGroup canvasGroup;
        
        public void Display(string description, Sprite sprite, float delay, Action onClick)
        {
            if (onClick != null) GetComponent<Button>().onClick.AddListener(onClick.Invoke);
            text.text = description;
            if (sprite) icon.sprite = sprite;
            canvasGroup.DOFade(1, FadeInTime).OnComplete(() =>
            {
                canvasGroup.DOFade(0, FadeOutTime).SetDelay(delay).OnComplete(() =>
                {
                    Destroy(gameObject);
                });
            });
        }
    }
}
