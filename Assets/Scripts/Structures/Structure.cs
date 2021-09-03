using System;
using System.Collections.Generic;
using System.Linq;
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
        public Quest Quest { get; private set; }
        public List<Cell> Occupied { get; private set; }
        public bool Selected { get; set; }
        public bool IsRuin => StructureType == StructureType.Ruins;
        public bool IsTerrain => StructureType == StructureType.Terrain;
        public bool IsQuest => StructureType == StructureType.Quest;
        public int SectionCount => _sections.Count;
        
        // Clear cost calculations TODO: make this based on distance to guild hall instead
        public static int TerrainClearCount { get; set; }
        public static int RuinsClearCount { get; set; }
        public int TerrainClearCost => 
            (int)(TerrainBaseCost * (10f - Manager.Upgrades.GetLevel(UpgradeType.Terrain)) / 10f *
                  Enumerable.Range(TerrainClearCount, SectionCount).Sum(i => Mathf.Pow(TerrainCostScale, i)));
        public int RuinsClearCost =>
            (int)(RuinsBaseCost * (10f - Manager.Upgrades.GetLevel(UpgradeType.Ruins)) / 10f * 
                  Enumerable.Range(RuinsClearCount, SectionCount).Sum(i => Mathf.Pow(RuinsCostScale, i)));
        
        // Stored here as well as blueprints so the stats can be modified by adjacency bonuses
        public Dictionary<Stat, int> Stats { get; private set; }
        
        private int _rootId; // Cell id of the building root
        private int _rotation;
        private List<Section> _sections = new List<Section>();
        private ParticleSystem _particleSystem;
        private ParticleSystem ParticleSystem => _particleSystem
            ? _particleSystem
            : _particleSystem = GetComponentInChildren<ParticleSystem>();
        private readonly List<Renderer> _sectionRenderers = new List<Renderer>();
        
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
            for (int i = 0; i < _sections.Count; i++) AddSection(_sections[i], Occupied[i], i);

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
            for (int i = 0; i < _sections.Count; i++) AddSection(_sections[i], Occupied[i], i);
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
            
            for (int i = 0; i < _sections.Count; i++) AddSection(_sections[i], Occupied[i], i);
            
            if (!Manager.State.Loading) AnimateCreate();
            return true;
        }

        public void Destroy()
        {
            foreach (Cell cell in Occupied) cell.Occupant = null;
            
            ChangeParticleSystemParent();
            ParticleSystem.Play();
            transform.DOScale(Vector3.zero, .25f).SetEase(Ease.OutSine).OnComplete(() => Destroy(gameObject));
            
            Manager.Jukebox.PlayDestroy();
        }
        
        public void Grow(Cell newCell)
        {
            Section newSection = Instantiate(Manager.Quests.SectionPrefab, transform).GetComponent<Section>();
            if (newCell.Occupied) Manager.Structures.Remove(newCell.Occupant);
            newCell.Occupant = this;
            Occupied.Add(newCell);
            _sections.Add(newSection);
            AddSection(newSection, newCell);
        }

        private void AddSection(Section section, Cell cell, int i = 0)
        {
            Vector3[] corners = Manager.Map.GetCornerPositions(cell);
            
            if (IsTerrain || IsQuest)
            {
                Vector3 v = new Vector3 (
                    corners.Average(x => x.x), 0,
                    corners.Average(x => x.z)
                );

                Transform t = section.transform;
                t.position = v;
                t.eulerAngles = new Vector3(0, Random.value * 360, 0);
                t.localScale *= Random.Range(0.8f, 1.2f);         
            } 
            else 
            {
                section.Fit(corners, Blueprint ? Blueprint.sections[i].clockwiseRotations : 0);
                section.SetRoofColor(IsRuin ? new Color(0,0,0) : Blueprint.roofColor);
            }
            
            // Add the renderer for all sections to a list for outline highlighting
            _sectionRenderers.Add(section.transform.GetComponent<Renderer>());
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
            // TODO: Iterate through sections and replace with ruin
            StructureType = StructureType.Ruins;
            foreach (Section section in _sections)
            {
                section.SetRoofColor(new Color(0,0,0));
            }
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
            //TODO: Make this on mouse input instead of every frame?
            if (!Selected || !Manager.State.InGame) return;

            foreach (Renderer r in _sectionRenderers)
            {
                Outline.OutlineBuffer?.DrawRenderer(r, Manager.Structures.OutlineMaterial);
            }
        }
    }
}
