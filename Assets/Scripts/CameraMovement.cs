using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraMovement : MonoBehaviour
{
    private Rigidbody rb;
    private Camera cam;
    private Vector3 dragOrigin, cameraOrigin;
    private float rotateOrigin;
    private bool dragging, rotating;
    
    public float
        dragSpeed = 3.5f,
        scrollSpeed = 15,
        rotateSpeed = 180;
    
    public int
        minHeight = 4,
        maxHeight = 20;
    
    void Awake()
    {
        cam = GetComponent<Camera>();
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (!rotating && Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            dragOrigin = Input.mousePosition;
            cameraOrigin = transform.position;
            dragging = true;
        }
        else if (!dragging && Input.GetMouseButtonDown(1) && !EventSystem.current.IsPointerOverGameObject())
        {
            dragOrigin = Input.mousePosition;
            rotateOrigin = transform.eulerAngles.y;
            rotating = true;

        }
        
        if (Input.GetMouseButtonUp(0))
        {
            dragging = false;
        }
        else if (Input.GetMouseButtonUp(1))
        {
            rotating = false;
        }

        if (dragging)
        {
            Vector3 dir = cam.ScreenToViewportPoint(dragOrigin - Input.mousePosition);
            Transform t = transform;
            Vector3 pos = t.position;
            t.position = cameraOrigin + 
                         Quaternion.Euler(0, t.eulerAngles.y, 0) * 
                         new Vector3(dir.x * dragSpeed * pos.y, 0, dir.y * dragSpeed * pos.y);
        }
        else if(rotating)
        {
            Vector3 dir = cam.ScreenToViewportPoint(Input.mousePosition - dragOrigin);
            Transform t = transform;
            Vector3 angles = t.eulerAngles;
            t.eulerAngles = new Vector3(angles.x, rotateOrigin + dir.x * rotateSpeed, angles.z);
        }
        else
        {
            if (transform.position.y > maxHeight)
            {
                rb.AddForce(transform.TransformDirection(new Vector3(0, 0,2)));
            }
            else if (transform.position.y < minHeight)
            {
                rb.AddForce(transform.TransformDirection(new Vector3(0, 0,-1)));
            }
            else
            {
                Debug.Log(Input.GetAxis("Zoom"));
                rb.AddForce(transform.TransformDirection(
                    new Vector3(0, 0, Mathf.Clamp(Input.GetAxis("Zoom"), -1, 1) * scrollSpeed * 10)
                ));
            }
        }
    }
}
