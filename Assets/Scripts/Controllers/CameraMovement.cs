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
        private float scrollAcceleration;
        private float scrollAccelerationRef = 0;
        private Vector3 followVelRef = Vector3.zero;

        private Vector3 _dragOrigin, _cameraOrigin, _rotateAxis, startPos, startRot;
        private Vector3 lastDrag;
        private bool _dragging, _rotating;

        public static Action OnCameraMove;

        [SerializeField] private float maxScrollSpeed = 0.1f;
        [SerializeField] private float maxDragSpeed = 1f;
        [SerializeField] private float dragAcceleration = 0.1f;
        [SerializeField] private float scrollAccelerationSpeed = 0.1f;
        [SerializeField] private float bounceTime = 0.1f;
        [SerializeField] private PostProcessProfile profile;
        [SerializeField] private PostProcessVolume volume;
        [SerializeField] private LayerMask layerMask;
        [SerializeField] private float DoFAdjustMultiplier = 5f;


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
            freeLook.Follow.position += dragDir * maxDragSpeed;

            if (Input.GetMouseButtonUp(0))
            {
                _dragging = false;
            }

            if (!_dragging)
            {
                dragDir = Vector3.SmoothDamp(dragDir, Vector3.zero, ref vel, dragAcceleration);
            }

            float scroll = -Input.mouseScrollDelta.y;
            scrollAcceleration += scroll * Time.deltaTime;
            scrollAcceleration = Mathf.SmoothDamp(scrollAcceleration, 0, ref scrollAccelerationRef, scrollAccelerationSpeed);
            scrollAcceleration = Mathf.Clamp(scrollAcceleration, -maxScrollSpeed, maxScrollSpeed);
            freeLook.m_YAxis.Value += scrollAcceleration;

            volume.weight = Mathf.Lerp(1, 0, freeLook.m_YAxis.Value);
            var DoFRay = _cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            if (Physics.Raycast(DoFRay, out var hit, 100f, layerMask))
            {
                _depthOfField.focusDistance.value = Mathf.MoveTowards(_depthOfField.focusDistance.value, hit.distance, Time.deltaTime * DoFAdjustMultiplier);
            }

            bool atLimit = freeLook.m_YAxis.Value <= 0.01 | freeLook.m_YAxis.Value >= 0.98;
            if (atLimit && Mathf.Abs(scrollAcceleration) > 0)
                freeLook.Follow.position += new Vector3(0, scrollAcceleration, 0);

            float clampedY = Mathf.Clamp(freeLook.Follow.position.y, -1, 2);
            freeLook.Follow.position = new Vector3(freeLook.Follow.position.x, clampedY, freeLook.Follow.position.z);

            Vector3 newFollowPos =  new Vector3(freeLook.Follow.position.x, 1, freeLook.Follow.position.z);
            freeLook.Follow.position = Vector3.SmoothDamp(freeLook.Follow.position, newFollowPos, ref followVelRef, bounceTime);
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
