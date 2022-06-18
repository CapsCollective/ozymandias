﻿using System;
using System.Collections.Generic;
using System.Linq;
using Managers;
using Map;
using NaughtyAttributes;
using UnityEngine;
using Utilities;
using static Managers.GameManager;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Structures
{
    public class Structures : MonoBehaviour
    {
        public static Action<Structure> OnBuild;
        public static Action<Structure> OnDestroy;
        public static Action OnGuildHallDemolished;

        [Serializable] private struct Location { public int root, rotation; }
        
        [SerializeField] private GameObject rockSection, treeSection, structurePrefab;
        [SerializeField] private Material outlineMaterial;
        [SerializeField] private Location dockLocation;
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
        public Vector3 Dock => Manager.Map.GetCell(dockLocation.root).WorldSpace;
        
        private void Awake()
        {
            State.OnNewGame += () => { if (!Tutorial.Tutorial.Active) SpawnGuildHall(); };
            State.OnGameEnd += RemoveAll;
            
            var buildingsText = Resources.LoadAll("SectionData/", typeof(TextAsset));
            foreach(Object o1 in buildingsText)
            {
                TextAsset o = (TextAsset) o1; // Explicit cast to avoid cast from Object to object above
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
        
        public Structure GetRandom(BuildingType type)
        {
            return _buildings.Where(x => x.Blueprint.type == type).ToList().SelectRandom();
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
                CheckAdjacencyBonuses();
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
                Manager.EventQueue.AddGuildHallDestroyedEvents();
                Manager.State.EnterState(GameState.NextTurn);
                OnGuildHallDemolished?.Invoke();
            }
            
            if (structure.IsTerrain) _terrain.Remove(structure);
            else if (structure.IsRuin) _ruins.Remove(structure);
            else _buildings.Remove(structure);
            
            structure.Destroy();
            CheckAdjacencyBonuses();
            OnDestroy?.Invoke(structure);
            if(!Manager.State.Loading) UpdateUi();
        }

        public string Remove(BuildingType type)
        {
            Structure structure = _buildings.Find(b => b.Blueprint.type == type);
            if (structure == null) return null;
            Remove(structure);
            return structure.name;
        }

        public void CheckAdjacencyBonuses()
        {
            _buildings.ForEach(building => building.CheckAdjacencyBonus());
        }

        private void RemoveAll()
        {
            int count = 0;
            List<Structure> dupList = new List<Structure>(_buildings);
            dupList.Reverse(); // Reverse processing order so its more likely the furthest out buildings become ruins
            dupList.ForEach(building =>
            {
                if (building.Blueprint.type != BuildingType.GuildHall &&
                    building.Blueprint.type != BuildingType.Farm &&
                    building.Blueprint.type != BuildingType.Markets &&
                    building.Blueprint.type != BuildingType.Plaza &&
                    building.Blueprint.type != BuildingType.Barracks &&
                    building.Blueprint.type != BuildingType.Armoury &&
                    building.Blueprint.type != BuildingType.BathHouse &&
                    building.Blueprint.type != BuildingType.Monastery &&
                    building.Blueprint.type != BuildingType.FightingRing &&
                    Random.Range(0, count) == 0
                )
                {
                    ToRuin(building);
                    count++;
                }
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
        
        public void SpawnGuildHall()
        {
            foreach (Cell cell in Manager.Map.GetCells(TownCentre, 4).Where(cell => cell.Occupied && cell.Occupant.IsTerrain)) Remove(cell.Occupant);
            Manager.Structures.AddBuilding(Manager.Cards.GuildHall, SpawnLocation.root, SpawnLocation.rotation);
        }

        public void SpawnTutorialRuins()
        {
            AddBuilding(Manager.Cards.Find(BuildingType.Herbalist), 158, 1, true);
            AddBuilding(Manager.Cards.Find(BuildingType.Herbalist), 225, 3, true);
            AddBuilding(Manager.Cards.Find(BuildingType.Herbalist), 445, 2, true);
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
            foreach (BuildingDetails building in details.buildings ?? new List<BuildingDetails>())
            {
                Blueprint blueprint = Manager.Cards.Find(building.type);
                if (!blueprint) Debug.LogError(
                    "Blueprint of type \"" + building.type + "\" could not be found." + 
                    "It may not yet be available to the player.");
                AddBuilding(
                    blueprint, 
                    building.rootId, building.rotation, building.isRuin);
            }

            foreach (TerrainDetails terrain in details.terrain ?? new List<TerrainDetails>())
                AddTerrain(terrain.rootId, terrain.sectionCount);
            Structure guildHall = _buildings.Find(structure => structure.Blueprint.type == BuildingType.GuildHall);
            if (guildHall)
                TownCentre = guildHall.Occupied[0].WorldSpace;
            else
            {
                SpawnLocation = Tutorial.Tutorial.Active ? spawnLocations[0] : NewSpawnLocation();
                TownCentre = Manager.Map.GetCell(SpawnLocation.root).WorldSpace;
            }
            
            CheckAdjacencyBonuses(); // Checks at end of load so it doesn't repeat
        }
    }
}
