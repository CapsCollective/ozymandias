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
using Utilities;
using static Managers.GameManager;

namespace UI
{
    public class BuildingCard : UiUpdater, IPointerEnterHandler, IPointerExitHandler
    {
        private const int Deselected = -1;

        public Toggle toggle;
        public GameObject buildingPrefab;
        public bool isReplacing;
        
        [SerializeField] private int position;
        [SerializeField] private BuildingCard[] siblingCards;
        
        [SerializeField] private TextMeshProUGUI title;
        [SerializeField] private TextMeshProUGUI description;
        [SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI cost;
        
        [SerializeField] private Image cardBack;
        [SerializeField] private Image cardHighlight;
        [SerializeField] private Ease tweenEase;

        [Serializable]
        private struct EffectBadge
        {
            public Image background, icon, chevron;

            public void SetActive(bool active)
            {
                background.gameObject.SetActive(active);
                icon.gameObject.SetActive(active);
                chevron.gameObject.SetActive(active);
            }
        }

        [SerializeField] private List<EffectBadge> badges;
        [SerializeField] private List<Sprite> chevronSizes;
        [SerializeField] private GenericDictionary<Stat, Sprite> statIcons;

        private readonly Dictionary<Stat, Color> _statColors = new Dictionary<Stat, Color>
        {
            {Stat.Brawler, new Color(0.92f, 0.48f, 0.48f, 1.0f)},
            {Stat.Outrider, new Color(0.50f, 0.88f, 0.48f, 1.0f)},
            {Stat.Performer, new Color(0.0f, 0.95f, 1.0f, 1.0f)},
            {Stat.Diviner, new Color(1.0f, 0.70f, 0.27f, 1.0f)},
            {Stat.Arcanist, new Color(0.84f, 0.40f, 1.0f, 1.0f)},
            {Stat.Spending, new Color(1.0f, 0.86f, 0.0f, 1.0f)},
            {Stat.Defense, new Color(0.1f, 0.5f, 0.85f, 1.0f)},
            {Stat.Food, new Color(0.20f, 1f, 0.38f, 1.0f)}, // TODO: Fix Color 
            {Stat.Housing, new Color(0.98f, 0.67f, 0.45f, 1.0f)}
        };

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

            // Darken the card if unselectable
            cardBack.color = toggle.interactable ? Color.white : new Color(0.8f, 0.8f, 0.8f);

            List<KeyValuePair<Stat,int>> effects = building.stats.OrderByDescending(x => x.Value).ToList();

            // Set the class badges to the card
            for (int i = 0; i < badges.Count; i++)
            {
                if (i >= effects.Count)
                {
                    
                    // Hide the badge and chevron if there are no more effects to display
                    badges[i].SetActive(false);
                    continue;
                }
                badges[i].SetActive(true);
                
                // Set the chevron values
                badges[i].chevron.color = 
                    effects[i].Value > 0 ? new Color(0.37f, 0.73f, 0.19f) : new Color(0.82f, 0.17f, 0.14f);
                badges[i].chevron.transform.localRotation = 
                    Quaternion.Euler(effects[i].Value > 0 ? new Vector3(0, 0, 180) : Vector3.zero);
                badges[i].chevron.sprite = chevronSizes[Math.Abs(effects[i].Value)-1];
                // Set the badge values
                badges[i].background.color = _statColors[effects[i].Key];
                badges[i].icon.sprite = statIcons[effects[i].Key];
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
