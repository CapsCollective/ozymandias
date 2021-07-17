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
        [SerializeField] private GameObject guildHall;
        [SerializeField] private Event[] guildHallDestroyedEvents;
        
        [HideInInspector] public int placedThisTurn;

        private readonly List<Building> _buildings = new List<Building>();
        private readonly List<Building> _terrain = new List<Building>();
        private readonly List<Building> _ruins = new List<Building>();
        public readonly Dictionary<string, BuildingSection.SectionData> BuildingCache = new Dictionary<string, BuildingSection.SectionData>();

        public int Count => _buildings.Count;
        public int Ruins => _ruins.Count;

        private void Start()
        {
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
        
        public float GetClosestBuildingDistance(Vector3 from)
        {
            // Find distance of closest building to the camera
            try
            {
                return _buildings.Select(building => Vector3.Distance(from, building.transform.position)).Min();
            }
            catch
            {
                return float.MaxValue;
            }
        }

        public Building SelectRandom()
        {
            return _buildings.SelectRandom();
        }
        
        public void Add(Building building)
        {
            if (building.type == BuildingType.Terrain) _terrain.Add(building);
            else if (building.IsRuin) _ruins.Add(building);
            else _buildings.Add(building);

            if(!SaveFile.loading && ++placedThisTurn >= 5) Manager.Achievements.Unlock("I'm Saving Up!");
            if (_buildings.Count >= 30 && Clear.TerrainClearCount == 0) Manager.Achievements.Unlock("One With Nature");
            
            if(!SaveFile.loading) Manager.UpdateUi();
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

            Manager.Map.ClearBuilding(building);
            if (building.type == BuildingType.Terrain) _terrain.Remove(building);
            else if (building.IsRuin) _ruins.Remove(building);
            else _buildings.Remove(building);
            Manager.UpdateUi();
        }

        public string Remove(BuildingType type)
        {
            Building building = _buildings.Find(b => b.type == type);
            if (building == null) return null;
            Remove(building);
            return building.name;
        }

        public void RemoveAll()
        {
            List<Building> dupList = new List<Building>(_buildings);
            dupList.ForEach(building =>
            {
                if (building.type != BuildingType.Farm && building.type != BuildingType.GuildHall && Random.Range(0,_ruins.Count) == 0) ToRuin(building);
                else building.Clear();
            });
        }

        public void ToRuin(Building building)
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
            foreach (BuildingDetails building in buildings)
            {
                GameObject buildingInstance = await Addressables.InstantiateAsync(building.name, transform).Task;
                if (!Manager.Map.CreateBuilding(buildingInstance, building.rootId, building.rotation, building.isRuin, sectionCount: building.sectionCount))
                    Destroy(buildingInstance);
            }
        }

        public void SpawnGuildHall()
        {
            const int rootId = 296;
            const int rotation = 0;

            GameObject buildingInstance = Instantiate(guildHall, transform);
            if (!Manager.Map.CreateBuilding(buildingInstance, rootId, rotation)) Destroy(buildingInstance);
        }
    }
}
