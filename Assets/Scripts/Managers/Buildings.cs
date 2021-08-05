using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Controllers;
using Entities;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Utilities;
using static Managers.GameManager;
using Event = Entities.Event;

namespace Managers
{
    public class Buildings : MonoBehaviour
    {
        [SerializeField] private GameObject rockSection, treeSection, terrainPrefab, guildHallPrefab;
        [SerializeField] private Event[] guildHallDestroyedEvents;
        [SerializeField] private Material outlineMaterial;
        
        [HideInInspector] public int placedThisTurn;

        private readonly List<Building> _buildings = new List<Building>();
        private readonly List<Building> _terrain = new List<Building>();
        private readonly List<Building> _ruins = new List<Building>();
        public readonly Dictionary<string, BuildingSection.SectionData> BuildingCache = new Dictionary<string, BuildingSection.SectionData>();

        public int Count => _buildings.Count;
        public int Ruins => _ruins.Count;
        public GameObject TreeSection => treeSection;
        public GameObject RockSection => rockSection;
        public GameObject TerrainPrefab => terrainPrefab;
        public Material OutlineMaterial => outlineMaterial;
        
        private void Awake()
        {
            GameManager.OnGameEnd += OnGameEnd;
            
            object[] buildingsText = Resources.LoadAll("SectionData/", typeof(TextAsset));
            foreach(TextAsset o in buildingsText)
            {
                BuildingCache.Add(o.name, JsonUtility.FromJson<BuildingSection.SectionData>(o.text));
            }
        }

        public int GetStat(Stat stat)
        {
            return _buildings.Sum(b => b.stats.ContainsKey(stat) ? b.stats[stat] : 0);
        }
        
        public int GetCount(BuildingType type)
        {
            return _buildings.Count(x => x.type == type);
        }
        
        public float GetClosestDistance(Vector3 position)
        {
            // Find distance of closest building to the camera
            try
            {
                return _buildings.Select(building => Vector3.Distance(position, building.transform.position)).Min();
            }
            catch
            {
                return float.MaxValue;
            }
        }

        public Building GetClosest(Vector3 position)
        {
            Building closestBuilding = null;
            float closestDistance = float.MaxValue;
            foreach (Building building in _buildings)
            {
                float distance = Vector3.Distance(building.transform.position, position);
                if (!(distance < closestDistance)) continue;
                closestBuilding = building;
                closestDistance = distance;
            }
    
            return closestBuilding;
        } 

        public Cell RandomCell => _buildings.SelectRandom().Occupied.SelectRandom();

        public bool Add(Building building, int rootId, int rotation = 0, int sectionCount = -1,  bool isRuin = false, bool animate = false)
        {
            // Quests are handled by the quest manager
            if (building.IsQuest || !building.Create(rootId, rotation, sectionCount, isRuin, animate )) return false;
            
            // Add to correct collection for querying
            if (building.IsTerrain) _terrain.Add(building);
            else if (building.IsRuin) _ruins.Add(building);
            else _buildings.Add(building);
            
            //TODO: Make this a action callback
            if(!Manager.IsLoading && ++placedThisTurn >= 5) Manager.Achievements.Unlock("I'm Saving Up!");
            if (_buildings.Count >= 30 && Clear.TerrainClearCount == 0) Manager.Achievements.Unlock("One With Nature");
            
            if(!Manager.IsLoading) Manager.UpdateUi();
            return true;
        }
        
        public void Remove(Building building)
        {
            if (building.type == BuildingType.GuildHall)
            {
                //TODO: Add an 'are you sure?' dialogue
                Manager.Achievements.Unlock("Now Why Would You Do That?");
                foreach (Event e in guildHallDestroyedEvents) Manager.EventQueue.Add(e, true);
                Manager.NextTurn();
            }

            if (building.IsTerrain) _terrain.Remove(building);
            else if (building.IsRuin) _ruins.Remove(building);
            else _buildings.Remove(building);
            
            building.Destroy();
            Manager.UpdateUi();
        }

        public string Remove(BuildingType type)
        {
            Building building = _buildings.Find(b => b.type == type);
            if (building == null) return null;
            Remove(building);
            return building.name;
        }

        private void OnGameEnd()
        {
            List<Building> dupList = new List<Building>(_buildings);
            dupList.ForEach(building =>
            {
                if (building.type != BuildingType.Farm && 
                    building.type != BuildingType.GuildHall && 
                    Random.Range(0,_ruins.Count) == 0
                ) ToRuin(building);
                else building.Destroy();
            });
            _buildings.Clear();
        }

        private void ToRuin(Building building)
        {
            _buildings.Remove(building);
            _ruins.Add(building);
            building.ToRuin();
        }

        public List<BuildingDetails> Save()
        {
            return _buildings.Concat(_ruins).Concat(_terrain).Select(x => x.Save()).ToList();
        }

        public async Task Load(List<BuildingDetails> buildings)
        {
            foreach (BuildingDetails details in buildings)
            {
                Building building = (await Addressables.InstantiateAsync(details.name, transform).Task).GetComponent<Building>();
                if (!Manager.Buildings.Add(building, details.rootId, details.rotation, details.sectionCount, details.isRuin))
                    Destroy(building.gameObject);
            }
        }

        public void SpawnGuildHall()
        {
            const int rootId = 296;
            const int rotation = 1;

            Building building = Instantiate(guildHallPrefab, transform).GetComponent<Building>();
            if (!Manager.Buildings.Add(building, rootId, rotation, animate: true)) Destroy(building.gameObject);
        }
    }
}
