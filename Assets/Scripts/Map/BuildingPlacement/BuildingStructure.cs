using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Managers_and_Controllers;
using UnityEngine;

public class BuildingStructure : MonoBehaviour
{
    // Member Variables
    public List<SectionInfo> sections;
    public bool indestructable;
    public bool fitToCell = false;

    public string buildTrigger = "Build";
    public string clearTrigger = "Clear";

    private Animator _animator;
    private Animator Animator { get { return _animator ? _animator : _animator = GetComponent<Animator>(); } }

    private ParticleSystem _particleSystem;
    private ParticleSystem ParticleSystem { get { return _particleSystem ? _particleSystem : _particleSystem = GetComponentInChildren<ParticleSystem>(); } }

    // Enums
    public enum Direction { Left, Forward, Right, Back }

    public void Start()
    {

    }

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
                Vector3 v = new Vector3();
                v.x = vertices[i].Average(x => x.x);
                v.z = vertices[i].Average(x => x.z);
                s.transform.position = v;

                s.transform.eulerAngles = new Vector3(0, Random.value * 360, 0);
                s.transform.localScale = s.transform.localScale * Random.Range(0.8f, 1.2f);
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
        JukeboxController.Instance.PlayBuild();
    }
    
    public void PlayDestroySound()
    {
        JukeboxController.Instance.PlayDestroy();
    }
}