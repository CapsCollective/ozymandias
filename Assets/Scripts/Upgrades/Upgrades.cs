using System;
using System.Collections.Generic;
using System.Linq;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities;
using static Managers.GameManager;

namespace Upgrades
{
    public class Upgrades: MonoBehaviour
    {
        public Dictionary<Guild, int> GuildTokens { get; private set; }


        [SerializeField] private Upgrade root;
        
        [Serializable]
        private struct PurchaseBox
        {
            public RectTransform transform;
            public Canvas canvas;
            public GameObject costBox;
            public TextMeshProUGUI title, description;
            public SerializedDictionary<Guild, GameObject> costs;
            public Button purchaseButton, deselectButton;
        }
        [SerializeField] private PurchaseBox purchaseBox;

        private Upgrade _selected;
        private Dictionary<UpgradeType, Upgrade> _upgrades;

        public int GetLevel(UpgradeType type) => _upgrades[type].level;
        public bool IsUnlocked(UpgradeType type) => _upgrades[type].level > 0;

        private void Awake()
        {
            State.OnEnterState += Deselect;

            purchaseBox.purchaseButton.onClick.AddListener(Purchase);
            purchaseBox.deselectButton.onClick.AddListener(Deselect);
            _upgrades = GetComponentsInChildren<Upgrade>().ToDictionary(upgrade => upgrade.type);
        }

        private void Purchase()
        {
            foreach (KeyValuePair<Guild, int> cost in _selected.costs)
            {
                GuildTokens[cost.Key] -= cost.Value;
            }

            _selected.level++;
            _selected.Display(true);
            DisplayDetails(_selected);
            UpdateUi();
        }

        public void Select(Upgrade upgrade)
        {
            if (_selected == upgrade)
            {
                Deselect();
                return;
            }

            _selected = upgrade;
            purchaseBox.transform.pivot = new Vector2(0.5f, 1.1f);
            if(upgrade.transform.localPosition.y < -80) purchaseBox.transform.pivot = new Vector2(0.5f, -0.1f);
            purchaseBox.transform.position = upgrade.transform.position;
            purchaseBox.canvas.enabled = true;
            DisplayDetails(upgrade);
        }

        private void DisplayDetails(Upgrade upgrade)
        {
            purchaseBox.title.text = upgrade.title;
            purchaseBox.description.text = upgrade.Description;

            purchaseBox.purchaseButton.gameObject.SetActive(!upgrade.LevelMaxed);
            purchaseBox.costBox.gameObject.SetActive(!upgrade.LevelMaxed);
            purchaseBox.purchaseButton.interactable = Affordable(upgrade.costs);

            foreach (Guild guild in Enum.GetValues(typeof(Guild)))
            {
                if (upgrade.costs.ContainsKey(guild))
                {
                    purchaseBox.costs[guild].SetActive(true);
                    TextMeshProUGUI text = purchaseBox.costs[guild].GetComponentInChildren<TextMeshProUGUI>();
                    text.text = upgrade.costs[guild].ToString();
                    text.color = GuildTokens[guild] >= upgrade.costs[guild] ? Colors.CardLight : Colors.Red;
                }
                else
                {
                    purchaseBox.costs[guild].SetActive(false);
                }
            }
        }

        private void Deselect()
        {
            _selected = null;
            purchaseBox.canvas.enabled = false;
        }

        private bool Affordable(Dictionary<Guild, int> costs)
        {
            return  costs.All(cost => GuildTokens[cost.Key] >= cost.Value);
        }
        
        public UpgradeDetails Save()
        {
            return new UpgradeDetails
            {
                guildTokens = GuildTokens,
                upgradeLevels = _upgrades.ToDictionary(
                    upgrade => upgrade.Key,
                    upgrade => upgrade.Value.level
                )
            };
        }
        
        public void Load(UpgradeDetails details)
        {
            GuildTokens = details.guildTokens ?? new Dictionary<Guild, int>();
            foreach (Guild guild in Enum.GetValues(typeof(Guild)))
            {
                if (!GuildTokens.ContainsKey(guild)) GuildTokens.Add(guild, 0);
            }
            
            foreach (KeyValuePair<UpgradeType,int> upgradeLevel in details.upgradeLevels ?? new Dictionary<UpgradeType, int>())
            {
                if(_upgrades.ContainsKey(upgradeLevel.Key))
                    _upgrades[upgradeLevel.Key].level = upgradeLevel.Value;
            }
            root.Display(true);
        }
    }
}
