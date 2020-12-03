using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;
using static GameManager;

namespace Controllers
{
    public class CameraMovement : MonoBehaviour
    {
        private Rigidbody rb;
        private Camera cam;
        private Vector3 dragOrigin, cameraOrigin, rotateAxis;
        private float rotateOrigin;
        private bool dragging, rotating;

        public Vector3 startPos, startRot;
        public Button centerButton;
        public TextMeshProUGUI centerButtonText;
        public GameObject dummyCursor;
        public Canvas canvas;
        private bool crRunning;
        
        #pragma warning disable 0649
        [SerializeField] private PostProcessProfile profile;
        [SerializeField] private PostProcessVolume volume;
        [SerializeField] private LayerMask layerMask;
        private DepthOfField depthOfField;
        
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
            cam = GetComponent<UnityEngine.Camera>();
            rb = GetComponent<Rigidbody>();
            profile.TryGetSettings(out depthOfField);
        }

        void Update()
        {
            if (Manager.inMenu) return;
            
            volume.weight = Mathf.InverseLerp(maxHeight,minHeight, transform.position.y);
            Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100f, layerMask))
            {
                depthOfField.focusDistance.value = hit.distance;
            }
            
            if (!rotating && Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
            {
                dragOrigin = Input.mousePosition;
                cameraOrigin = transform.position;
                dragging = true;
            }
            else if (!dragging && Input.GetMouseButtonDown(1) && !EventSystem.current.IsPointerOverGameObject())
            {
                rotateAxis = hit.point;
                rotating = true;
            }
        
            if (!Input.GetMouseButton(0)) dragging = false;
            if (!Input.GetMouseButton(1)) rotating = false;

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
                    Transform t = transform;
                    t.eulerAngles = new Vector3(Remap(t.position.y, minHeight, maxHeight - 5, minAngle, maxAngle), t.eulerAngles.y, 0);
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
            else if (!crRunning)
            {
                centerButtonText.text = "Fine, I'll do it myself";
                StartCoroutine(ManualCenter());
            }
        }

        public void Center()
        {
            Transform t = transform;
            t.position = startPos;
            t.eulerAngles = startRot;
        }

        private IEnumerator ManualCenter()
        {
            crRunning = true;
            yield return new WaitForSeconds(1);
            dummyCursor.SetActive(true);

            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, 
                Input.mousePosition, canvas.worldCamera, out var currentPos);
            currentPos = canvas.transform.TransformPoint(currentPos);

            var endPos = centerButton.transform.position;
            dummyCursor.transform.position = currentPos;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            while (Vector2.Distance(currentPos, endPos) > 10f)
            {
                currentPos = Vector3.Lerp(currentPos, endPos, Time.deltaTime * 1.5f);
                dummyCursor.transform.position = currentPos;
                yield return null;
            }
        
            Jukebox.Instance.PlayClick();
            Center();
            dummyCursor.SetActive(false);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            crRunning = false;
        }

        public static float Remap (float value, float min1, float max1, float min2, float max2) {
            return Mathf.Clamp((value - min1) / (max1 - min1) * (max2 - min2) + min2, min2, max2);
        }
    }
}
