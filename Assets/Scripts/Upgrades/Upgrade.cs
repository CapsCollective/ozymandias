using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;
using Utilities;
using static Managers.GameManager;

namespace Upgrades
{
    [Serializable]
    public class Upgrade : MonoBehaviour
    {
        public UpgradeType type;
        public string title;
        [SerializeField] [TextArea] private string description;
        [SerializeField] private string effect;
        public SerializedDictionary<Guild, int> costs;
        [SerializeField] private int maxLevel; // -1 for infinite
        [SerializeField] private bool percentage;
        [SerializeField] private int multiplier, baseEffect;

        [HorizontalLine]
        [SerializeField] private List<Upgrade> children;
        [SerializeField] private List<Image> connections;
        
        [HorizontalLine]
        [ReadOnly] public int level;
        [SerializeField] private Image background;
        [SerializeField] private Sprite halfConnection, fullConnection;
        
        public bool NoLevelCap => maxLevel != -1;
        public bool LevelMaxed => maxLevel != -1 && level >= maxLevel;
        public string Description => 
            $"{description}\n\n" +
            $"Level: {level}{(NoLevelCap ? $"/{maxLevel}" : "")}\n" +
            $"{baseEffect + level * multiplier}{(percentage ? "%" : "")}{(level < maxLevel || maxLevel == -1 ? $"→{baseEffect + (level + 1) * multiplier}{(percentage ? "%": "")}" : "")} {effect}";
        
        private void Awake()
        {
            GetComponentInChildren<Button>().onClick.AddListener(() =>
            {
                Manager.Upgrades.Select(this);
            });
            
        }

        public void Display(bool visible)
        {
            gameObject.SetActive(visible);
            children.ForEach(upgrade => upgrade.Display(level != 0));
            if (!visible) return;
            
            connections.ForEach(connection => connection.sprite = level == 0 ? halfConnection : fullConnection);
            background.color = new Color(1f, 1f, 1f, level != 0 ? 1f : 0.8f);
        }
    }
}
