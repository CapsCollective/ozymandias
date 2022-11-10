using System;
using System.Collections.Generic;
using UnityEngine;
using Utilities;
using static Managers.GameManager;

namespace Structures
{
    [Serializable]
    public class SectionInfo
    {
        public GameObject prefab;
        public List<Direction> directions;
        public int clockwiseRotations;
    }
    
    [Serializable]
    public class AdjacencyConfiguration
    {
        public bool hasBonus;
        public bool specialCheck; // For Farms, Watchtower and Monastery
        public UpgradeType upgrade;
        public Stat stat;
        public StructureType structureType = StructureType.Building;
        public BuildingType neighbourType;
        public string customDescription;
        public string Description => customDescription != "" ? customDescription : $"from adjacent {neighbourType}"; 
    }
    
    [CreateAssetMenu(fileName = "Blueprint")]
    public class Blueprint : ScriptableObject
    {
        private enum ScaleSpeed
        {
            Slow = 4,
            Medium = 3,
            Fast = 2,
            VeryFast = 1
        } // Calculated placement rate

        public BuildingType type;
        [TextArea(3, 5)] public string description;
        public Sprite icon;
        public SerializedDictionary<Stat, int> stats;
        [SerializeField] private int baseCost;
        [SerializeField] private ScaleSpeed scaleSpeed;
        
        public Color roofColor;
        public bool hasGrass = true;
        public List<SectionInfo> sections;
        public bool starter;
        
        public AdjacencyConfiguration adjacencyConfig;

        public bool Free { get; set; }
        private const float BuildingCostScale = 1.45f;
        private const float BaseRefundPercentage = 5f; // Representing 50%
        
        public int ScaledCost => Free ? 0 :
            (int)(baseCost * Mathf.Pow(BuildingCostScale, Manager.Structures.GetCount(type) * 4 / (float)scaleSpeed));
        public int Refund =>
            (int)(baseCost * Mathf.Pow(BuildingCostScale, (Manager.Structures.GetCount(type) - 1) * 4 / (float)scaleSpeed) *
                (BaseRefundPercentage + Manager.Upgrades.GetLevel(UpgradeType.Refund)) / 10f);
    }
}
