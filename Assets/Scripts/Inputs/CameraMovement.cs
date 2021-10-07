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
        private Quaternion startRot;
        private bool _dragging, _rotating;
        
        [SerializeField] private Vector2 dragSpeed;
        [SerializeField] private float dragAcceleration = 0.1f;
        [SerializeField] private float scrollAccelerationSpeed = 0.1f;
        [SerializeField] private float bounceTime = 0.1f;
        [SerializeField] private PostProcessProfile profile;
        [SerializeField] private PostProcessVolume volume;
        [SerializeField] private LayerMask layerMask;
        [Header("Clamping")]
        [SerializeField] private Vector3 clampCenterPos;
        [SerializeField] private float clampDistance;
        [SerializeField] private float pushForce = 5.0f;
        [SerializeField] private float dotMultiplier = 2.0f;
        [SerializeField] private bool showDebugSphere = false;
        [SerializeField] private Mesh debugMesh;
        [SerializeField] private Material debugMaterial;

        private void Awake()
        {
            _cam = GetComponent<Camera>();
            profile.TryGetSettings(out _depthOfField);
            FreeLook = GetComponent<CinemachineFreeLook>();
        }

        private void Start()
        {
            Manager.Inputs.OnRightMouse.started += RightClick;
            Manager.Inputs.OnRightMouse.canceled += RightClick;
            Manager.Inputs.OnLeftMouse.started += LeftClick;
            Manager.Inputs.OnLeftMouse.canceled += LeftClick;
        }

        private void RightClick(InputAction.CallbackContext context)
        {
            if (context.started) StartCursorGrab();
            else if (context.canceled)
            {
                FreeLook.m_XAxis.m_InputAxisValue = 0;
                EndCursorGrab();
            }
        }

        private void LeftClick(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                leftClick = true;
                lastDrag = Manager.Inputs.MousePosition;
            }
            else if (context.canceled)
            {
                leftClick = false;
                _dragging = false;
                EndCursorGrab();
            }
        }

        private void Update()
        {
            // Depth of Field stuff
            volume.weight = Mathf.Lerp(1, 0, FreeLook.m_YAxis.Value);
            
            if (!Manager.State.InGame) return;

            FreeLook.m_XAxis.m_InputAxisValue = -Manager.Inputs.OnRotateCamera.ReadValue<float>();

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

            Vector2 inputDir = Manager.Inputs.OnMoveCamera.ReadValue<Vector2>() + dragDir;
            Vector3 crossFwd = Vector3.Cross(transform.right, Vector3.up);
            Vector3 crossSide = Vector3.Cross(transform.up, transform.forward);
            FreeLook.Follow.Translate(((crossFwd * inputDir.y) + (crossSide * inputDir.x)) * 0.01f);

            // Scrolling
            float scroll = -Manager.Inputs.OnZoomCamera.ReadValue<float>();
            scrollAcceleration += scroll * Time.deltaTime;
            scrollAcceleration = Mathf.SmoothDamp(scrollAcceleration, 0, ref scrollAccelerationRef, scrollAccelerationSpeed);
            FreeLook.m_YAxis.Value += scrollAcceleration;

            // Bounciness stuff
            bool atLimit = FreeLook.m_YAxis.Value <= 0.01 | FreeLook.m_YAxis.Value >= 0.98;
            if (atLimit && Mathf.Abs(scrollAcceleration) > 0)
                FreeLook.Follow.position += new Vector3(0, scrollAcceleration, 0);

            float clampedY = Mathf.Clamp(FreeLook.Follow.position.y, -1, 2);
            FreeLook.Follow.position = new Vector3(FreeLook.Follow.position.x, clampedY, FreeLook.Follow.position.z);

            Vector3 newFollowPos = new Vector3(FreeLook.Follow.position.x, 1, FreeLook.Follow.position.z);
            FreeLook.Follow.position = Vector3.SmoothDamp(FreeLook.Follow.position, newFollowPos, ref followVelRef, bounceTime);

            float dot = Vector3.Dot(Vector3.back, (FreeLook.Follow.position - clampCenterPos).normalized) * dotMultiplier;
            float distanceFromBorder = Vector3.Distance(FreeLook.Follow.position, clampCenterPos) / clampDistance;
            if (dot > 0)
                distanceFromBorder -= dot;
            if(distanceFromBorder > 1.0f)
                    FreeLook.Follow.position += ((clampCenterPos - FreeLook.Follow.position).normalized * (distanceFromBorder - 1.0f) * pushForce) * Time.deltaTime;
        }

        public TweenerCore<Vector3,Vector3,VectorOptions> MoveTo(Vector3 pos, float duration = 0.5f)
        {
            return FreeLook.Follow.transform.DOMove(pos, duration);
        }

        private void StartCursorGrab()
        {
            Manager.Cursor.Current = CursorType.Grab;
            IsMoving = true;
        }
        
        private void EndCursorGrab()
        {
            Manager.Cursor.Current = Manager.Cards.SelectedCard ? CursorType.Build : CursorType.Pointer;
            IsMoving = false;
        }

        private void OnDrawGizmos()
        {
            if (!showDebugSphere) return;
            Gizmos.DrawWireSphere(clampCenterPos, clampDistance);
            if (debugMaterial != null && debugMesh != null)
            {
                debugMaterial.SetFloat("_Distance", clampDistance);
                debugMaterial.SetFloat("_DotMultiplier", dotMultiplier);
                Graphics.DrawMesh(debugMesh, Matrix4x4.TRS(clampCenterPos - new Vector3(0, 0, clampDistance * 0.5f),
                    Quaternion.Euler(90, 0, 0), 
                    Vector3.one * (clampDistance * 2) * (dotMultiplier * 2)),
                    debugMaterial, 0);
            }
        }
    }
}
