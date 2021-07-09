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
        
        private Vector3 _placementPosition;
        private int _rotation;
        
        public int ScaledCost => Mathf.FloorToInt( baseCost * Mathf.Pow(1.25f, 
            Manager.Buildings.GetCount(type) * 4 / (float)scaleSpeed));
        
        [HorizontalLine]
        public List<SectionInfo> sections;
        public bool indestructible;
        [SerializeField] private bool fitToCell;
        public bool grassMask;

        // Check to prevent immediate selection on build
        public bool selected;

        private const string BuildTrigger = "Build";
        private const string ClearTrigger = "Clear";
        
        private ParticleSystem _particleSystem;
        private ParticleSystem ParticleSystem => _particleSystem ? _particleSystem : _particleSystem = GetComponentInChildren<ParticleSystem>();

        [SerializeField] private Material mat;

        private readonly List<Renderer> _segments = new List<Renderer>();

        public void Fit(Vector3[][] vertices, bool animate = false)
        {
            if (fitToCell)
            {
                for (int i = 0; i < sections.Count; i++)
                {
                    BuildingSection buildingSection = Instantiate(sections[i].prefab, transform).GetComponent<BuildingSection>();
                    buildingSection.clockwiseRotations = sections[i].clockwiseRotations;
                    buildingSection.Fit(vertices[i]);
                    buildingSection.SetRoofColor(roofColor);
                }
            }
            else
            {
                for (int i = 0; i < sections.Count; i++)
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
            //if (animate) Animator.SetTrigger(BuildTrigger);
        }
        
        public void Clear()
        {
            //Animator.SetTrigger(ClearTrigger);
            ChangeParticleSystemParent();
            ParticleSystem.Play();
            transform.DOScale(Vector3.zero, .25f).SetEase(Ease.OutSine).OnComplete(() => Manager.Buildings.Remove(this));
            Jukebox.Instance.PlayDestroy();
        }

        // Structs
        [Serializable]
        public struct SectionInfo
        {
            public GameObject prefab;
            public List<Direction> directions;
            public int clockwiseRotations;
        }

        public void Build(Vector3 placementPosition, int rotation)
        {
            name = name.Replace("(Clone)", "");
            _placementPosition = placementPosition;
            _rotation = rotation;
            Manager.Buildings.Add(this);
            
            foreach (Transform t in transform)
            {
                if (t.GetComponent<ParticleSystem>()) continue;
                _segments.Add(t.GetComponent<Renderer>());
            }
        }

        public string Save()
        {
            return $"{name},{_placementPosition.x:n2},{_placementPosition.z:n2},{_rotation % 4}";
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
