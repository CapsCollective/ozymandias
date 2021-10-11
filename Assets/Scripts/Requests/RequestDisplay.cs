using System;
using DG.Tweening;
using Requests.Templates;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace Requests
{
    public class RequestDisplay : UiUpdater
    {
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
        }
        
        protected override void UpdateUi()
        {
            if (Request == null)
            {
                _bookDisplay.description.text = "Nothings here yet!";
                _bookDisplay.count.text = "";
                _bookDisplay.tokens.text = "x0";
                _bookDisplay.slider.gameObject.SetActive(false);
            }
            else
            {
                _bookDisplay.description.text = Request.Description;
                _bookDisplay.count.text = Request.Completed + "/" + Request.Required;
                _bookDisplay.tokens.text = "x" + Request.Tokens;
                _bookDisplay.slider.gameObject.SetActive(true);
                _bookDisplay.slider.value = (float)Request.Completed / Request.Required;

                if (Request.Completed == 0) {
                    _oldCompleted = 0; // Reset for new requests
                    _notificationDisplay.slider.value = 0;
                }
                
                if (Request.Completed == _oldCompleted) return;
                _oldCompleted = Request.Completed;
                
                // Cancel any current tweens in case updates trigger twice in quick succession
                DOTween.Kill(_notificationDisplay);
                DOTween.Kill(_notificationCanvasGroup);
                
                notification.SetActive(true);
                _notificationCanvasGroup.alpha = 1;

                _notificationDisplay.description.text = Request.Description;
                _notificationDisplay.count.text = Request.Completed + "/" + Request.Required;

                _notificationDisplay.slider
                    .DOValue((float)Request.Completed / Request.Required, 0.5f)
                    .OnComplete(() =>
                    {
                        _notificationCanvasGroup
                            .DOFade(0, 2f)
                            .OnComplete(() => notification.SetActive(false)).SetDelay(2f);
                    });
            }
        }
    }
}
