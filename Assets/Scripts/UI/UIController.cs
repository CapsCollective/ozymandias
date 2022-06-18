using System;
using System.Collections.Generic;
using Inputs;
using UnityEngine;
using UnityEngine.InputSystem;
using Utilities;
using static Managers.GameManager;

namespace UI
{
    public class UIController : MonoBehaviour
    {
        public static Action<GameObject, bool> OnUIOpen;
        public static Action OnUIClose;

        [SerializeField] private bool showCursor = true;
        [SerializeField] private GameObject firstSelected;
        private GameObject currentSelected;

        [SerializeField] private SerializedDictionary<GameObject, Vector2> cursorOffsetOverrides = new SerializedDictionary<GameObject, Vector2>();

        private void Awake()
        {
            foreach (KeyValuePair<GameObject, Vector2> pair in cursorOffsetOverrides)
            {
                InputHelper.CursorOffsetOverrides.Add(pair.Key, pair.Value);
            }
        }

        public virtual void OnOpen()
        {
            Debug.Log("Opened " + name);
            Inputs.Inputs.OnControlChange += ControllerFocus;
            OnUIOpen?.Invoke(firstSelected, showCursor);
        }

        public virtual void OnClose()
        {
            Debug.Log("Closed " + name);
            Inputs.Inputs.OnControlChange -= ControllerFocus;
            OnUIClose?.Invoke();
        }

        private void ControllerFocus(InputControlScheme scheme)
        {
            if(scheme == Manager.Inputs.PlayerInput.ControllerScheme)
            {
                OnUIOpen?.Invoke(firstSelected, showCursor);
            }
        }
    }
}
