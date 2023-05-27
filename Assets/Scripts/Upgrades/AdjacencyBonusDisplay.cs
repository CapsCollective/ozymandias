using System;
using UI;
using UnityEngine;
using Utilities;
using static Managers.GameManager;

namespace Upgrades
{
    [Serializable]
    public class AdjacencyBonusDisplay : UiUpdater
    {
        [SerializeField] UpgradeType type;
        
        protected override void UpdateUi()
        {
            gameObject.SetActive(Manager.Upgrades.IsUnlocked(type));
        }
    }
}
