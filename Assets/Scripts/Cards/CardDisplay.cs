using System;
using System.Collections.Generic;
using System.Linq;
using Buildings;
using DG.Tweening;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace Cards
{
    public class CardDisplay : MonoBehaviour
    {
        private readonly Color _chevronGreen = new Color(0,0.7f,0.1f);
        private readonly Color _chevronRed = new Color(1f,0,0);
        private readonly Color _costActive = new Color(0.8f,0.6f,0.2f);
        private readonly Color _costInactive = new Color(0.85f,0.85f,0.85f);
    
        private readonly Dictionary<Stat, Color> _statColors = new Dictionary<Stat, Color>
        {
            {Stat.Brawler, new Color(0.7f, 0.3f, 0.3f)},
            {Stat.Outrider, new Color(0.25f, 0.45f, 0.2f)},
            {Stat.Performer, new Color(0.30f, 0.6f, 0.6f)},
            {Stat.Diviner, new Color(0.75f, 0.6f, 0.3f)},
            {Stat.Arcanist, new Color(0.6f, 0.3f, 0.75f)},
            {Stat.Spending, new Color(0.8f, 0.6f, 0f)},
            {Stat.Defence, new Color(0.25f, 0.35f, 1f)},
            {Stat.Food, new Color(0.5f, 0.65f, 0f)},
            {Stat.Housing, new Color(0.6f, 0.4f, 0f)}
        };
    
        [Serializable]
        private struct EffectBadge
        {
            public Image background, icon, chevron;
            public CardBadge badge;

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
        [SerializeField] private Image costIcon;
        [SerializeField] private Image cardHighlight;
    
        [SerializeField] private List<EffectBadge> badges;
        [SerializeField] private List<Sprite> chevronSizes;
        [SerializeField] private SerializedDictionary<Stat, Sprite> statIcons;
        
        private Image _cardBack;
        
        private void Awake()
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
                cost.color = _costActive;
                costIcon.color = _costActive;
            }
            else
            {
                // Darken the card if unselectable
                _cardBack.color = _costInactive;
                cost.color = _costInactive;
                costIcon.color = _costInactive;
            }

            var effects = building.stats
                .OrderByDescending(x => x.Value).ToList();
            
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
                badges[i].chevron.color = effects[i].Value > 0 ? _chevronGreen : _chevronRed;
                badges[i].chevron.transform.localRotation = 
                    Quaternion.Euler(effects[i].Value > 0 ? new Vector3(0, 0, 180) : Vector3.zero);
                badges[i].chevron.sprite = chevronSizes[Math.Abs(effects[i].Value)-1];
                // Set the badge values
                badges[i].background.color = _statColors[effects[i].Key];
                badges[i].icon.sprite = statIcons[effects[i].Key];
                
                badges[i].badge.Description = $"{(effects[i].Value > 0 ? "+" : "")}{effects[i].Value} " +
                                              $"{effects[i].Key.ToString()}{((int)effects[i].Key < 5 ? " Satisfaction" : "")}";
            }
        }
    }
}
