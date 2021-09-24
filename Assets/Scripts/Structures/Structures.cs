using System;
using System.Collections.Generic;
using System.Linq;
using Managers;
using Map;
using UnityEngine;
using Utilities;
using static Managers.GameManager;
using Random = UnityEngine.Random;

namespace Structures
{
    public class Structures : MonoBehaviour
    {
        public static Action<Structure> OnBuild;

        [Serializable] private struct Location { public int root, rotation; }
        
        [SerializeField] private GameObject rockSection, treeSection, structurePrefab;
        [SerializeField] private Material outlineMaterial;
        [SerializeField] private List<Location> spawnLocations; // TODO: Pick new spawn point from random if no guild hall exists 

        private readonly List<Structure> _buildings = new List<Structure>();
        private readonly List<Structure> _terrain = new List<Structure>();
        private readonly List<Structure> _ruins = new List<Structure>();
        public readonly Dictionary<string, Section.SectionData> BuildingCache = new Dictionary<string, Section.SectionData>();

        public int Count => _buildings.Count;
        public int Ruins => _ruins.Count;
        public GameObject TreeSection => treeSection;
        public GameObject RockSection => rockSection;
        public GameObject StructurePrefab => structurePrefab;
        public Material OutlineMaterial => outlineMaterial;
        
        private Location SpawnLocation { get; set; }
        public Vector3 TownCentre { get; private set; }
        
        private void Awake()
        {
            State.OnNewGame += SpawnGuildHall;
            State.OnGameEnd += RemoveAll;
            
            object[] buildingsText = Resources.LoadAll("SectionData/", typeof(TextAsset));
            foreach(TextAsset o in buildingsText)
            {
                BuildingCache.Add(o.name, JsonUtility.FromJson<Section.SectionData>(o.text));
            }
        }

        public int GetStat(Stat stat)
        {
            return _buildings.Sum(b => (b.Stats.ContainsKey(stat) ? b.Stats[stat] : 0) + (b.Bonus == stat ? 1 : 0));
        }
        
        public int GetCount(BuildingType type)
        {
            return _buildings.Count(x => x.Blueprint.type == type);
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

        public Structure GetClosest(Vector3 position)
        {
            Structure closestBuilding = null;
            float closestDistance = float.MaxValue;
            foreach (Structure building in _buildings)
            {
                float distance = Vector3.Distance(building.transform.position, position);
                if (!(distance < closestDistance)) continue;
                closestBuilding = building;
                closestDistance = distance;
            }
    
            return closestBuilding;
        } 

        public Cell RandomCell => _buildings.SelectRandom().Occupied.SelectRandom();

        public bool AddBuilding(Blueprint blueprint, int rootId, int rotation = 0, bool isRuin = false)
        {
            Structure structure = Instantiate(structurePrefab, transform).GetComponent<Structure>();
            // Quests are handled by the quest manager
            if (!structure.CreateBuilding(blueprint, rootId, rotation, isRuin))
            {
                Destroy(structure.gameObject);
                return false;
            }
            
            // Add to correct collection for querying
            if (structure.IsRuin) _ruins.Add(structure);
            else _buildings.Add(structure);

            if (!Manager.State.Loading)
            {
                OnBuild?.Invoke(structure);
                UpdateUi();
            }
            return true;
        }

        public void AddTerrain(int rootId, int sectionCount = -1)
        {
            if (Manager.Map.GetCell(rootId).Occupied) return; 
            Structure structure = Instantiate(structurePrefab, transform).GetComponent<Structure>();
            structure.CreateTerrain(rootId, sectionCount);
            _terrain.Add(structure);
        }
        
        public void Remove(Structure structure)
        {
            if (structure.Blueprint && structure.Blueprint.type == BuildingType.GuildHall)
            {
                //TODO: Add an 'are you sure?' dialogue
                //Manager.Achievements.Unlock("Now Why Would You Do That?");
                Manager.EventQueue.AddGuildHallDestroyedEvents();
                Manager.State.EnterState(GameState.NextTurn);
            }
            
            if (structure.IsTerrain) _terrain.Remove(structure);
            else if (structure.IsRuin) _ruins.Remove(structure);
            else _buildings.Remove(structure);
            
            structure.Destroy();
            UpdateUi();
        }

        public string Remove(BuildingType type)
        {
            Structure structure = _buildings.Find(b => b.Blueprint.type == type);
            if (structure == null) return null;
            Remove(structure);
            return structure.name;
        }

        private void RemoveAll()
        {
            List<Structure> dupList = new List<Structure>(_buildings);
            dupList.ForEach(building =>
            {
                if (building.Blueprint.type != BuildingType.Farm &&
                    building.Blueprint.type != BuildingType.GuildHall &&
                    Random.Range(0,_ruins.Count) == 0
                ) ToRuin(building);
                else building.Destroy();
            });
            _buildings.Clear();
            SpawnLocation = NewSpawnLocation();
            TownCentre = Manager.Map.GetCell(SpawnLocation.root).WorldSpace;
        }

        private void ToRuin(Structure structure)
        {
            _buildings.Remove(structure);
            _ruins.Add(structure);
            structure.ToRuin();
        }

        public int NewQuestSpawn()
        {
            List<Structure> furthestBuildings = _buildings
                .OrderByDescending(building => Vector3.Distance(building.transform.position, TownCentre))
                .ToList();

            foreach (Structure building in furthestBuildings)
            {
                Vector3 position = Vector3.MoveTowards(building.transform.position, TownCentre, -Random.Range(6f, 10f));
                Cell cell = Manager.Map.GetClosestCell(position);
                if (cell != null  && cell.Active && (!cell.Occupied || cell.Occupant.IsTerrain) && Manager.Quests.FarEnoughAway(cell.WorldSpace))
                {
                    return cell.Id;
                }
            }

            Debug.LogError("Couldn't find valid location for quest");
            //TODO: A backup better than this
            return Random.Range(200, 1200);
        }
        
        private Location NewSpawnLocation() => spawnLocations.SelectRandom();
        
        private void SpawnGuildHall()
        {
            foreach (Cell cell in Manager.Map.GetCells(TownCentre, 4).Where(cell => cell.Occupied)) Remove(cell.Occupant);
            Manager.Structures.AddBuilding(Manager.Cards.GuildHall, SpawnLocation.root, SpawnLocation.rotation);
        }

        public StructureDetails Save()
        {
            return new StructureDetails
            {
                buildings = _buildings.Concat(_ruins).Select(x => x.SaveBuilding()).ToList(),
                terrain = _terrain.Select(x => x.SaveTerrain()).ToList()
            };
        }

        public void Load(StructureDetails details)
        {
            foreach (BuildingDetails building in details.buildings)
                AddBuilding(Manager.Cards.Find(building.type), building.rootId, building.rotation, building.isRuin);
            
            foreach (TerrainDetails terrain in details.terrain)
                AddTerrain(terrain.rootId, terrain.sectionCount);
            Structure guildHall = _buildings.Find(structure => structure.Blueprint.type == BuildingType.GuildHall);
            if (guildHall)
                TownCentre = guildHall.Occupied[0].WorldSpace;
            else
            {
                SpawnLocation = NewSpawnLocation();
                TownCentre = Manager.Map.GetCell(SpawnLocation.root).WorldSpace;
            }
        }
    }
}
