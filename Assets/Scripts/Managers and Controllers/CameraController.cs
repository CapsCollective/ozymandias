using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static GameManager;

public class CameraController : MonoBehaviour
{
    private Rigidbody rb;
    private Camera cam;
    private Vector3 dragOrigin, cameraOrigin, rotateAxis;
    private float rotateOrigin;
    private bool dragging, rotating;

    public Vector3 startPos, startRot;
    public Button centerButton;
    public TextMeshProUGUI centerButtonText;
    private bool crRunning = false;
    
    [Range(1,10)]
    public int
        dragSpeed = 3,
        scrollSpeed = 3,
        rotateSpeed = 3;
    
    public int
        minHeight = 5,
        maxHeight = 25,
        minAngle = 25,
        maxAngle = 60;

    public bool invertScroll;
    
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
            Ray ray = cam.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                rotateAxis = hit.point;
                rotating = true;
            }
        }
        
        if (Input.GetMouseButtonUp(0)) dragging = false;
        if (Input.GetMouseButtonUp(1)) rotating = false;

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
             transform.RotateAround(rotateAxis, Vector3.up, Input.GetAxis("Mouse X") * rotateSpeed);
        }
        else
        {
            if (transform.position.y > maxHeight)
            {
                rb.AddForce(new Vector3(0, -1,0));
            }
            else if (transform.position.y < minHeight)
            {
                rb.AddForce(new Vector3(0, 1, 0));
            }
            else
            {
                int dir = invertScroll ? 1 : -1;
                // Map the angle by the height
                transform.eulerAngles = new Vector3(Remap(transform.position.y, minHeight, maxHeight - 5, minAngle, maxAngle), transform.eulerAngles.y, 0);
                if (rb.velocity.y < 10 && rb.velocity.y > -10)
                    rb.AddForce(new Vector3(0, dir * Mathf.Clamp(Input.GetAxis("Zoom"), -0.5f, 0.5f) * scrollSpeed * 30, 0));
            }
        }
        
        float dist = Vector3.Magnitude(transform.position);
        centerButton.gameObject.SetActive(dist > 50);
        if (dist < 50) return;
        if (dist < 80) centerButtonText.text = "Return to Town";
        else if (dist < 120) centerButtonText.text = "Please, Return to Town";
        else if (dist < 160) centerButtonText.text = "There's nothing here";
        else
        {
            StartCoroutine(ManualCenter());
            centerButtonText.text = "Fine, I'll do it myself";
        }
    }

    public void Center()
    {
        transform.position = startPos;
        transform.eulerAngles = startRot;
    }

    IEnumerator ManualCenter()
    {
        crRunning = true;
        yield return new WaitForSeconds(2);
        Center();
        crRunning = false;
    }
    
    public static float Remap (float value, float min1, float max1, float min2, float max2) {
        return Mathf.Clamp((value - min1) / (max1 - min1) * (max2 - min2) + min2, min2, max2);
    }
}
