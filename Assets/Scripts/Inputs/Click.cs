﻿using System;
using Managers;
using UnityEngine;
using static Managers.GameManager;

namespace Inputs
{
    //TODO: Move this into the Input Controller
    public class Click : MonoBehaviour
    {
        public static bool PlacingBuilding;

        [Range(0.1f,0.5f)]
        public float clickSpeed = 0.2f;

        public static Action OnLeftClick;
        public static Action OnRightClick;
    
        private float _time0, _time1;

        private void Start()
        {
            Manager.Inputs.IA_OnLeftClick.performed += I_OnLeftClick;
            Manager.Inputs.IA_OnLeftClick.canceled += I_OnLeftClick;
            Manager.Inputs.IA_OnRightClick.performed += I_OnRightClick;
            Manager.Inputs.IA_OnRightClick.canceled += I_OnRightClick;
        }

        private void I_OnLeftClick(UnityEngine.InputSystem.InputAction.CallbackContext obj)
        {
            if (!Manager.State.InGame) return;

            if (obj.performed) _time0 = Time.time;
            if(obj.canceled && Time.time - _time0 < clickSpeed) OnLeftClick?.Invoke();
            if (obj.canceled) PlacingBuilding = false;
        }

        private void I_OnRightClick(UnityEngine.InputSystem.InputAction.CallbackContext obj)
        {
            if (!Manager.State.InGame) return;

            if (obj.performed) _time1 = Time.time;
            if (obj.canceled && Time.time - _time1 < clickSpeed) OnRightClick?.Invoke();
        }

        private void OnDestroy()
        {
            OnLeftClick = null;
            OnRightClick = null;
        }
    }
}