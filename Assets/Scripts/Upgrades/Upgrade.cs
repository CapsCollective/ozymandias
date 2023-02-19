using System;
using System.Collections.Generic;
using System.Linq;
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
        [SerializeField] private SerializedDictionary<Guild, int> costs;
        [SerializeField] private  SerializedDictionary<Guild, float> levelScale;
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
        [SerializeField] private GameObject availableIcon;
        
        public Dictionary<Guild, int> Costs => costs
            .Select(cost => new KeyValuePair<Guild, int>(cost.Key, 
                cost.Value + (levelScale.ContainsKey(cost.Key) ? Mathf.RoundToInt(level * levelScale[cost.Key]) : 0)
            ))
            .ToDictionary(x => x.Key, x => x.Value); 
        public bool HasLevelCap => maxLevel != -1;
        public bool LevelMaxed => HasLevelCap && level >= maxLevel;
        public bool Unlocked => level > 0;
        private bool SingleUnlock => maxLevel == 1;
        
        public string Description => 
            $"{description}\n\n{LevelText}{EffectText.Conditional(!SingleUnlock)} {effect}";
        private string LevelText => SingleUnlock ? 
            level == 0 ? "Locked:" : "Unlocked:" : 
            $"Level {level}{$"/{maxLevel}".Conditional(HasLevelCap)}";

        private string EffectText =>
            $"\n{baseEffect + level * multiplier}{"%".Conditional(percentage)}" +
            $" <voffset=2>→</voffset> {baseEffect + (level + 1) * multiplier}{"%".Conditional(percentage)}"
                .Conditional(level < maxLevel || maxLevel == -1);

        private void Awake()
        {
            GetComponentInChildren<Button>().onClick.AddListener(() => { Manager.Upgrades.Select(this); });
        }

        public void Display(bool visible)
        {
            gameObject.SetActive(visible);
            children.ForEach(upgrade => upgrade.Display(Unlocked));
            if (!visible) return;
            
            availableIcon.SetActive(!LevelMaxed && Manager.Upgrades.Affordable(Costs));
            
            connections.ForEach(connection => connection.sprite = Unlocked ? fullConnection : halfConnection);
            
            if (!Unlocked) background.color = new Color(1f, 1f, 1f, 0.8f);                
            else if (LevelMaxed) background.color = Color.black;
            else background.color = Color.white;
        }
    }
}
