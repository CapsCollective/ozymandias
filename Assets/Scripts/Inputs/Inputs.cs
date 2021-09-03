using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

namespace Inputs
{
    public class Inputs
    {
        public bool UsingController => ControlScheme == PlayerInput.ControllerScheme;
        public Vector2 MousePosition 
        {
            get
            {
                if(!UsingController)
                    return OnMousePosition.ReadValue<Vector2>();
                return Camera.main.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
            }
        }

        public Action<InputControlScheme> OnControlChange;

        public InputControlScheme ControlScheme;

        // Player Input
        public PlayerInput PlayerInput { get; private set; }
        public InputAction OnLeftClick { get; private set; }
        public InputAction OnRightClick { get; private set; }
        public InputAction OnScroll { get; private set; }
        public InputAction OnRotateCamera { get; private set; }
        public InputAction OnMousePosition { get; private set; }
        public InputAction OnMoveCamera { get; private set; }
        public InputAction OnDeleteBuilding { get; private set; }
        public InputAction OnNextTurn { get; private set; }
        public InputAction OnShowPause { get; private set; }
        public InputAction OnRotateBuilding { get; private set; }
        public InputAction OnDeselectCards { get; private set; }

        // UI Input
        public InputAction OnSelectCards { get; private set; }
        public InputAction OnUINavigate { get; private set; }
        public InputAction OnUICancel { get; private set; }

        public Inputs()
        {
            PlayerInput = new PlayerInput();

            OnLeftClick = PlayerInput.Player.LeftClick;
            OnRightClick = PlayerInput.Player.RightClick;
            OnScroll = PlayerInput.Player.Scroll;
            OnRotateCamera = PlayerInput.Player.RotateCamera;
            OnMousePosition = PlayerInput.Player.MousePosition;
            OnMoveCamera = PlayerInput.Player.MoveCamera;
            OnDeleteBuilding = PlayerInput.Player.DeleteBuilding;
            OnNextTurn = PlayerInput.Player.NextTurn;
            OnShowPause = PlayerInput.Player.ShowPause;
            OnRotateBuilding = PlayerInput.Player.BuildingRotate;
            OnDeselectCards = PlayerInput.Player.DeselectCards;

            // UI
            OnSelectCards = PlayerInput.UI.SelectCards;
            OnUINavigate = PlayerInput.UI.Navigate;
            OnUICancel = PlayerInput.UI.Cancel;

            PlayerInput.UI.Enable();

            // Remove this line to disable Player Input on launch
            PlayerInput.Player.Enable();

            InputUser.onChange += InputUser_onChange;
        }

        public void TogglePlayerInput(bool toggle)
        {
            if (toggle) PlayerInput.Player.Enable();
            else PlayerInput.Player.Disable();
        }

        private void InputUser_onChange(InputUser arg1, InputUserChange arg2, InputDevice arg3)
        {
            if (arg2 == InputUserChange.ControlSchemeChanged)
            {
                Debug.Log($"Input Changed: {arg1.controlScheme.Value.name}");
                ControlScheme = arg1.controlScheme.Value;
                OnControlChange?.Invoke(arg1.controlScheme.Value);
            }
        }
    }
}
