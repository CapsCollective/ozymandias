using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceTerrain : MonoBehaviour
{
    public Map map;
    private RaycastHit hit;
    public float distance = 1.5f;
    private LayerMask lm;
    public GameObject terrainBuilding;
    
    private bool isPlaced = false;

    private void Awake()
    {
        
        lm = LayerMask.GetMask("Surface","Terrain");
    }

    // Start is called before the first frame update
    void Start()
    {
        Place();   
    }

    // Update is called once per frame
    void Update()
    {
        //Place();
    }

    public void Place()
    {
        if (!isPlaced)
        {
            isPlaced = true;
            if (map)
            {
                if (GetSurfaceHit())
                {
                    if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Surface"))
                    {
                        map.Occupy(terrainBuilding, hit.point);
                    }

                }
            }
        }
        
        
    }

    public bool GetSurfaceHit()
    {
        return Physics.Raycast(transform.position, Vector3.down, out hit, distance, lm);
    }
}
