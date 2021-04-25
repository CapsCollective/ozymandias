
﻿using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.PostProcessing;
using static Managers.GameManager;

namespace Controllers
{
    public class CameraMovement : MonoBehaviour
    {
        private Rigidbody _rb;
        private Camera _cam;
        private DepthOfField _depthOfField;

        private Vector3 _dragOrigin, _cameraOrigin, _rotateAxis;
        private float _rotateOrigin;
        private bool _dragging, _rotating;

        public static Action OnCameraMove;

        [SerializeField] private Vector3 startPos, startRot;
        [SerializeField] private PostProcessProfile profile;
        [SerializeField] private PostProcessVolume volume;
        [SerializeField] private LayerMask layerMask;

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
        }

        private void Update()
        {
            if (Manager.inMenu) return;

            volume.weight = Mathf.InverseLerp(maxHeight, minHeight, transform.position.y);
            var ray = _cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            if (Physics.Raycast(ray, out var hit, 100f, layerMask))
            {
                _depthOfField.focusDistance.value = hit.distance;
            }

            if (!_rotating && Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
            {
                _dragOrigin = Input.mousePosition;
                _cameraOrigin = transform.position;
                _dragging = true;
            }
            else if (!_dragging && Input.GetMouseButtonDown(1) && !EventSystem.current.IsPointerOverGameObject())
            {
                _rotateAxis = hit.point;
                _rotating = true;
            }

            if (!Input.GetMouseButton(0)) _dragging = false;
            if (!Input.GetMouseButton(1)) _rotating = false;

            if (_dragging)
            {
                OnCameraMove?.Invoke();
                var dir = _cam.ScreenToViewportPoint(_dragOrigin - Input.mousePosition);
                var t = transform;
                var pos = t.position;
                t.position = _cameraOrigin +
                             Quaternion.Euler(0, t.eulerAngles.y, 0) *
                             new Vector3(dir.x * dragSpeed * pos.y, 0, dir.y * dragSpeed * pos.y);
            }
            else if (_rotating)
            {
                transform.RotateAround(_rotateAxis, Vector3.up, Input.GetAxis("Mouse X") * rotateSpeed);
            }
            else
            {
                if (transform.position.y > maxHeight)
                {
                    _rb.AddForce(new Vector3(0, -2.0f,0));
                }
                else if (transform.position.y < minHeight)
                {
                    _rb.AddForce(new Vector3(0, 2.0f, 0));
                }
                else
                {
                    var dir = invertScroll ? 1 : -1;
                    // Map the angle by the height
                    var t = transform;
                    t.eulerAngles = new Vector3(
                        Remap(t.position.y,
                            minHeight,
                            maxHeight - 5,
                            minAngle,
                            maxAngle), t.eulerAngles.y, 0);
                    if (_rb.velocity.y < 10 && _rb.velocity.y > -10)
                    {
                        Vector3 force = new Vector3(0,
                            dir * Mathf.Clamp(Input.GetAxis("Zoom"), -0.5f, 0.5f) * scrollSpeed * 30, 0);
                        if (force != Vector3.zero) _rb.AddForce(force);
                    }
                }
            }
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
