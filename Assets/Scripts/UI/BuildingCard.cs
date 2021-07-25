using System;
using Controllers;
using DG.Tweening;
using Entities;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Managers.GameManager;

namespace UI
{
    public class BuildingCard : UiUpdater, IPointerEnterHandler, IPointerExitHandler
    {
        private const int Deselected = -1;
        
        public Toggle toggle;
        public GameObject buildingPrefab;
        public bool isReplacing;
        [SerializeField] private BuildingCardDisplay cardDisplay;
        [SerializeField] private int position;
        [SerializeField] private Ease tweenEase;
        [SerializeField] private int popupMultiplier = 60;
        [SerializeField] private int highlightMultiplier = 20;

        private Vector3 _initialPosition;
        private RectTransform _rectTransform;


        private void Start()
        {
            // Get the card's rect-transform details
            toggle = GetComponent<Toggle>();
            _rectTransform = GetComponent<RectTransform>();
            _initialPosition = _rectTransform.localPosition;
        }

        protected override void UpdateUi()
        {
            var building = buildingPrefab.GetComponent<Building>();
            
            // Set toggle interactable
            toggle.interactable = building.ScaledCost <= Manager.Wealth;
            if (toggle.isOn && !toggle.interactable)
            {
                // Unselect the card if un-interactable
                toggle.isOn = false;
                BuildingPlacement.Selected = Deselected;
                cardDisplay.SetHighlight(false);
            }

            cardDisplay.UpdateDetails(building, toggle.interactable);
        }

        public void ToggleSelect(bool isOn)
        {
            cardDisplay.SetHighlight(isOn);
            
            if (isReplacing) { return; }

            if (isOn) BuildingPlacement.Selected = position;
            else
            {
                BuildingPlacement.Selected = Deselected;
                OnPointerExit(null);
            }
        }

        public void SelectCard()
        {
            ApplyTween(_initialPosition + _rectTransform.transform.up * popupMultiplier,
                new Vector3(1.1f, 1.1f), 0.5f);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            // Run pointer enter tween
            SelectCard();
        }

        public void DeselectCard()
        {
            if (!_rectTransform) return;

            var move = _initialPosition;
            if (toggle.isOn) move += _rectTransform.transform.up * highlightMultiplier;

            ApplyTween(move, Vector3.one, 0.5f);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            // Run pointer exit tween
            DeselectCard();
        }

        public void SwitchCard(Action<int> callback)
        {
            isReplacing = true;
            _rectTransform.DOLocalMove(
                    _initialPosition - _rectTransform.transform.up * 100, 
                    0.5f).SetEase(tweenEase)
                .OnComplete(() => {
                    callback(position);
                    _rectTransform.DOLocalMove(_initialPosition, 0.5f).SetEase(tweenEase)
                        .OnComplete(() => {
                            isReplacing = false;
                        });
                });
        }

        private void ApplyTween(Vector3 localMove, Vector3 scale, float duration)
        {
            if (!_rectTransform) return;
            _rectTransform.DOLocalMove(localMove, duration).SetEase(tweenEase);
            _rectTransform.DOScale(scale, duration).SetEase(tweenEase);
        }
    }
}
