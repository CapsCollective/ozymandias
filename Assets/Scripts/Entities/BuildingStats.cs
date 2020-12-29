using UnityEngine;
using static Managers.GameManager;

namespace Entities
{
    public class BuildingStats : MonoBehaviour
    {
        [TextArea(3,5)]
        public string description;
        public Sprite icon;
        public ScaleSpeed scaleSpeed;
        public bool operational;
        public bool terrain;

        public BuildingType type;

        public Metric primaryStat;

        private Vector3 _placementPosition;
        private int _rotation;
    
        public enum ScaleSpeed
        {
            Slow,
            Medium,
            Fast,
            Special
        }

        private float CostScale
        {
            get {
                switch (scaleSpeed)
                {
                    case ScaleSpeed.Slow: return 1.20f; 
                    case ScaleSpeed.Medium: return 1.25f;
                    case ScaleSpeed.Fast: return 1.30f;
                    case ScaleSpeed.Special: return 1.5f;
                    default: return 1;
                }
            }
        }

        public Color IconColour {
            get {
                switch (primaryStat)
                {
                    case Metric.Equipment: return new Color(1,1,0.212f);
                    case Metric.Weaponry: return new Color(1,0.45f,0.45f);
                    case Metric.Magic: return new Color(0.8f,0.44f,1);
                    case Metric.Food: return new Color(0.667f,1,0.667f);
                    case Metric.Entertainment: return new Color(0.5f,1,1);
                    case Metric.Luxuries: return new Color(1, 0.650f, 0.203f);
                    case Metric.Defense: return new Color(0.5f,0.6f,1);
                    default: return new Color(0.65f,0.35f,0f);
                }
            }
        }

        public int  
            baseCost,
            accommodation,
            equipment,
            weaponry,
            magic,
            food,
            entertainment,
            luxury,
            spending,
            defense;

        public int ScaledCost => Mathf.FloorToInt( baseCost * Mathf.Pow(CostScale, Manager.BuildingCount(type)));

        public void Build(Vector3 placementPosition, int rotation)
        {
            operational = true;
            name = name.Replace("(Clone)", "");
            _placementPosition = placementPosition;
            _rotation = rotation;
            Manager.Build(this);
        }

        public string Serialize()
        {
            return $"{name},{_placementPosition.x:n2},{_placementPosition.z:n2},{_rotation % 4}";
        }
    }
}
