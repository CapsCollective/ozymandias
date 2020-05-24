    using UnityEngine;

    [CreateAssetMenu(fileName = "Adventurer")][System.Serializable]
    public class AdventurerDetails : ScriptableObject
    {
        public AdventurerCategory category;
        public bool isSpecial;
    }