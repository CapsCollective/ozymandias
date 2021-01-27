﻿using System;
using UnityEngine;

namespace Utilities
{
    public class Click : MonoBehaviour
    {
        [Range(0.1f,0.5f)]
        public float clickSpeed = 0.2f;

        public static Action OnLeftClick;
        public static Action OnRightClick;
    
        private float _time0, _time1;

        private void LateUpdate()
        {
            if (Input.GetMouseButtonDown(0)) _time0 = Time.time;
            if (Input.GetMouseButtonUp(0) && Time.time - _time0 < clickSpeed) OnLeftClick?.Invoke();
        
            if (Input.GetMouseButtonDown(1)) _time1 = Time.time;
            if (Input.GetMouseButtonUp(1) && Time.time - _time1 < clickSpeed) OnRightClick?.Invoke();
        }

        private void OnDestroy()
        {
            OnLeftClick = null;
            OnRightClick = null;
        }
    }
}