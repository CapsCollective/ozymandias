using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class MapColliderController : MonoBehaviour
{
    [SerializeField] private Map map;
    [SerializeField] private float size = 25;

    [Button]
    public void GenerateColliders()
    {
        map = GetComponent<Map>();
        MapLayout layout = map.layout;
        foreach (Cell cell in layout.CellGraph.GetData())
        {
            GameObject go = new GameObject("Collider " + cell.Centre);
        }
    }
}
