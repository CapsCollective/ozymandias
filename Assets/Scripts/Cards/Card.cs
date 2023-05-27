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
        public CardDisplay cardDisplay;
        [SerializeField] private Ease tweenEase;
        [SerializeField] private int popupMultiplier = 60;
        [SerializeField] private int highlightMultiplier = 20;

        private Vector3 _initialPosition, _dropPosition;
        private RectTransform _rectTransform;
        private bool _isPointerOverCard;
        public bool IsReplacing { get; private set; }
        public bool Interactable { get; private set; }
        
        private void Start()
        {
            // Get the card's rect-transform details
            Toggle = GetComponent<Toggle>();
            _rectTransform = GetComponent<RectTransform>();
            _initialPosition = _rectTransform.localPosition;
            _dropPosition = _initialPosition - _rectTransform.transform.up * 260;
        }

        protected override void UpdateUi()
        {
            // Set toggle interactable
            Interactable = Blueprint && Blueprint.ScaledCost <= Manager.Stats.Wealth;
            if (Toggle.isOn && !Interactable)
            {
                // Unselect the card if un-interactable
                Toggle.isOn = false;
                cardDisplay.SetHighlight(false);
            }

            cardDisplay.UpdateDetails(Blueprint, Interactable);
        }

        public void ToggleSelect(bool isOn)
        {
            if (IsReplacing) return;

            if (Interactable) cardDisplay.SetHighlight(isOn);
            else if (isOn)
            {
                transform.DOShakeRotation(0.5f, new Vector3(0,0,2));
                cardDisplay.FlashCostRed();
                if (!Manager.Inputs.UsingController) Toggle.isOn = false;
            }

            if (isOn) Manager.Cards.SelectedCard = this;
            else if (Manager.Cards.SelectedCard == this) Manager.Cards.SelectedCard = null;
            
            // Animate the selection for the input type
            if (Manager.Inputs.UsingController ? isOn : _isPointerOverCard) AnimateSelected();
            else AnimateDeselected();
        }
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            _isPointerOverCard = true;
            AnimateSelected();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _isPointerOverCard = false;
            AnimateDeselected();
        }
        
        public void Drop()
        {
            if(_rectTransform.localPosition != _dropPosition)
                _rectTransform
                    .DOLocalMove(_dropPosition, 0.5f)
                    .SetEase(tweenEase)
                    .OnComplete(() => _rectTransform.localEulerAngles = Vector3.zero);
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
                .DOLocalMove(_dropPosition, 0.5f)
                .SetEase(tweenEase)
                .OnComplete(() =>
                {
                    _rectTransform.localEulerAngles = Vector3.zero;
                    Blueprint = Manager.Cards.NewCard();

                    UpdateUi();
                    if (!Manager.State.InGame)
                    {
                        IsReplacing = false;
                        return;
                    }
                    _rectTransform
                        .DOLocalMove(_initialPosition, 0.5f).SetEase(tweenEase)
                        .OnComplete(() => { IsReplacing = false; });
                });
        }

        public void AnimateSelected()
        {
            ApplyTween(
                _initialPosition + _rectTransform.transform.up * popupMultiplier, 
                new Vector3(1.1f, 1.1f), 0.5f);
        }
        
        private void AnimateDeselected()
        {
            if (!_rectTransform) return;
            var move = _initialPosition;
            if (Toggle.isOn && Interactable) move += _rectTransform.transform.up * highlightMultiplier;
            ApplyTween(move, Vector3.one, 0.5f);
        }

        private void ApplyTween(Vector3 localMove, Vector3 scale, float duration)
        {
            if (!_rectTransform || IsReplacing) return;
            _rectTransform.DOLocalMove(localMove, duration).SetEase(tweenEase);
            _rectTransform.DOScale(scale, duration).SetEase(tweenEase);
        }
    }
}
