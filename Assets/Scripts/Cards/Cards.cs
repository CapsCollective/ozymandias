using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Buildings;
using Managers;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Utilities;
using Random = UnityEngine.Random;
using static Managers.GameManager;

namespace Cards
{
    public class Cards : MonoBehaviour
    {
        public static Action<Building> OnUnlock;
        public static Action OnDiscoverRuin;
        
        [SerializeField] private List<GameObject> starterBuildings;
        [SerializeField] private List<GameObject> unlockableBuildings;
        public List<GameObject> StarterBuildings => starterBuildings;
        public List<GameObject> UnlockableBuildings => unlockableBuildings;
        
        private readonly List<GameObject> 
            _all = new List<GameObject>(), // All unlocked buildings across all playthroughs
            _current = new List<GameObject>(), // Unlocked for current run
            _discoverable = new List<GameObject>(); // Discoverable from ruins

        public List<GameObject> All => starterBuildings.Concat(_current).ToList();
        private int MaxDiscoverable => 3; // TODO: System to determine how many cards are discoverable

        private void Awake()
        {
            BuildingSelect.OnClear += Discover;
            State.OnGameEnd += OnGameEnd;
        }

        public bool Unlock(GameObject building, bool isRuin = false)
        {
            if (_current.Contains(building)) return false;
            if (!_all.Contains(building)) _all.Add(building);
            _current.Add(building);
            
            OnUnlock?.Invoke(building.GetComponent<Building>());
            if (isRuin) OnDiscoverRuin?.Invoke();
            return true;
        }

        private void Discover(Building building)
        {
            // Gets more likely to discover buildings as ruins get cleared until non remain
            if (_discoverable.Count != 0 && building.IsRuin &&
                Random.Range(0, Manager.Buildings.Ruins) <= _discoverable.Count
            ) Unlock(_discoverable.PopRandom(), true);
        }

        public bool IsDiscoverableOrUnlocked(GameObject building)
        {
            return _current.Contains(building) || _discoverable.Contains(building);
        }
        
        public bool IsUnlocked(GameObject building)
        {
            return _all.Contains(building);
        }
        
        public BuildingCardDetails Save()
        {
            return new BuildingCardDetails
            {
                all =  _all.Select(x => x.name).ToList(),
                current = _current.Select(x => x.name).ToList(),
                discoverable = _discoverable.Select(x => x.name).ToList()
            };
        }
        
        public async Task Load(BuildingCardDetails buildings)
        {
            foreach (string b in buildings.all)
                _all.Add(await Addressables.LoadAssetAsync<GameObject>(b).Task);
            foreach (string b in buildings.current)
                _current.Add(await Addressables.LoadAssetAsync<GameObject>(b).Task);
            foreach (string b in buildings.discoverable)
                _discoverable.Add(await Addressables.LoadAssetAsync<GameObject>(b).Task);
        }

        private void OnGameEnd()
        {
            _current.Clear();
            _discoverable.Clear();
            _discoverable.AddRange(_all.RandomSelection(Mathf.Min(MaxDiscoverable, _all.Count)));
        }
    }
}
