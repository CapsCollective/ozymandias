using System;
using Cinemachine;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Managers;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Utilities;
using static Managers.GameManager;

namespace Inputs
{
    public class CameraMovement : MonoBehaviour
    {
        public static Action OnPan, OnRotate, OnZoom;
        
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
        [SerializeField] private Volume dofVolume;
        [SerializeField] private Volume mainVolume;
        [Header("Clamping")]
        [SerializeField] private Vector3 clampCenterPos;
        [SerializeField] private float clampDistance;
        [SerializeField] private float pushForce = 5.0f;
        [SerializeField] private float dotMultiplier = 2.0f;
        [SerializeField] private float dot2Multiplier = 0.5f;
        [SerializeField] private bool showDebugSphere = false;
        [SerializeField] private Mesh debugMesh;
        [SerializeField] private Vector3 waterfallDir;
        [SerializeField] private Material debugMaterial;
        private void Awake()
        {
            _cam = GetComponent<Camera>();
            dofVolume.sharedProfile.TryGet<DepthOfField>(out _depthOfField);
            FreeLook = GetComponent<CinemachineFreeLook>();
        }

        private void Start()
        {
            Manager.Inputs.RightMouse.started += RightClick;
            Manager.Inputs.RightMouse.canceled += RightClick;
            Manager.Inputs.LeftMouse.started += LeftClick;
            Manager.Inputs.LeftMouse.canceled += LeftClick;
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
            if (Globals.RestartingGame) return;
            
            // Depth of Field stuff
            dofVolume.weight = Mathf.Lerp(1, 0, FreeLook.m_YAxis.Value);
            
            if (!Manager.State.InGame || Manager.Tooltip.NavigationActive) return;

            float rotation = Manager.Inputs.RotateCamera.ReadValue<float>();
            if (rotation != 0)
            {
                OnRotate?.Invoke();
                var keyboardOrMouse = Inputs.DeviceIsKeyboard(Manager.Inputs.RotateCamera) ? Time.deltaTime : .5f;
                FreeLook.m_XAxis.Value += rotation * keyboardOrMouse;
            }

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

            Vector2 inputDir = Manager.Inputs.MoveCamera.ReadValue<Vector2>() * Time.deltaTime;
            inputDir += dragDir;
            if (inputDir.sqrMagnitude > 0.0002f)
            {
                OnPan?.Invoke();
            }
            Vector3 crossFwd = Vector3.Cross(transform.right, Vector3.up);
            Vector3 crossSide = Vector3.Cross(transform.up, transform.forward);
            FreeLook.Follow.Translate(((crossFwd * inputDir.y) + (crossSide * inputDir.x)));
            FreeLook.Follow.GetChild(0).forward = crossFwd;

            // Scrolling
            float scroll = -Manager.Inputs.ZoomCamera.ReadValue<float>() * Time.deltaTime;
            if(scroll != 0) OnZoom?.Invoke();
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
            float dot2 = Vector3.Dot((clampCenterPos - waterfallDir), (FreeLook.Follow.position - clampCenterPos)) * dot2Multiplier;
            float distanceFromBorder = Vector3.Distance(FreeLook.Follow.position, clampCenterPos) / clampDistance;
            if (dot > 0)
                distanceFromBorder -= dot;
            if (dot2 > 0)
                distanceFromBorder -= dot2;
            if(distanceFromBorder > 1.0f)
                FreeLook.Follow.position += ((clampCenterPos - FreeLook.Follow.position).normalized * (distanceFromBorder - 1.0f) * pushForce) * Time.deltaTime;
        }

        public TweenerCore<Vector3,Vector3,VectorOptions> MoveTo(Vector3 pos, float duration = 0.5f)
        {
            return FreeLook.Follow.transform.DOMove(pos + Vector3.up, duration);
        }
        
        public struct CameraMove
        {
            public Vector3 Position;
            public float OrbitHeight;
            public float XAxisValue;
            public float YAxisValue;

            public CameraMove(Vector3 pos, float orbitHeight, float xVal, float yVal)
            {
                Position = pos;
                OrbitHeight = orbitHeight;
                XAxisValue = xVal;
                YAxisValue = yVal;
            }
        }
        
        private const float MoveEpsilon = 0.05f;
        private float _startMoveTime;
        private CameraMove _startRig;

        public bool MoveCamRig(CameraMove cameraMove, AnimationCurve curve, float length = 3.0f)
        {
            // Set these values on first run
            if (Mathf.Approximately(_startMoveTime, 0))
            {
                _startMoveTime = Time.time;
                
                // Store the initial rig position values
                Vector3 followPos = FreeLook.Follow.position;
                _startRig.Position = new Vector3(followPos.x, followPos.y, followPos.z);
                _startRig.OrbitHeight = FreeLook.m_Orbits[1].m_Height;
                _startRig.XAxisValue = FreeLook.m_XAxis.Value;
                _startRig.YAxisValue = FreeLook.m_YAxis.Value;
            }

            var elapsedTime = Time.time - _startMoveTime;
            var time = elapsedTime / length;
            time = curve.Evaluate(time);

            // Lerp follow position
            cameraMove.Position.y = 1;
            FreeLook.Follow.position = Vector3.Lerp(
                _startRig.Position, cameraMove.Position, time);

            // Lerp camera orbit
            FreeLook.m_Orbits[1].m_Height = Mathf.Lerp(
                _startRig.OrbitHeight, cameraMove.OrbitHeight, time);
            
            // Lerp camera X axis
            FreeLook.m_XAxis.Value =  Mathf.Lerp(
                _startRig.XAxisValue, cameraMove.XAxisValue, time);
            
            // Lerp camera Y axis
            FreeLook.m_YAxis.Value =  Mathf.Lerp(
                _startRig.YAxisValue, cameraMove.YAxisValue, time);
            
            if ((cameraMove.Position - FreeLook.Follow.position).magnitude >= MoveEpsilon) return false;
            FreeLook.Follow.position = cameraMove.Position;
            FreeLook.m_Orbits[1].m_Height = cameraMove.OrbitHeight;
            FreeLook.m_XAxis.Value = cameraMove.XAxisValue;
            FreeLook.m_YAxis.Value = cameraMove.YAxisValue;
            MoveCamRigCancel();
            return true;
        }

        public void MoveCamRigCancel()
        {
            _startMoveTime = 0;
        }

        public void SetCamRig(CameraMove cameraMove)
        {
            FreeLook.Follow.position = new Vector3(cameraMove.Position.x, 1, cameraMove.Position.z);
            FreeLook.m_Orbits[1].m_Height = cameraMove.OrbitHeight;
            FreeLook.m_XAxis.Value = cameraMove.XAxisValue;
            FreeLook.m_YAxis.Value = cameraMove.YAxisValue;
        }

        private void StartCursorGrab()
        {
            Manager.Cursor.Current = CursorType.Grab;
            IsMoving = true;
        }
        
        private void EndCursorGrab()
        {
            Manager.Cursor.Current = Manager.Cards.SelectedCard && Manager.Cards.SelectedCard.Interactable ? CursorType.Build : CursorType.Pointer;
            IsMoving = false;
        }

        private void OnDrawGizmos()
        {
            var dir = (Vector3.zero - waterfallDir).normalized;
            Gizmos.DrawLine(Vector3.zero, dir);
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
