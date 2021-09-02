using UnityEngine;
using Utilities;

namespace Adventurers
{
    [CreateAssetMenu(fileName = "Adventurer")][System.Serializable]
    public class PremadeAdventurer : ScriptableObject
    {
        public Guild type;
        public bool isSpecial;
    }
}
