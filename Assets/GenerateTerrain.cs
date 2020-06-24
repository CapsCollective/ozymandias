using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateTerrain : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Map map = FindObjectOfType<Map>();
        foreach (var pt in GetComponentsInChildren<PlaceTerrain>())
        {
            Debug.Log("pt");
            pt.map = map;
            pt.Place();
        }

        //GameObject buildings = GameObject.Find("Buildings");
        //GameObject[] terrain = new GameObject[buildings.transform.childCount];
        //Debug.Log(buildings.transform.childCount);
        //for (int i = 0; i < terrain.Length; i++)
        //{
        //    terrain[i] = buildings.transform.GetChild(i).gameObject;
        //}
        //StaticBatchingUtility.Combine(terrain, buildings);
    }

}
