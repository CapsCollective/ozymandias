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
        //[SerializeField] private Blueprint requiredBuilding; TODO: Lock upgrades unless relevant building has been unlocked
        
        [HorizontalLine]
        [ReadOnly] public int level;
        [SerializeField] private Image background;
        [SerializeField] private Sprite halfConnection, fullConnection;
        
        public bool HasLevelCap => maxLevel != -1;
        public bool LevelMaxed => HasLevelCap && level >= maxLevel;
        public bool Unlocked => level > 0;
        private bool SingleUnlock => maxLevel == 1;
        
        public string Description => 
            $"{description}\n\n" + LevelText + EffectText;
        private string LevelText => SingleUnlock ? (level == 0 ? "Locked: " : "Unlocked: ") : $"Level: {level}{ (HasLevelCap ? $"/{maxLevel}" : "") }\n";
        private string EffectText => (SingleUnlock ? "" : $"{baseEffect + level * multiplier}{(percentage ? "%" : "")}{(level < maxLevel || maxLevel == -1 ? $"→{baseEffect + (level + 1) * multiplier}{(percentage ? "%": "")}" : "")} ") + effect;

        private void Awake()
        {
            GetComponentInChildren<Button>().onClick.AddListener(() => { Manager.Upgrades.Select(this); });
        }

        public void Display(bool visible)
        {
            gameObject.SetActive(visible);
            children.ForEach(upgrade => upgrade.Display(Unlocked));
            if (!visible) return;
            
            connections.ForEach(connection => connection.sprite = Unlocked ? fullConnection : halfConnection);
            
            if (!Unlocked) background.color = new Color(1f, 1f, 1f, 0.8f);                
            else if (LevelMaxed) background.color = Color.black;
            else background.color = Color.white;
        }
    }
}
