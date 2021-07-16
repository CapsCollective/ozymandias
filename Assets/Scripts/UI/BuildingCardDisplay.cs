using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Entities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace UI
{
    public class BuildingCardDisplay : MonoBehaviour
    {
    
        private readonly Dictionary<Stat, Color> _statColors = new Dictionary<Stat, Color>
        {
            {Stat.Brawler, new Color(0.92f, 0.48f, 0.48f, 1.0f)},
            {Stat.Outrider, new Color(0.50f, 0.88f, 0.48f, 1.0f)},
            {Stat.Performer, new Color(0.0f, 0.95f, 1.0f, 1.0f)},
            {Stat.Diviner, new Color(1.0f, 0.70f, 0.27f, 1.0f)},
            {Stat.Arcanist, new Color(0.84f, 0.40f, 1.0f, 1.0f)},
            {Stat.Spending, new Color(1.0f, 0.86f, 0.0f, 1.0f)},
            {Stat.Defence, new Color(0.1f, 0.5f, 0.85f, 1.0f)},
            {Stat.Food, new Color(0.20f, 1f, 0.38f, 1.0f)}, // TODO: Fix Color 
            {Stat.Housing, new Color(0.98f, 0.67f, 0.45f, 1.0f)}
        };
    
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
    
        [SerializeField] private TextMeshProUGUI title;
        [SerializeField] private TextMeshProUGUI description;
        [SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI cost;
        [SerializeField] private Image costIconTexture;
        [SerializeField] private Image cardHighlight;
    
        [SerializeField] private List<EffectBadge> badges;
        [SerializeField] private List<Sprite> chevronSizes;
        [SerializeField] private SerializedDictionary<Stat, Sprite> statIcons;
        
        private Image _cardBack;
        
        private void Start()
        {
            _cardBack = GetComponent<Image>();
        }

        public void SetHighlight(bool isOn)
        {
            // Highlight the card if selected
            cardHighlight.DOFade(isOn ? 1 : 0, 0.5f);
        }

        public void UpdateDetails(Building building, bool interactable = true)
        {
        
            // Set card details
            title.text = building.name;
            description.text = building.description;
            cost.text = building.ScaledCost.ToString();
            icon.sprite = building.icon;

            if (interactable)
            {
                // Brighten the card if selectable
                _cardBack.color = Color.white;
                cost.color = new Color(0.93f, 0.63f, 0.03f);
                costIconTexture.color = new Color(1f, 0.71f, 0.16f);
            }
            else
            {
                // Darken the card if unselectable
                var grey = new Color(0.8f, 0.8f, 0.8f);
                _cardBack.color = grey;
                cost.color = grey;
                costIconTexture.color = grey;
            }

            var effects = building.stats
                .OrderByDescending(x => x.Value).ToList();
            
            // Set the class badges to the card
            for (var i = 0; i < badges.Count; i++)
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
    }
}
