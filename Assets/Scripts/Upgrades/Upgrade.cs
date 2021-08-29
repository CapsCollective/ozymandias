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
        public string title;
        [TextArea] public string description;

        public int maxLevel; // -1 for infinite
        public List<Upgrade> children;
        public SerializedDictionary<Guild, int> costs;
        [SerializeField] private List<Image> connections;

        [HorizontalLine]
        [ReadOnly] public int level;
        [SerializeField] private Image background;
        [SerializeField] private Sprite halfConnection, fullConnection;
        
        private Button _button;

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
            if (!visible) return;
            
            connections.ForEach(connection => connection.sprite = level == 0 ? halfConnection : fullConnection);
            children.ForEach(upgrade => upgrade.Display(level != 0));
            background.color = new Color(1f, 1f, 1f, level != 0 ? 1f : 0.8f);
        }
    }
}
