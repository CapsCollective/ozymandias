using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Entities;
using UnityEngine;
using UnityEngine.AddressableAssets;
using static Managers.GameManager;

namespace Managers
{
    public class BuildingCards : MonoBehaviour
    {
        // Public fields
        public static Action<Building> OnUnlock;
        
        public List<GameObject> starterBuildings;
        [HideInInspector] public List<GameObject> unlockedBuildings;

        public List<GameObject> All => starterBuildings.Concat(unlockedBuildings).ToList();

        public bool Unlock(GameObject building)
        {
            if (unlockedBuildings.Contains(building)) return false;
            unlockedBuildings.Add(building);
            OnUnlock?.Invoke(building.GetComponent<Building>());
            Manager.Achievements.Unlock("A Helping Hand");
            if (unlockedBuildings.Count >= 5)
                Manager.Achievements.Unlock("Modern Influences");
            return true;
        }

        public List<string> Save()
        {
            return unlockedBuildings.Select(x => x.name).ToList();
        }
        
        public async Task Load(List<string> buildings)
        {
            foreach (string b in buildings)
                unlockedBuildings.Add(await Addressables.LoadAssetAsync<GameObject>(b).Task);
        }
    }
}
