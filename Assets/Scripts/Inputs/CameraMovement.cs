using System;
using Cinemachine;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.PostProcessing;
using Utilities;
using static Managers.GameManager;

namespace Inputs
{
    public class CameraMovement : MonoBehaviour
    {
        public static bool IsMoving;
        public CinemachineFreeLook FreeLook { get; private set; }
        
        private Camera _cam;
        private DepthOfField _depthOfField;

        private Vector2 dragDir;
        private Vector3 vel = Vector3.zero;
        private float scrollAcceleration;
        private float scrollAccelerationRef = 0;
        private Vector3 followVelRef = Vector3.zero;
        private bool leftClick, rightClick;
        private RaycastHit posHit;

        private Vector2 lastDrag;
        private Vector3 startPos;
        private Quaternion startRot;
        private bool _dragging, _rotating;
        
        private CursorType _previousCursor = CursorType.Pointer;

        public static Action OnCameraMove;

        [SerializeField] private Vector2 dragSpeed;
        [SerializeField] private float dragAcceleration = 0.1f;
        [SerializeField] private float scrollAccelerationSpeed = 0.1f;
        [SerializeField] private float bounceTime = 0.1f;
        [SerializeField] private PostProcessProfile profile;
        [SerializeField] private PostProcessVolume volume;
        [SerializeField] private LayerMask layerMask;
        [SerializeField] private float DoFAdjustMultiplier = 5f;

        private void Awake()
        {
            _cam = GetComponent<Camera>();
            profile.TryGetSettings(out _depthOfField);
            FreeLook = GetComponent<CinemachineFreeLook>();
        }

        private void Start()
        {
            Manager.Inputs.IA_OnRightClick.performed += RightClick;
            Manager.Inputs.IA_OnRightClick.canceled += RightClick;
            Manager.Inputs.IA_OnLeftClick.performed += LeftClick;
            Manager.Inputs.IA_OnLeftClick.canceled += LeftClick;
            startPos = transform.position;
        }

        private void RightClick(InputAction.CallbackContext context)
        {
            if (context.performed) StartCursorGrab();
            else if (context.canceled)
            {
                FreeLook.m_XAxis.m_InputAxisValue = 0;
                EndCursorGrab();
            }
        }

        private void LeftClick(InputAction.CallbackContext context)
        {
            leftClick = context.performed;
            if (context.performed)
            {
                lastDrag = Manager.Inputs.MousePosition;
            }
            else if (context.canceled)
            {
                _dragging = false;
                EndCursorGrab();
            }
        }

        private void Update()
        {
            if (!Manager.State.InGame) return;

            FreeLook.m_XAxis.m_InputAxisValue = -Manager.Inputs.IA_RotateCamera.ReadValue<float>();

            if (leftClick)
            {
                Vector2 dir = _cam.ScreenToViewportPoint(lastDrag) - _cam.ScreenToViewportPoint(Manager.Inputs.MousePosition);
                if (dir.sqrMagnitude > 0.0002f)
                {
                    StartCursorGrab();
                    _dragging = true;
                }

                if (_dragging)
                {
                    dragDir = dir * Mathf.Lerp(dragSpeed.x, dragSpeed.y, FreeLook.m_YAxis.Value);
                    lastDrag = Manager.Inputs.MousePosition;
                }
            }

            if (!_dragging)
            {
                dragDir = Vector3.SmoothDamp(dragDir, Vector3.zero, ref vel, dragAcceleration);
            }

            Vector2 inputDir = Manager.Inputs.IA_MoveCamera.ReadValue<Vector2>() + dragDir;
            Vector3 crossFwd = Vector3.Cross(transform.right, Vector3.up);
            Vector3 crossSide = Vector3.Cross(transform.up, transform.forward);
            FreeLook.Follow.Translate(((crossFwd * inputDir.y) + (crossSide * inputDir.x)) * 0.01f);

            // Scrolling
            float scroll = -Manager.Inputs.IA_OnScroll.ReadValue<float>();
            scrollAcceleration += scroll * Time.deltaTime;
            scrollAcceleration = Mathf.SmoothDamp(scrollAcceleration, 0, ref scrollAccelerationRef, scrollAccelerationSpeed);
            FreeLook.m_YAxis.Value += scrollAcceleration;

            // Depth of Field stuff
            volume.weight = Mathf.Lerp(1, 0, FreeLook.m_YAxis.Value);
            var DoFRay = _cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            if (Physics.Raycast(DoFRay, out var hit, 100f, layerMask))
            {
                _depthOfField.focusDistance.value = Mathf.MoveTowards(_depthOfField.focusDistance.value, hit.distance, Time.deltaTime * DoFAdjustMultiplier);
            }

            // Bounciness stuff
            bool atLimit = FreeLook.m_YAxis.Value <= 0.01 | FreeLook.m_YAxis.Value >= 0.98;
            if (atLimit && Mathf.Abs(scrollAcceleration) > 0)
                FreeLook.Follow.position += new Vector3(0, scrollAcceleration, 0);

            float clampedY = Mathf.Clamp(FreeLook.Follow.position.y, -1, 2);
            FreeLook.Follow.position = new Vector3(FreeLook.Follow.position.x, clampedY, FreeLook.Follow.position.z);

            Vector3 newFollowPos = new Vector3(FreeLook.Follow.position.x, 1, FreeLook.Follow.position.z);
            FreeLook.Follow.position = Vector3.SmoothDamp(FreeLook.Follow.position, newFollowPos, ref followVelRef, bounceTime);
        }

        public TweenerCore<Vector3,Vector3,VectorOptions> MoveTo(Vector3 pos, float duration = 0.5f)
        {
            return FreeLook.Follow.transform.DOMove(pos, duration);
        }

        private void StartCursorGrab()
        {
            if (Manager.Cursor.Current == CursorType.Grab) return;
            _previousCursor = Manager.Cursor.Current;
            Manager.Cursor.Current = CursorType.Grab;
        }
        
        private void EndCursorGrab()
        {
            //TODO: Causing bug where cursor being set to build incorrectly
            Manager.Cursor.Current = _previousCursor;
        }
    }
}
