#pragma warning disable 0649
using System;
using Controllers;
using DG.Tweening;
using Entities;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Managers.GameManager;

namespace UI
{
    public class BuildingSelect : UiUpdater, IPointerEnterHandler, IPointerExitHandler
    {
        // Constants
        private const int Deselected = -1;

        // Public fields
        public Toggle toggle;
        public GameObject buildingPrefab;
        public bool isReplacing;
        
        // Serialised fields
        [SerializeField] private int position;
        [SerializeField] private BuildingSelect[] siblingCards;
        
        [SerializeField] private TextMeshProUGUI title;
        [SerializeField] private TextMeshProUGUI description;
        [SerializeField] private Image icon;
        [SerializeField] private Image iconTexture;
        [SerializeField] private TextMeshProUGUI cost;
        
        [SerializeField] private Image cardBack;
        [SerializeField] private Image cardHighlight;
        [SerializeField] private Ease tweenEase;
        

        // Private fields
        private Vector3 _initialPosition;
        private RectTransform _rectTransform;

        private void Start()
        {
            // Get the card's rect-transform details
            _rectTransform = GetComponent<RectTransform>();
            _initialPosition = _rectTransform.localPosition;
        }

        protected override void UpdateUi()
        {
            var building = buildingPrefab.GetComponent<BuildingStats>();
            var colour = building.IconColour;
            
            // Set card details
            title.text = building.name;
            title.color = colour;
            description.text = building.description;
            cost.text = building.ScaledCost.ToString();
            icon.sprite = building.icon;
            iconTexture.color = colour;
            
            // Set toggle interactable
            toggle.interactable = building.ScaledCost <= Manager.Wealth;
            if (toggle.isOn && !toggle.interactable)
            {
                // Unselect the card if un-interactable
                toggle.isOn = false;
                BuildingPlacement.Selected = Deselected;
                cardHighlight.color = new Color(1, 1, 1, 0);
            }

            // Darken the card if unselectable
            cardBack.color = toggle.interactable ? Color.white : new Color(0.8f, 0.8f, 0.8f);
        }

        public void ToggleSelect()
        {
            // Highlight the card if selected
            cardHighlight.DOFade(toggle.isOn ? 1 : 0, 0.5f);
            
            if (isReplacing) { return; }

            if (toggle.isOn)
            {
                // Deselect all other cards
                Array.ForEach(siblingCards, card => {
                    card.toggle.isOn = false;
                    card.OnPointerExit(null);
                });
                
                // Set card selection
                BuildingPlacement.Selected = position;
                OnPointerEnter(null);
            }
            else
            {
                // Deselect the building
                BuildingPlacement.Selected = Deselected;
                OnPointerExit(null);
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            // Run pointer enter tween
            if (!_rectTransform) return;
            _rectTransform.DOLocalMove(_initialPosition + _rectTransform.transform.up * 60, 0.5f)
                .SetEase(tweenEase);
            _rectTransform.DOScale(new Vector3(1.1f, 1.1f), 0.5f).SetEase(tweenEase);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            // Run pointer exit tween if not selected
            if (toggle.isOn || !_rectTransform) return;
            _rectTransform.DOLocalMove(_initialPosition, 0.5f).SetEase(tweenEase);
            _rectTransform.DOScale(Vector3.one, 0.5f).SetEase(tweenEase);
        }
    }
}
