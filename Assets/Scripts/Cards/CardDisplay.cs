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

        [SerializeField] private Sprite lockedIcon;
        
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
            if (building == null)
            {
                //TODO: Make actual locked design
                title.text = "???";
                icon.sprite = lockedIcon;
                cost.text = "?";
                description.text = "This card hasn't been unlocked yet.";
                badges.ForEach(badge => badge.SetActive(false));
                return;
            }
            
            // Set card details
            title.text = building.name;
            description.text = building.description;
            cost.text = building.ScaledCost.ToString();
            icon.sprite = building.icon;

            if (interactable)
            {
                // Brighten the card if selectable
                _cardBack.color = Color.white;
                cost.color = Colors.CostActive;
                costIcon.color = Colors.CostActive;
            }
            else
            {
                // Darken the card if unselectable
                _cardBack.color = Colors.CostInactive;
                cost.color = Colors.CostInactive;
                costIcon.color = Colors.CostInactive;
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
                badges[i].chevron.color = effects[i].Value > 0 ? Colors.Green : Colors.Red;
                badges[i].chevron.transform.localRotation = 
                    Quaternion.Euler(effects[i].Value > 0 ? new Vector3(0, 0, 180) : Vector3.zero);
                badges[i].chevron.sprite = chevronSizes[Math.Abs(effects[i].Value)-1];
                // Set the badge values
                badges[i].background.color = Colors.StatColours[effects[i].Key];
                badges[i].icon.sprite = statIcons[effects[i].Key];
                
                badges[i].badge.Description = $"{(effects[i].Value > 0 ? "+" : "")}{effects[i].Value} " +
                                              $"{effects[i].Key.ToString()}{((int)effects[i].Key < 5 ? " Satisfaction" : "")}";
            }
        }
    }
}
