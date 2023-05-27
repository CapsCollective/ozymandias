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

        public BuildingType type;
        [TextArea(3, 5)] public string description;
        public Sprite icon;
        public SerializedDictionary<Stat, int> stats;
        [SerializeField] private int baseCost;
        [SerializeField] private int scaleSpeed;
        
        public Color roofColor;
        public bool hasGrass = true;
        public List<SectionInfo> sections;
        public bool starter;
        
        public AdjacencyConfiguration adjacencyConfig;

        public bool Free { get; set; }
        private const float BaseRefundPercentage = 5f; // Representing 50%
        
        public int ScaledCost => Free ? 0 : baseCost + Manager.Structures.GetCount(type) * scaleSpeed;
        public int Refund => Mathf.RoundToInt(
            (baseCost + (Manager.Structures.GetCount(type) - 1) * scaleSpeed) * 
            ((BaseRefundPercentage + Manager.Upgrades.GetLevel(UpgradeType.Refund)) / 10f)
        );
    }
}
