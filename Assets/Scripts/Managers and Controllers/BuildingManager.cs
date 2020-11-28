using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class BuildingManager : MonoBehaviour
{
    public List<GameObject> starterBuildings;
    public List<GameObject> unlockedBuildings;

    public List<GameObject> AllBuildings => starterBuildings.Concat(unlockedBuildings).ToList();

    private static BuildingManager _instance;

    public static BuildingManager BuildManager
    {
        get
        {
            if (!_instance)
                _instance = FindObjectsOfType<BuildingManager>()[0];
            return _instance;
        }
    }

    public async Task LoadUnlocks(List<string> buildings)
    {
        foreach (var b in buildings)
        {
            unlockedBuildings.Add(await Addressables.LoadAssetAsync<GameObject>(b).Task);
        }
    }

}
