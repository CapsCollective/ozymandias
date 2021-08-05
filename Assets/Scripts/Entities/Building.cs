using System;
using System.Collections.Generic;
using System.Linq;
using Controllers;
using NaughtyAttributes;
using UnityEngine;
using Utilities;
using static Managers.GameManager;
using Random = UnityEngine.Random;
using DG.Tweening;
using Managers;

namespace Entities
{
    public class Building : MonoBehaviour
    {
        [Serializable]
        public class SectionInfo
        {
            public GameObject prefab;
            public List<Direction> directions;
            public int clockwiseRotations;
        }

        public enum Direction
        {
            Left,
            Forward,
            Right,
            Back
        }

        private enum ScaleSpeed
        {
            Slow = 4,
            Medium = 3,
            Fast = 2,
            VeryFast = 1
        } // Calculated placement rate

        [TextArea(3, 5)] public string description;
        public Sprite icon;
        public BuildingType type;
        public SerializedDictionary<Stat, int> stats;
        [SerializeField] private int baseCost;
        [SerializeField] private Color roofColor;
        [SerializeField] private ScaleSpeed scaleSpeed;

        private int _rootId; // Cell id of the building root
        private int _rotation;
        private List<BuildingSection> _sections = new List<BuildingSection>();

        public Quest Quest { get; private set; }
        public List<Cell> Occupied { get; private set; }
        public bool Selected { get; set; }
        public bool IsRuin => type == BuildingType.Ruins;
        public bool IsTerrain => type == BuildingType.Terrain;
        public bool IsQuest => type == BuildingType.Quest;
        public int SectionCount => _sections.Count;

        public int ScaledCost =>
            Mathf.FloorToInt(baseCost * Mathf.Pow(1.25f, Manager.Buildings.GetCount(type) * 4 / (float)scaleSpeed));

        [HorizontalLine] public List<SectionInfo> sections;
        public bool indestructible;

        // Check to prevent immediate selection on build

        private ParticleSystem _particleSystem;

        private ParticleSystem ParticleSystem => _particleSystem
            ? _particleSystem
            : _particleSystem = GetComponentInChildren<ParticleSystem>();

        private readonly List<Renderer> _sectionRenderers = new List<Renderer>();
        
        public bool Create(List<int> occupied, Quest quest, bool animate = false) // Creation for quests
        {
            if (!IsQuest) return false;
            name = quest.name;
            Quest = quest;
            
            Occupied = Manager.Map.GetCells(occupied);
            transform.position = Occupied[0].WorldSpace;
            _rootId = Occupied[0].Id;
            
            // Destroy anything in its way
            Occupied.ForEach(cell => {
                if (cell.Occupied) Manager.Buildings.Remove(cell.Occupant);
            });
            foreach (Cell cell in Occupied) cell.Occupant = this;
            
            // Same section for each quest cell
            _sections = Enumerable.Range(0, Occupied.Count).Select(_ => 
                Instantiate(Manager.Quests.SectionPrefab, transform).GetComponent<BuildingSection>()).ToList();
            for (int i = 0; i < _sections.Count; i++) AddSection(_sections[i], Occupied[i], i);

            if (animate) AnimateCreate();
            return true;
        }

        public bool Create(int rootId, int rotation = 0, int sectionCount = -1, bool isRuin = false, bool animate = false)
        {
            if (IsQuest) return false;

            name = name.Replace("(Clone)", "");
            if (isRuin) type = BuildingType.Ruins;

            Occupied = Manager.Map.GetCells(this, rootId, rotation);

            // Get the centre and count of all valid cells
            Vector3 centre = new Vector3();
            int cellCount = 0;
            centre = Occupied
                .TakeWhile(cell => Cell.IsValid(cell) && 
                                   !Occupied.GetRange(0, cellCount).Contains(cell) && 
                                   cellCount++ != sectionCount
                ).Aggregate(centre, (current, cell) => current + cell.Centre);

            if (cellCount == 0) return false;
            centre /= cellCount;
            transform.position = Quaternion.Euler(90f, 0f, 30f) * centre; //Center rotated to get world position
            
            // Checks validity if terrain or building
            if (IsTerrain)
            {
                Occupied = Occupied.GetRange(0, cellCount); // Limit cells
            }
            else
            {
                // If all cells are valid
                if (cellCount != Occupied.Count || !Manager.Spend(ScaledCost)) return false;
                if (!isRuin) Manager.Map.CreateRoad(Occupied);
                Manager.Map.Align(Occupied, rotation); // Align vertices to rotate the building sections in the right direction
            }

            _rootId = Occupied[0].Id;
            _rotation = rotation;
            foreach (Cell cell in Occupied) cell.Occupant = this;
            Random.InitState(Occupied[0].Id); // Init random with the id so it's the same each time
            
            if (IsTerrain)
            {
                // Randomises the sections of terrain, giving a 1/4 chance to be a rock
                // TODO: Look into randomising the shape too
                sections.ForEach(section => section.prefab = Random.Range(0,4) == 0 ? 
                    Manager.Buildings.RockSection : 
                    Manager.Buildings.TreeSection
                );
            }
            
            // Spawn sections for building/ terrain
            _sections = sections.Take(Occupied.Count).Select(section =>
                Instantiate(section.prefab, transform).GetComponent<BuildingSection>()).ToList();
            
            for (int i = 0; i < _sections.Count; i++) AddSection(_sections[i], Occupied[i], i);
            
            if (animate) AnimateCreate();
            return true;
        }

        public void Destroy()
        {
            foreach (Cell cell in Occupied) cell.Occupant = null;
            
            ChangeParticleSystemParent();
            ParticleSystem.Play();
            transform.DOScale(Vector3.zero, .25f).SetEase(Ease.OutSine).OnComplete(() => Destroy(gameObject));
            
            Jukebox.Instance.PlayDestroy();
        }
        
        public void Grow(Cell newCell)
        {
            BuildingSection newSection = Instantiate(Manager.Quests.SectionPrefab, transform).GetComponent<BuildingSection>();
            if (newCell.Occupied) Manager.Buildings.Remove(newCell.Occupant);
            newCell.Occupant = this;
            Occupied.Add(newCell);
            _sections.Add(newSection);
            AddSection(newSection, newCell);
        }

        private void AddSection(BuildingSection section, Cell cell, int i = 0)
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
                section.Fit(corners, sections[i].clockwiseRotations);
                section.SetRoofColor(IsRuin ? new Color(0,0,0) : roofColor);
            }
            
            // Add the renderer for all sections to a list for outline highlighting
            _sectionRenderers.Add(section.transform.GetComponent<Renderer>());
        }

        private void AnimateCreate()
        {
            transform.localScale = Vector3.zero;
            transform.DOScale(Vector3.one, 1f).SetEase(Ease.OutElastic);
            ParticleSystem.Play();
            Jukebox.Instance.PlayBuild(); // Only play sound if animated
        }
        
        public void ToRuin()
        {
            // TODO: Iterate through sections and replace with ruin
            type = BuildingType.Ruins;
            foreach (BuildingSection section in _sections)
            {
                section.SetRoofColor(new Color(0,0,0));
            }
        }

        public BuildingDetails Save()
        {
            return new BuildingDetails
            {
                name = name,
                rootId = _rootId,
                rotation = _rotation,
                sectionCount = SectionCount,
                isRuin = IsRuin
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
            if (!Selected) return;

            foreach (Renderer r in _sectionRenderers)
            {
                BuildingOutline.OutlineBuffer?.DrawRenderer(r, Manager.Buildings.OutlineMaterial);
            }
        }
    }
}
