using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingStructure : MonoBehaviour
{
    // Member Variables
    public List<SectionInfo> sections;
    public bool indestructable;

    public string buildTrigger = "Build";

    private Animator _animator;
    private Animator Animator {  get { return _animator ? _animator : _animator = GetComponent<Animator>(); } }

    // Enums
    public enum Direction { Left, Forward, Right, Back }

    // Class Functions
    public void Fit(Vector3[][] vertices, float heightFactor, bool animate = false)
    {
        for (int i = 0; i < sections.Count; i++)
        {
            Section section = Instantiate(sections[i].prefab, transform).GetComponent<Section>();
            section.Fit(vertices[i], heightFactor);
        }

        Debug.Log(buildTrigger + " : " + animate);

        if (animate) Animator.SetTrigger(buildTrigger);
    }

    // Structs
    [System.Serializable]
    public struct SectionInfo
    {
        public GameObject prefab;
        public List<Direction> directions;
    }
}