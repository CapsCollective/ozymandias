using System;
using Platform;
using UnityEngine;
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

        public Transform WorldSpaceCursor;

        public static Action<InputControlScheme> OnControlChange;

        public static InputControlScheme ControlScheme;

        // Player Input
        public PlayerInputs PlayerInput { get; }
        
        public InputAction OnLeftMouse { get; }
        public InputAction OnLeftClick { get; }
        public InputAction OnRightMouse { get; }
        public InputAction OnRightClick { get; }
        public InputAction OnZoomCamera { get; }
        public InputAction OnRotateCamera { get; }
        public InputAction OnMousePosition { get; }
        public InputAction OnMoveCamera { get; }
        public InputAction OnConfirmSelectedStructure { get; }
        public InputAction OnNextTurn { get; }
        public InputAction OnToggleBook { get; }
        public InputAction OnRotateBuilding { get; }
        public InputAction OnSelectCards { get; }
        public InputAction OnDeselectCards { get; }
        public InputAction OnNavigateCards { get; }
        public InputAction OnSelectCardIndex { get; }
        
        public InputAction OnDialogueNext{ get; }

        public Inputs()
        {
            PlayerInput = new PlayerInputs();

            OnMousePosition = PlayerInput.Player.MousePosition;
            OnLeftMouse = PlayerInput.Player.LeftMouse;
            OnLeftClick = PlayerInput.Player.LeftClick;
            OnRightMouse = PlayerInput.Player.RightMouse;
            OnRightClick = PlayerInput.Player.RightClick;

            // Camera Controls
            OnMoveCamera = PlayerInput.Player.MoveCamera;
            OnRotateCamera = PlayerInput.Player.RotateCamera;
            OnZoomCamera = PlayerInput.Player.ZoomCamera;
            
            // UI Navigation
            OnConfirmSelectedStructure = PlayerInput.Player.ConfirmSelectedStructure;
            OnNextTurn = PlayerInput.Player.NextTurn;
            OnToggleBook = PlayerInput.Player.ToggleBook;
            OnRotateBuilding = PlayerInput.Player.RotateBuilding;
            
            // Cards
            OnSelectCards = PlayerInput.Player.SelectCards;
            OnDeselectCards = PlayerInput.Player.DeselectCards;
            OnNavigateCards = PlayerInput.Player.NavigateCards;
            OnSelectCardIndex = PlayerInput.Player.SelectCardIndex;
            
            // Dialogue
            OnDialogueNext = PlayerInput.Player.DialogueNext;

            PlayerInput.UI.Enable();
            PlayerInput.Player.Enable();

            InputUser.onChange += InputUser_onChange;
            ControlScheme =  PlayerInput.controlSchemes[PlatformManager.Instance.Input.GetDefaultControlScheme()];
            PlayerInput.bindingMask = InputBinding.MaskByGroup(ControlScheme.bindingGroup);
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
                PlayerInput.bindingMask = InputBinding.MaskByGroup(ControlScheme.bindingGroup);
                OnControlChange?.Invoke(arg1.controlScheme.Value);
            }
        }

        public static bool DeviceIsKeyboard(InputAction action)
        {
            if (action.activeControl.device.displayName.Contains("Keyboard") ||
                action.activeControl.device.displayName.Contains("Controller"))
                return true;

            return false;
        }

        public Ray GetMouseRay(Camera cam)
        {
            if (!UsingController)
            {
                return cam.ScreenPointToRay(
                    new Vector3(MousePosition.x, MousePosition.y, cam.nearClipPlane));
            }
            return new Ray(WorldSpaceCursor.position + new Vector3(0, 10, 0), Vector3.down);
        }
    }
}
