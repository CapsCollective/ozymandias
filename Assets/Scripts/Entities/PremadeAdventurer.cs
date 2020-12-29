using UnityEngine;

namespace Entities
{
    [CreateAssetMenu(fileName = "Adventurer")][System.Serializable]
    public class PremadeAdventurer : ScriptableObject
    {
        public AdventurerCategory category;
        public bool isSpecial;
    }
}
