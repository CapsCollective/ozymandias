using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    private Rigidbody _rb;

    public int moveSpeed, scrollSpeed, rotateSpeed;
    // Start is called before the first frame update
    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 acceleration = transform.TransformDirection(new Vector3(Input.GetAxis("Horizontal") * moveSpeed * 2, Input.GetAxis("Vertical") * moveSpeed / Mathf.Cos(Mathf.Deg2Rad * transform.eulerAngles.x), (Input.GetAxis("Zoom") * scrollSpeed * 100) + (Input.GetAxis("Vertical") * moveSpeed / Mathf.Sin(Mathf.Deg2Rad * transform.eulerAngles.x))));
        transform.eulerAngles += new Vector3(0,Input.GetAxis("Camera Rotate") * Time.deltaTime * rotateSpeed,0);
        _rb.AddForce(acceleration);
    }
}
