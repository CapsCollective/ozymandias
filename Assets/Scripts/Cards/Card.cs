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
        public Toggle Toggle { get; private set; }
        public Blueprint Blueprint { get; set; }
        [SerializeField] private CardDisplay cardDisplay;
        [SerializeField] private Ease tweenEase;
        [SerializeField] private int popupMultiplier = 60;
        [SerializeField] private int highlightMultiplier = 20;

        private Vector3 _initialPosition;
        private RectTransform _rectTransform;
        public bool IsReplacing { get; private set; }
        
        private void Start()
        {
            // Get the card's rect-transform details
            Toggle = GetComponent<Toggle>();
            _rectTransform = GetComponent<RectTransform>();
            _initialPosition = _rectTransform.localPosition;
        }

        protected override void UpdateUi()
        {
            // Set toggle interactable
            Toggle.interactable = Blueprint && Blueprint.ScaledCost <= Manager.Stats.Wealth;
            if (Toggle.isOn && !Toggle.interactable)
            {
                // Unselect the card if un-interactable
                Toggle.isOn = false;
                cardDisplay.SetHighlight(false);
            }

            cardDisplay.UpdateDetails(Blueprint, Toggle.interactable);
        }

        public void ToggleSelect(bool isOn)
        {
            cardDisplay.SetHighlight(isOn);

            if (IsReplacing) return;
            
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
            if (Toggle.isOn) move += _rectTransform.transform.up * highlightMultiplier;

            ApplyTween(move, Vector3.one, 0.5f);
        }
        
        public void Drop()
        {
            _rectTransform.localPosition = _initialPosition - _rectTransform.transform.up * 250;
        }
        
        public void Pop()
        {
            if(_rectTransform.localPosition != _initialPosition)
                _rectTransform.DOLocalMove(_initialPosition, 0.5f).SetEase(tweenEase);
        }
        
        // Animated replacement with a new card
        public void Replace()
        {
            Toggle.isOn = false;
            IsReplacing = true;
            _rectTransform
                .DOLocalMove(_initialPosition - _rectTransform.transform.up * 250, 0.5f)
                .SetEase(tweenEase)
                .OnComplete(() =>
                {
                    Blueprint = Manager.Cards.NewCard();
                    UpdateUi();
                    _rectTransform
                        .DOLocalMove(_initialPosition, 0.5f).SetEase(tweenEase)
                        .OnComplete(() => { IsReplacing = false; });
                });
        }
        
        private void ApplyTween(Vector3 localMove, Vector3 scale, float duration)
        {
            if (!_rectTransform || IsReplacing) return;
            _rectTransform.DOLocalMove(localMove, duration).SetEase(tweenEase);
            _rectTransform.DOScale(scale, duration).SetEase(tweenEase);
        }
    }
}
