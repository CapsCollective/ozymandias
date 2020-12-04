using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using static GameManager;

namespace Managers
{
    public class BuildingCards : MonoBehaviour
    {
        public List<GameObject> starterBuildings;
        public List<GameObject> unlockedBuildings;

        public List<GameObject> All => starterBuildings.Concat(unlockedBuildings).ToList();

        public bool Unlock(GameObject building)
        {
            if (unlockedBuildings.Contains(building)) return false;
            unlockedBuildings.Add(building);
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
            foreach (var b in buildings)
                unlockedBuildings.Add(await Addressables.LoadAssetAsync<GameObject>(b).Task);
        }
    }
}
