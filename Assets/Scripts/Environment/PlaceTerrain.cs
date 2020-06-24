using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlaceTerrain : MonoBehaviour
{
    public Map map;
    private RaycastHit hit;
    public float distance = 1.5f;
    private LayerMask lm;
    public GameObject terrainBuilding;
    
    public int rotation = 0;
    
    //private void Awake()
    //{
    //    lm = LayerMask.GetMask("Surface", "Terrain");
    //}

    //void Update()
    //{
    //    if (!map) map = FindObjectOfType<Map>();
    //    Place();
    //    enabled = false;
    //}

    public void Place()
    {
        lm = LayerMask.GetMask("Surface", "Terrain");
        if (GetSurfaceHit())
        {
            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Surface"))
            {
                Debug.Log(map);
                map.CreateBuilding(terrainBuilding, hit.point, rotation, animate: false);
                //Destroy(gameObject);
            }
        }
    }

    public bool GetSurfaceHit()
    {
        return Physics.Raycast(transform.position, Vector3.down, out hit, distance, lm);
    }
}
