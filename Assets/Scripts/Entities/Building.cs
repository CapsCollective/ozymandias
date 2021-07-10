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
        public enum Direction { Left, Forward, Right, Back }
        private enum ScaleSpeed { Slow = 4, Medium = 3, Fast = 2, VeryFast = 1 } // Calculated placement rate

        [TextArea(3,5)]
        public string description;
        public Sprite icon;
        public BuildingType type;
        public SerializedDictionary<Stat, int> stats;
        public int baseCost;
        public Color roofColor;
        [SerializeField] private ScaleSpeed scaleSpeed;

        private int _rootId; // Cell id of the building root
        private int _rotation;
        private int _sectionCount; // 
        private bool _isRuin;
        public int SectionCount => _sectionCount;
        
        public int ScaledCost => Mathf.FloorToInt( baseCost * Mathf.Pow(1.25f, 
            Manager.Buildings.GetCount(type) * 4 / (float)scaleSpeed));
        
        [HorizontalLine]
        public List<SectionInfo> sections;
        public bool indestructible;
        [SerializeField] private bool fitToCell;
        public bool grassMask;
        [SerializeField] private GameObject tree, rock;

        // Check to prevent immediate selection on build
        public bool selected;

        private ParticleSystem _particleSystem;
        private ParticleSystem ParticleSystem => _particleSystem ? _particleSystem : _particleSystem = GetComponentInChildren<ParticleSystem>();

        [SerializeField] private Material mat;

        private readonly List<Renderer> _segments = new List<Renderer>();

        public void Build(int rootId, int rotation, int sectionCount, Vector3[][] vertices, bool animate = false)
        {
            name = name.Replace("(Clone)", "");
            _rootId = rootId;
            _rotation = rotation;
            _sectionCount = sectionCount;
            Random.InitState(rootId); // Init random with the id so it's the same each time

            if (type == BuildingType.Terrain)
            {
                // Randomises the sections of terrain, giving a 1/4 chance to be a rock
                // TODO: Look into randomising the shape too
                sections.ForEach(section => section.prefab = Random.Range(0,4) == 0 ? rock : tree);
            }
            
            if (fitToCell)
            {
                for (int i = 0; i < sectionCount; i++)
                {
                    BuildingSection buildingSection = Instantiate(sections[i].prefab, transform).GetComponent<BuildingSection>();
                    buildingSection.clockwiseRotations = sections[i].clockwiseRotations;
                    buildingSection.Fit(vertices[i]);
                    buildingSection.SetRoofColor(roofColor);
                }
            }
            else
            {
                for (int i = 0; i < sectionCount; i++)
                {
                    BuildingSection s = Instantiate(sections[i].prefab, transform).GetComponent<BuildingSection>();
                    Vector3 v = new Vector3(
                        vertices[i].Average(x => x.x), 0,
                        vertices[i].Average(x => x.z)
                    );

                    Transform t = s.transform;
                    t.position = v;
                    t.eulerAngles = new Vector3(0, Random.value * 360, 0);
                    t.localScale *= Random.Range(0.8f, 1.2f);
                }
            }
            if (animate)
            {
                transform.localScale = Vector3.zero;
                transform.DOScale(Vector3.one, 1f).SetEase(Ease.OutElastic);
                ParticleSystem.Play();
            }
            if (grassMask)
            {
                gameObject.layer = LayerMask.NameToLayer("Mask");
                foreach (Transform t in transform)
                {
                    t.gameObject.layer = LayerMask.NameToLayer("Mask");
                }
            }
            
            Manager.Buildings.Add(this);
            
            // TODO: Work out what is this doing
            foreach (Transform t in transform)
            {
                if (t.GetComponent<ParticleSystem>()) continue;
                _segments.Add(t.GetComponent<Renderer>());
            }
        }
        
        public void Clear()
        {
            //Animator.SetTrigger(ClearTrigger);
            ChangeParticleSystemParent();
            ParticleSystem.Play();
            transform.DOScale(Vector3.zero, .25f).SetEase(Ease.OutSine).OnComplete(() => Manager.Buildings.Remove(this));
            Jukebox.Instance.PlayDestroy();
        }
        
        public void ToRuins(bool animate = false)
        {
            // TODO: Iterate through sections and replace with ruin
            _isRuin = true;
        }
        
        [Serializable]
        public class SectionInfo
        {
            public GameObject prefab;
            public List<Direction> directions;
            public int clockwiseRotations;
        }

        public BuildingDetails Save()
        {
            return new BuildingDetails
            {
                name = name,
                rootId = _rootId,
                rotation = _rotation,
                sectionCount = _sectionCount,
                isRuin = _isRuin
            };
        }
        
        // Animator Methods
        // Functionality for buildings called in the animator
        public void PlayBuildSound()
        {
            Jukebox.Instance.PlayBuild();
        }
    
        public void PlayDestroySound()
        {
            Jukebox.Instance.PlayDestroy();
        }
        
        public void Destroy()
        {
            Destroy(gameObject);
        }
        
        public void Burst()
        {
            ParticleSystem.Play();
        }

        public void ChangeParticleSystemParent()
        {
            ParticleSystem.transform.parent = null;
            var psMain = ParticleSystem.main;
            psMain.stopAction = ParticleSystemStopAction.Destroy;
        }
        
        private void Update()
        {
            if (!selected || mat == null) return;

            foreach (Renderer r in _segments)
            {
                //t.GetComponent<Renderer>().material.SetInt("_Selected", selected ? 1 : 0);
                BuildingOutline.OutlineBuffer?.DrawRenderer(r, mat);
            }
        }
    }
}
