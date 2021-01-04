using System;
using System.Collections.Generic;
using System.Linq;
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
    internal enum BadgeType
    {
        Brawler = 0,
        Outrider = 1,
        Performer = 2,
        Diviner = 3,
        Arcanist = 4,
        Money = 5,
        Housing = 6,
        Food = 7,
        Defence = 8,
    }
    
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
        [SerializeField] private TextMeshProUGUI cost;
        [SerializeField] private Image costIconTexture;
        
        [SerializeField] private Image cardBack;
        [SerializeField] private Image cardHighlight;
        [SerializeField] private Ease tweenEase;
        
        [SerializeField] private Image[] classBadges;
        [SerializeField] private Image[] classBadgeIcons;
        [SerializeField] private Image[] chevronIcons;
        [SerializeField] private Sprite brawlerIcon;
        [SerializeField] private Sprite outriderIcon;
        [SerializeField] private Sprite performerIcon;
        [SerializeField] private Sprite divinerIcon;
        [SerializeField] private Sprite arcanistIcon;
        [SerializeField] private Sprite moneyIcon;
        [SerializeField] private Sprite housingIcon;
        [SerializeField] private Sprite foodIcon;
        [SerializeField] private Sprite defenceIcon;
        [SerializeField] private Sprite chevron1;
        [SerializeField] private Sprite chevron2;
        [SerializeField] private Sprite chevron3;
        

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
            Building building = buildingPrefab.GetComponent<Building>();

            // Set card details
            title.text = building.name;
            description.text = building.description;
            cost.text = building.ScaledCost.ToString();
            icon.sprite = building.icon;
            
            // Set toggle interactable
            toggle.interactable = building.ScaledCost <= Manager.Wealth;
            if (toggle.isOn && !toggle.interactable)
            {
                // Unselect the card if un-interactable
                toggle.isOn = false;
                BuildingPlacement.Selected = Deselected;
                cardHighlight.color = new Color(1, 1, 1, 0);
            }
            
            if (toggle.interactable)
            {
                // Brighten the card if selectable
                cardBack.color = Color.white;
                cost.color = new Color(0.93f, 0.63f, 0.03f);
                costIconTexture.color = new Color(1f, 0.71f, 0.16f);
            }
            else
            {
                // Darken the card if unselectable
                var grey = new Color(0.8f, 0.8f, 0.8f);
                cardBack.color = grey;
                cost.color = grey;
                costIconTexture.color = grey;
            }

            // TODO link this up to game logic
            Dictionary<BadgeType, int> effects = new Dictionary<BadgeType, int>
            {
                {BadgeType.Brawler, 2}, 
                {BadgeType.Arcanist, -1}
            };

            // Set the class badges to the card
            for (var i = 0; i < classBadges.Length; i++)
            {
                if (effects.Count == 0)
                {
                    // Hide the badge and chevron if there are no more effects to display
                    classBadges[i].gameObject.SetActive(false);
                    chevronIcons[i].gameObject.SetActive(false);
                    continue;
                }

                // Get the next effect from the list and get a reference to the image
                classBadges[i].gameObject.SetActive(true);
                var effect = effects.Keys.First();

                // Get the relevant class colour and icon for effect
                Color classColor;
                Sprite classSprite;
                switch (effect)
                {
                    case BadgeType.Brawler:
                        classColor = new Color(0.92f, 0.48f, 0.48f, 1.0f);
                        classSprite = brawlerIcon;
                        break;
                    case BadgeType.Outrider:
                        classColor = new Color(0.50f, 0.88f, 0.48f, 1.0f);
                        classSprite = outriderIcon;
                        break;
                    case BadgeType.Performer:
                        classColor = new Color(0.0f, 0.95f, 1.0f, 1.0f);
                        classSprite = performerIcon;
                        break;
                    case BadgeType.Diviner:
                        classColor = new Color(1.0f, 0.70f, 0.27f, 1.0f);
                        classSprite = divinerIcon;
                        break;
                    case BadgeType.Arcanist:
                        classColor = new Color(0.84f, 0.40f, 1.0f, 1.0f);
                        classSprite = arcanistIcon;
                        break;
                    case BadgeType.Money:
                        classColor = new Color(1.0f, 0.86f, 0.0f, 1.0f);
                        classSprite = moneyIcon;
                        break;
                    case BadgeType.Housing:
                        classColor = new Color(0.98f, 0.67f, 0.45f, 1.0f);
                        classSprite = housingIcon;
                        break;
                    case BadgeType.Food:
                        classColor = Color.gray;
                        classSprite = foodIcon;
                        break;
                    case BadgeType.Defence:
                        classColor = Color.gray;
                        classSprite = defenceIcon;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                
                // Set chevron for the effect
                var value = effects[effect];
                if (value != 0)
                {
                    chevronIcons[i].gameObject.SetActive(true);
                    
                    // Get the relevant chevron icon for effect
                    Sprite chevronSprite;
                    switch (Mathf.Abs(value))
                    {
                        case 1:
                            chevronSprite = chevron1;
                            break;
                        case 2:
                            chevronSprite = chevron2;
                            break;
                        case 3:
                            chevronSprite = chevron3;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    
                    // Set the chevron values
                    chevronIcons[i].color = 
                        value > 0 ? new Color(0.37f, 0.73f, 0.19f) : new Color(0.82f, 0.17f, 0.14f);
                    chevronIcons[i].transform.localRotation = 
                        Quaternion.Euler(value > 0 ? new Vector3(0, 0, 180) : Vector3.zero);
                    chevronIcons[i].sprite = chevronSprite;
                }
                else chevronIcons[i].gameObject.SetActive(false);

                // Set the badge values
                classBadges[i].color = classColor;
                classBadgeIcons[i].sprite = classSprite;

                // Remove the effect from the list
                effects.Remove(effect);
            }
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
