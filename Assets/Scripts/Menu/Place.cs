using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Place : MonoBehaviour
{
    EventSystem eventSystem;
    public Image image;
    public Map map;
    public Click selectedObject;
    private GameObject thingInstantiated;
    private RaycastHit hit;

    private void Start()
    {
        eventSystem = EventSystem.current;
    }

    void Update()
    {
        if (selectedObject)
        {
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane));
            Physics.Raycast(ray, out hit);
            if (!thingInstantiated)
            {
                if (hit.collider)
                {
                    thingInstantiated = Instantiate(selectedObject.building, hit.point, transform.rotation);
                }
            }
            else
            {
                thingInstantiated.transform.position = hit.point;
            }
            if (Input.GetMouseButtonDown(1))
            {
                thingInstantiated.transform.Rotate(0,30,0);
            }
        }


        if (Input.GetMouseButtonDown(0))
        {
            if (selectedObject)
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane));
                Physics.Raycast(ray, out hit);
                if (hit.collider)
                {
                    Destroy(thingInstantiated);
                    map.Occupy(selectedObject.building, hit.point);
                    selectedObject = null;
                }
            }
        }
    }
}
