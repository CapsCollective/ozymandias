using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.PostProcessing;
using static Managers.GameManager;
using Cinemachine;

namespace Controllers
{
    public class CameraMovement : MonoBehaviour
    {
        public static bool Moving;
        
        private Rigidbody _rb;
        private Camera _cam;
        private DepthOfField _depthOfField;
        private CinemachineFreeLook freeLook;
        private Vector3 dragDir;
        private Vector3 vel = Vector3.zero;

        private Vector3 _dragOrigin, _cameraOrigin, _rotateAxis;
        private Vector3 lastDrag;
        private bool _dragging, _rotating;

        public static Action OnCameraMove;

        [SerializeField] private float dragAcceleration = 0.1f;
        [SerializeField] private Vector3 startPos, startRot;
        [SerializeField] private PostProcessProfile profile;
        [SerializeField] private PostProcessVolume volume;
        [SerializeField] private LayerMask layerMask;
        [SerializeField] private float DoFAdjustMultiplier = 5f;
        [SerializeField] private float sensitivity = 5f;

        [Range(1,10)]
        [SerializeField] private int
            dragSpeed = 3,
            scrollSpeed = 3,
            rotateSpeed = 3;
    
        [SerializeField] private int
            minHeight = 5,
            maxHeight = 25,
            minAngle = 25,
            maxAngle = 60;

        [SerializeField] private bool invertScroll;

        private void Awake()
        {
            _cam = GetComponent<Camera>();
            _rb = GetComponent<Rigidbody>();
            profile.TryGetSettings(out _depthOfField);
            freeLook = GetComponent<CinemachineFreeLook>();
        }

        private void Update()
        {
            if (Manager.inMenu) return;

            if (Input.GetMouseButton(1))
            {
                freeLook.m_XAxis.m_InputAxisValue = -Input.GetAxis("Mouse X");
            }
            else if (Input.GetMouseButtonUp(1))
            {
                freeLook.m_XAxis.m_InputAxisValue = 0;
            }

            Ray posRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit posHit;

            if (Physics.Raycast(posRay, out posHit, 1000f, LayerMask.GetMask("Ocean")))
            {
                if (Input.GetMouseButtonDown(0))
                {
                    _dragging = true;
                    lastDrag = posHit.point;
                }

                if (Input.GetMouseButton(0))
                {
                    dragDir = lastDrag - posHit.point;
                    dragDir.y = 0;
                }
            }
            freeLook.Follow.position += dragDir;

            if (Input.GetMouseButtonUp(0))
            {
                _dragging = false;
            }

            if (!_dragging)
            {
                dragDir = Vector3.SmoothDamp(dragDir, Vector3.zero, ref vel, dragAcceleration);
            }


            volume.weight = Mathf.Lerp(1, 0, freeLook.m_YAxis.Value);//Mathf.InverseLerp(freeLook.m_Orbits[0].m_Height, freeLook.m_Orbits[2].m_Height, transform.position.y);
            var DoFRay = _cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            if (Physics.Raycast(DoFRay, out var hit, 100f, layerMask))
            {
                _depthOfField.focusDistance.value = Mathf.MoveTowards(_depthOfField.focusDistance.value, hit.distance, Time.deltaTime * DoFAdjustMultiplier);
            }

                //if (!_rotating && Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
                //{
                //    _dragOrigin = Input.mousePosition;
                //    _cameraOrigin = transform.position;
                //    _dragging = true;
                //}
                //else if (!_dragging && Input.GetMouseButtonDown(1) && !EventSystem.current.IsPointerOverGameObject())
                //{
                //    _rotateAxis = hit.point;
                //    _rotating = true;
                //}

                //if (!Input.GetMouseButton(0)) _dragging = false;
                //if (!Input.GetMouseButton(1)) _rotating = false;

                //Moving = _rotating || _dragging;

                //if (_dragging)
                //{
                //    var dragLength = _dragOrigin - Input.mousePosition;
                //    if (dragLength.magnitude > 1.0f) OnCameraMove?.Invoke();

                //    var dir = _cam.ScreenToViewportPoint(dragLength);
                //    var t = transform;
                //    var pos = t.position;
                //    t.position = _cameraOrigin +
                //                 Quaternion.Euler(0, t.eulerAngles.y, 0) *
                //                 new Vector3(dir.x * dragSpeed * pos.y, 0, dir.y * dragSpeed * pos.y);
                //}
                //else if (_rotating)
                //{
                //    transform.RotateAround(_rotateAxis, Vector3.up, Input.GetAxis("Mouse X") * rotateSpeed);
                //}
                //else
                //{
                //    if (transform.position.y > maxHeight)
                //    {
                //        _rb.AddForce(new Vector3(0, -2.0f, 0));
                //    }
                //    else if (transform.position.y < minHeight)
                //    {
                //        _rb.AddForce(new Vector3(0, 2.0f, 0));
                //    }
                //    else
                //    {
                //        var dir = invertScroll ? 1 : -1;
                //        // Map the angle by the height
                //        var t = transform;
                //        t.eulerAngles = new Vector3(
                //            Remap(t.position.y,
                //                minHeight,
                //                maxHeight - 5,
                //                minAngle,
                //                maxAngle), t.eulerAngles.y, 0);
                //        if (_rb.velocity.y < 10 && _rb.velocity.y > -10)
                //        {
                //            Vector3 force = new Vector3(0,
                //                dir * Mathf.Clamp(Input.GetAxis("Zoom"), -0.5f, 0.5f) * scrollSpeed * 30, 0);
                //            if (force != Vector3.zero) _rb.AddForce(force);
                //        }
                //    }
                //}

            }

        public void Center()
        {
            var t = transform;
            t.position = startPos;
            t.eulerAngles = startRot;
        }

        private static float Remap (float value, float min1, float max1, float min2, float max2) {
            return Mathf.Clamp((value - min1) / (max1 - min1) * (max2 - min2) + min2, min2, max2);
        }
    }
}
