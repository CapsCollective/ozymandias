using DG.Tweening;
using Structures;
using UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Managers.GameManager;

namespace Cards
{
    public class Card : UiUpdater, IPointerEnterHandler, IPointerExitHandler
    {
        private Toggle _toggle;
        public Blueprint Blueprint { get; set; }
        [SerializeField] private CardDisplay cardDisplay;
        [SerializeField] private Ease tweenEase;
        [SerializeField] private int popupMultiplier = 60;
        [SerializeField] private int highlightMultiplier = 20;

        private Vector3 _initialPosition;
        private RectTransform _rectTransform;
        private bool _isReplacing;
        
        private void Start()
        {
            // Get the card's rect-transform details
            _toggle = GetComponent<Toggle>();
            _rectTransform = GetComponent<RectTransform>();
            _initialPosition = _rectTransform.localPosition;
        }

        protected override void UpdateUi()
        {
            // Set toggle interactable
            _toggle.interactable = Blueprint && Blueprint.ScaledCost <= Manager.Stats.Wealth;
            if (_toggle.isOn && !_toggle.interactable)
            {
                // Unselect the card if un-interactable
                _toggle.isOn = false;
                cardDisplay.SetHighlight(false);
            }

            cardDisplay.UpdateDetails(Blueprint, _toggle.interactable);
        }

        public void ToggleSelect(bool isOn)
        {
            cardDisplay.SetHighlight(isOn);

            if (_isReplacing) { return; }
            if (isOn) Manager.Cards.SelectedCard = this;
            else
            {
                // Deselect if not changing to another cards
                if (Manager.Cards.SelectedCard == this) Manager.Cards.SelectedCard = null;
                OnPointerExit(null);
            }
        }
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            // Run pointer enter tween
            ApplyTween(_initialPosition + _rectTransform.transform.up * popupMultiplier, new Vector3(1.1f, 1.1f), 0.5f);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            // Run pointer exit tween
            if (!_rectTransform) return;

            var move = _initialPosition;
            if (_toggle.isOn) move += _rectTransform.transform.up * highlightMultiplier;

            ApplyTween(move, Vector3.one, 0.5f);
        }

        // Animated replacement with a new card
        public void Replace()
        {
            _toggle.isOn = false;
            _isReplacing = true;
            _rectTransform
                .DOLocalMove(_initialPosition - _rectTransform.transform.up * 250, 0.5f)
                .SetEase(tweenEase)
                .OnComplete(() => {
                    Blueprint = Manager.Cards.NewCard();
                    UpdateUi();
                    _rectTransform
                        .DOLocalMove(_initialPosition, 0.5f).SetEase(tweenEase)
                        .OnComplete(() => { _isReplacing = false; });
                });
        }

        private void ApplyTween(Vector3 localMove, Vector3 scale, float duration)
        {
            if (!_rectTransform || _isReplacing) return;
            _rectTransform.DOLocalMove(localMove, duration).SetEase(tweenEase);
            _rectTransform.DOScale(scale, duration).SetEase(tweenEase);
        }
    }
}
