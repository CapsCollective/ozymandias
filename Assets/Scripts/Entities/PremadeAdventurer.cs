using UnityEngine;
using Utilities;

namespace Entities
{
    [CreateAssetMenu(fileName = "Adventurer")][System.Serializable]
    public class PremadeAdventurer : ScriptableObject
    {
        public AdventurerType type;
        public bool isSpecial;
    }
}
