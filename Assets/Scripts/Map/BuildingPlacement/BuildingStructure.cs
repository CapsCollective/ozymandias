using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingStructure : MonoBehaviour
{
    // Member Variables
    public List<SectionInfo> sections;

    // Enums
    public enum Direction { Left, Forward, Right, Back }

    // Class Functions
    public void Fit(Vector3[][] vertices)
    {
        for (int i = 0; i < sections.Count; i++)
        {
            Section section = Instantiate(sections[i].prefab, transform).GetComponent<Section>();
            section.Fit(vertices[i]);
        }

    }

    // Structs
    [System.Serializable]
    public struct SectionInfo
    {
        public GameObject prefab;
        public List<Direction> directions;
    }
}