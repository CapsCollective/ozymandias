using System;
using DG.Tweening;
using Requests.Templates;
using TMPro;
using Tooltip;
using UI;
using UnityEngine;
using UnityEngine.UI;
using Utilities;
using static Managers.GameManager;

namespace Requests
{
    public class RequestDisplay : UiUpdater
    {
        private const float FadeInDuration = 0.5f;
        private const float FadeOutDuration = 1.0f;
        private const float NotificationCloseDelay = 3f;
        
        public static Action OnNotificationClicked;
        
        [Serializable]
        private struct Display {
            public Slider slider;
            public TextMeshProUGUI description, count, tokens;
        }

        [SerializeField] private GameObject notification;
        private Display _bookDisplay;
        private Display _notificationDisplay;
        private CanvasGroup _notificationCanvasGroup;
        private int _oldCompleted;
        
        public Request Request { get; set; }

        private void Start()
        {
            _bookDisplay = new Display
            {
                slider = GetComponentInChildren<Slider>(),
                description = transform.Find("Description").GetComponent<TextMeshProUGUI>(),
                count = transform.Find("Count").GetComponent<TextMeshProUGUI>(),
                tokens = transform.Find("Tokens").GetComponent<TextMeshProUGUI>()
            };
            
            _notificationDisplay = new Display
            {
                slider = notification.GetComponentInChildren<Slider>(),
                description = notification.transform.Find("Description").GetComponent<TextMeshProUGUI>(),
                count = notification.transform.Find("Count").GetComponent<TextMeshProUGUI>(),
            };
            notification.GetComponent<Button>().onClick.AddListener(() => OnNotificationClicked?.Invoke());
            
            _notificationCanvasGroup = notification.GetComponent<CanvasGroup>();

            TooltipDisplay.OnTooltipDisplay += display =>
            {
                if (display) ShowNotification();
                else HideNotification();
            };
        }
        
        protected override void UpdateUi()
        {
            if (Manager.Upgrades.TotalPurchasable == 0 && !Manager.Upgrades.IsUnlocked(UpgradeType.Discoveries))
            {
                _bookDisplay.description.text = "Finish first game";
                _bookDisplay.count.text = "";
                _bookDisplay.tokens.text = "x1";
                _bookDisplay.slider.gameObject.SetActive(false);
            }
            else if (Request == null)
            {
                _bookDisplay.description.text = "Nothings here yet!";
                _bookDisplay.count.text = "";
                _bookDisplay.tokens.text = "";
                _bookDisplay.slider.gameObject.SetActive(false);
                _notificationDisplay.slider.value = 0;
                _oldCompleted = 0;
            }
            else
            {
                _bookDisplay.description.text = Request.Description;
                _bookDisplay.count.text = Request.Completed + "/" + Request.Required;
                _bookDisplay.tokens.text = "x" + Request.Tokens;
                _bookDisplay.slider.gameObject.SetActive(true);
                _bookDisplay.slider.value = (float)Request.Completed / Request.Required;

                if (Request.Completed == _oldCompleted) return;
                _oldCompleted = Request.Completed;
                
                ShowNotification();
                
                _notificationDisplay.slider
                    .DOValue((float)Request.Completed / Request.Required, 0.5f)
                    .SetDelay(FadeInDuration)
                    .OnComplete(() =>
                    {
                        _notificationCanvasGroup
                            .DOFade(0, FadeOutDuration)
                            .SetDelay(NotificationCloseDelay)
                            .OnComplete(() => notification.SetActive(false));
                    });
            }
        }

        private void ShowNotification()
        {
            if (Request == null) return;
            // Cancel any current tweens in case updates trigger twice in quick succession
            DOTween.Kill(_notificationDisplay);
            DOTween.Kill(_notificationCanvasGroup);
            
            notification.SetActive(true);
            _notificationDisplay.description.text = Request.Description;
            _notificationDisplay.count.text = Request.Completed + "/" + Request.Required;
            _notificationCanvasGroup.DOFade(1, FadeInDuration);
        }
        
        private void HideNotification()
        {
            _notificationCanvasGroup
                .DOFade(0, FadeOutDuration)
                .OnComplete(() => notification.SetActive(false));
        }
    }
}
