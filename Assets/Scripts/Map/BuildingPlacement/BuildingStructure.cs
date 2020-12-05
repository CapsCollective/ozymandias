using System.Collections.Generic;
using System.Linq;
using Controllers;
using UnityEngine;

public class BuildingStructure : MonoBehaviour
{
    // Member Variables
    public List<SectionInfo> sections;
    public bool indestructible;
    public bool fitToCell;

    public string buildTrigger = "Build";
    public string clearTrigger = "Clear";

    private Animator _animator;
    private Animator Animator => _animator ? _animator : _animator = GetComponent<Animator>();

    private ParticleSystem _particleSystem;
    private ParticleSystem ParticleSystem => _particleSystem ? _particleSystem : _particleSystem = GetComponentInChildren<ParticleSystem>();

    // Enums
    public enum Direction { Left, Forward, Right, Back }

    // Class Functions
    public void Fit(Vector3[][] vertices, float heightFactor, bool animate = false)
    {
        if (fitToCell)
        {
            for (int i = 0; i < sections.Count; i++)
            {
                Section section = Instantiate(sections[i].prefab, transform).GetComponent<Section>();
                section.Fit(vertices[i], heightFactor);
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

        if (animate) Animator.SetTrigger(buildTrigger);
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

    public void Clear()
    {
        Animator.SetTrigger(clearTrigger);
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }

    // Structs
    [System.Serializable]
    public struct SectionInfo
    {
        public GameObject prefab;
        public List<Direction> directions;
    }

    public void PlayBuildSound()
    {
        Jukebox.Instance.PlayBuild();
    }
    
    public void PlayDestroySound()
    {
        Jukebox.Instance.PlayDestroy();
    }
}
