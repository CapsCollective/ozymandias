using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using DG.Tweening;
using Managers;
using Map;
using Quests;
using UnityEngine;
using Utilities;
using static Managers.GameManager;
using Random = UnityEngine.Random;

namespace Structures
{
    public class Structure : MonoBehaviour
    {
        public Blueprint Blueprint { get; private set; } // Left Blank for terrain and quest structures
        public StructureType StructureType { get; private set; }
        public Quest Quest { get; set; }
        public List<Cell> Occupied { get; private set; }
        public bool Selected { get; set; }
        public bool IsBuilding => StructureType == StructureType.Building;
        public bool IsRuin => StructureType == StructureType.Ruins;
        public bool IsTerrain => StructureType == StructureType.Terrain;
        public bool IsQuest => StructureType == StructureType.Quest;
        public bool IsFixed => fixedPosition;
        public bool IsBuildingType(BuildingType type) => Blueprint && Blueprint.type == type;
        public int SectionCount => _sections.Count;
        public Stat? Bonus { get; private set; }
        
        public int ClearCost => IsRuin ? 
            (int)(RuinsBaseCost * (10f - Manager.Upgrades.GetLevel(UpgradeType.Terrain)) / 10f *
                  Mathf.Pow(RuinsCostScale, Vector3.Distance(transform.position, Manager.Structures.TownCentre))) :
            (int)(TerrainBaseCost * SectionCount * (10f - Manager.Upgrades.GetLevel(UpgradeType.Terrain)) / 10f *
                  Mathf.Pow(TerrainCostScale, Vector3.Distance(transform.position, Manager.Structures.TownCentre)));
        
        // Stored here as well as blueprints so the stats can be modified by adjacency bonuses
        public Dictionary<Stat, int> Stats { get; private set; }
        
        private int _rootId; // Cell id of the building root
        private int _rotation;
        private List<Section> _sections = new List<Section>();
        private ParticleSystem _particleSystem;
        private ParticleSystem ParticleSystem => _particleSystem
            ? _particleSystem
            : _particleSystem = GetComponentInChildren<ParticleSystem>();
        private List<Renderer> _sectionRenderers = new List<Renderer>();
        [SerializeField] private bool fixedPosition;
        
        private void Awake()
        {
            if (!fixedPosition) return;
            StructureType = StructureType.Quest;
            _sectionRenderers = GetComponentsInChildren<Renderer>().ToList();
        }

        public void CreateQuest(List<int> occupied, Quest quest) // Creation for quests
        {
            name = quest.name;
            StructureType = StructureType.Quest;
            Quest = quest;
            
            Occupied = Manager.Map.GetCells(occupied);
            transform.position = Occupied[0].WorldSpace;
            _rootId = Occupied[0].Id;
            
            // Destroy anything in its way
            Occupied.ForEach(cell => {
                if (cell.Occupied) Manager.Structures.Remove(cell.Occupant);
            });
            foreach (Cell cell in Occupied) cell.Occupant = this;
            
            // Same section for each quest cell
            _sections = Enumerable.Range(0, Occupied.Count).Select(_ => 
                Instantiate(Manager.Quests.SectionPrefab, transform).GetComponent<Section>()).ToList();
            for (int i = 0; i < _sections.Count; i++)
            {
                _sections[i].Init(Occupied[i]);
                _sections[i].SetRoofColor(quest.colour);
                _sectionRenderers.Add(_sections[i].GetComponent<Renderer>());
            }

            if (!Manager.State.Loading) AnimateCreate();
        }

        public void CreateTerrain(int rootId, int sectionCount = -1)
        {
            Random.InitState(rootId); // Init random with the id so it's the same each time
            
            List<SectionInfo> terrainSections = new List<SectionInfo>
            {
                new SectionInfo { directions = new List<Direction>()},
                new SectionInfo { directions = new List<Direction>{ Direction.Forward }},
                new SectionInfo { directions = new List<Direction> { Direction.Right}},
                new SectionInfo { directions = new List<Direction> { Direction.Forward, Direction.Right}}
            };
           
            name = "Forest";
            Occupied = Manager.Map.GetCells(terrainSections, rootId);
            StructureType = StructureType.Terrain;
            _rootId = rootId;
            _rotation = 0;

            Vector3 centre = new Vector3();
            int cellCount = 0;
            centre = Occupied
                .TakeWhile(cell => 
                    Cell.IsValid(cell) && 
                    !Occupied.GetRange(0, cellCount).Contains(cell) && // No looping around
                    cellCount++ != sectionCount
                ).Aggregate(centre, (current, cell) => current + cell.Centre);

            if (cellCount == 0)
            {
                Destroy(gameObject);
                return;
            }
            centre /= cellCount;
            transform.position = Manager.Map.transform.TransformPoint(centre); // Center rotated to get world position
            
            Occupied = Occupied.GetRange(0, cellCount); // Limit cells
            foreach (Cell cell in Occupied) cell.Occupant = this;

            _sections = Enumerable
                .Range(0, cellCount)
                .Select(i => Instantiate(
                    Random.Range(0, 5) == 0 ? 
                    Manager.Structures.RockSection :
                    Manager.Structures.TreeSection,
                    transform)
                .GetComponent<Section>()).ToList();
            for (int i = 0; i < _sections.Count; i++) 
            {
                _sections[i].Init(Occupied[i]);
                _sectionRenderers.Add(_sections[i].GetComponent<Renderer>());
            }

            if (!Manager.State.Loading) AnimateCreate(); // TODO: Animate check (just not during loading???)
        }
        
        public bool CreateBuilding(Blueprint blueprint, int rootId, int rotation = 0, bool isRuin = false)
        {
            Stats = blueprint.stats;
            Blueprint = blueprint;
            name = blueprint.name; // TODO: We could randomly pick building names?
            StructureType = isRuin ? StructureType.Ruins : StructureType.Building;
            Occupied = Manager.Map.GetCells(blueprint.sections, rootId, rotation);

            // Get the centre and count of all valid cells
            Vector3 centre = new Vector3();
            int cellCount = 0;
            centre = Occupied
                .TakeWhile(cell => Cell.IsValid(cell) && !Occupied.GetRange(0, cellCount++).Contains(cell))
                .Aggregate(centre, (current, cell) => current + cell.Centre);

            if (cellCount != Occupied.Count || !Manager.Stats.Spend(Blueprint.ScaledCost)) return false;
            centre /= cellCount;
            transform.position = Manager.Map.transform.TransformPoint(centre);
            
            if (!isRuin) Manager.Map.CreateRoad(Occupied);
            Manager.Map.Align(Occupied, rotation); // Align vertices to rotate the building sections in the right direction

            _rootId = Occupied[0].Id;
            _rotation = rotation;
            foreach (Cell cell in Occupied) cell.Occupant = this;

            // Spawn sections
            _sections = blueprint.sections
                .Take(Occupied.Count)
                .Select(section => Instantiate(section.prefab, transform).GetComponent<Section>())
                .ToList();

            for (int i = 0; i < _sections.Count; i++)
            {
                _sections[i].Init(Occupied[i], true, isRuin, Blueprint.sections[i].clockwiseRotations);
                _sections[i].SetRoofColor(Blueprint.roofColor);
                // Add the renderer for all sections to a list for outline highlighting
                _sectionRenderers.Add(_sections[i].transform.GetComponent<Renderer>());
            }

            if (!Manager.State.Loading) AnimateCreate();
            Bonus = AdjacencyBonus();
            return true;
        }

        public void Destroy()
        {
            foreach (Cell cell in Occupied) cell.Occupant = null;
            
            ChangeParticleSystemParent();
            ParticleSystem.Play();
            transform.DOScale(Vector3.zero, .25f).SetEase(Ease.OutSine).OnComplete(() => Destroy(gameObject));
            
            if(Manager.State.InGame) Manager.Jukebox.PlayDestroy();
            Manager.Map.GetNeighbours(this).ForEach(neighbour => neighbour.Bonus = neighbour.AdjacencyBonus());
        }
        
        public void Grow(Cell newCell)
        {
            Section newSection = Instantiate(Manager.Quests.SectionPrefab, transform).GetComponent<Section>();
            if (newCell.Occupied) Manager.Structures.Remove(newCell.Occupant);
            newCell.Occupant = this;
            Occupied.Add(newCell);
            _sections.Add(newSection);
            newSection.SetRoofColor(Quest.colour);
            newSection.Init(newCell);
            _sectionRenderers.Add(newSection.GetComponent<Renderer>());
        }

        public void Shrink(int count)
        {
            for (int i = 0; i < count; i++)
            {
                _sectionRenderers.Remove(_sections[SectionCount - 1].transform.GetComponent<Renderer>());

                Destroy(_sections[SectionCount - 1].gameObject);
                Occupied[Occupied.Count - 1].Occupant = null;
                _sections.RemoveAt(SectionCount - 1);
                Occupied.RemoveAt(Occupied.Count - 1);
            }
        }

        private void AnimateCreate()
        {
            transform.localScale = Vector3.zero;
            transform.DOScale(Vector3.one, 1f).SetEase(Ease.OutElastic);
            ParticleSystem.Play();
            Manager.Jukebox.PlayBuild(); // Only play sound if animated
        }
        
        public void ToRuin()
        {
            StructureType = StructureType.Ruins;
            foreach (Section section in _sections)
            {
                section.ToRuin();
            }
        }
        
        private Stat? AdjacencyBonus(bool propagate = true)
        {
            // Only applies to buildings with unlocked bonuses
            if (!IsBuilding || !Blueprint.adjacencyConfig.hasBonus || !Manager.Upgrades.IsUnlocked(Blueprint.adjacencyConfig.upgrade)) return null;
            
            AdjacencyConfiguration config = Blueprint.adjacencyConfig;
            int farmCount = 0;
            bool noAdjacentBuildings = true;
            bool hasBonus = false;
            
            foreach (Structure neighbour in Manager.Map.GetNeighbours(this))
            {
                if (propagate) neighbour.Bonus = neighbour.AdjacencyBonus(false); // Recheck neighbours too
                // Checking for Terrain and Ruin bonuses
                if (config.structureType != StructureType.Building && neighbour.StructureType == config.structureType) hasBonus = true;
                if (neighbour.StructureType != StructureType.Building) continue; 
                noAdjacentBuildings = false;
                if (neighbour.IsBuildingType(BuildingType.Farm)) farmCount++;
                else if (!config.specialCheck && neighbour.IsBuildingType(config.neighbourType)) hasBonus = true;
            }

            // Special adjacency rules
            if (farmCount >= 2 && IsBuildingType(BuildingType.Farm)) hasBonus = true;
            if (noAdjacentBuildings && (IsBuildingType(BuildingType.Watchtower) || IsBuildingType(BuildingType.Monastery))) hasBonus = true;
            
            return hasBonus ? config.stat : (Stat?)null; 
        }
        
        public BuildingDetails SaveBuilding()
        {
            return new BuildingDetails
            {
                type = Blueprint.type,
                rootId = _rootId,
                rotation = _rotation,
                isRuin = IsRuin
            };
        }
        
        public TerrainDetails SaveTerrain()
        {
            return new TerrainDetails
            {
                rootId = _rootId,
                sectionCount = SectionCount,
            };
        }

        private void ChangeParticleSystemParent()
        {
            ParticleSystem.transform.parent = null;
            ParticleSystem.MainModule psMain = ParticleSystem.main;
            psMain.stopAction = ParticleSystemStopAction.Destroy;
        }
        
        private void Update()
        {
            #if UNITY_EDITOR
            if (Manager.disableOutline) return;
            #endif
            
            //TODO: Make this on mouse input instead of every frame?
            if (!Selected || !Manager.State.InGame) return;

            foreach (Renderer r in _sectionRenderers)
            {
                Outline.OutlineBuffer?.DrawRenderer(r, Manager.Structures.OutlineMaterial);
            }
        }
    }
}
