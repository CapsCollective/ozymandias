using System;
using System.Collections.Generic;
using System.Linq;
using Controllers;
using NaughtyAttributes;
using UnityEngine;
using Utilities;
using static Managers.GameManager;
using Random = UnityEngine.Random;

namespace Entities
{
    public class Building : MonoBehaviour
    {
        public enum Direction { Left, Forward, Right, Back }
        private enum ScaleSpeed { Slow = 1, Medium = 2, Fast = 3, VeryFast = 4 }

        [TextArea(3,5)]
        public string description;
        public Sprite icon;
        public BuildingType type;
        public GenericDictionary<Stat, int> stats;
        public int baseCost;
        public Color roofColor;
        [SerializeField] private ScaleSpeed scaleSpeed;
        //[HideInInspector] public bool operational;
        
        private Vector3 _placementPosition;
        private int _rotation;
        
        public int ScaledCost => Mathf.FloorToInt( baseCost * Mathf.Pow(1.1f, Manager.Buildings.GetCount(type) * (int)scaleSpeed));
        
        [HorizontalLine]
        public List<SectionInfo> sections;
        public bool indestructible;
        [SerializeField] private bool fitToCell;

        private const string BuildTrigger = "Build";
        private const string ClearTrigger = "Clear";

        private Animator _animator;
        private Animator Animator => _animator ? _animator : _animator = GetComponent<Animator>();

        private ParticleSystem _particleSystem;
        private ParticleSystem ParticleSystem => _particleSystem ? _particleSystem : _particleSystem = GetComponentInChildren<ParticleSystem>();
        
        public void Fit(Vector3[][] vertices, float heightFactor, bool animate = false)
        {
            if (fitToCell)
            {
                for (int i = 0; i < sections.Count; i++)
                {
                    Section section = Instantiate(sections[i].prefab, transform).GetComponent<Section>();
                    section.clockwiseRotations = sections[i].clockwiseRotations;
                    section.Fit(vertices[i], heightFactor);
                    section.SetRoofColor(roofColor);
                }
            }
            else
            {
                for (int i = 0; i < sections.Count; i++)
                {
                    Section s = Instantiate(sections[i].prefab, transform).GetComponent<Section>();
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

            if (animate) Animator.SetTrigger(BuildTrigger);
        }
        
        public void Clear()
        {
            Animator.SetTrigger(ClearTrigger);
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
        
    }
}
