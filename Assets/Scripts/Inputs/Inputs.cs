using System;
using Platform;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.InputSystem.Users;

namespace Inputs
{
    public class Inputs
    {
        const float SPHERECAST_RADIUS = 0.4f;

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

        public Action ToggleBook;
        public Action CenterCamera;

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
        public InputAction OnNavigateBookmark { get; }
        public InputAction OnSelectCardIndex { get; }
        public InputAction OnDialogueNext{ get; }
        public InputAction OpenQuests{ get; }
        public InputAction OpenNewspaper{ get; }
        public InputAction UIClose{ get; }
        public InputAction ReturnToTown{ get; }
        public InputAction ToggleTooltips{ get; }

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
            OnToggleBook = PlayerInput.UI.ToggleBook;
            OnRotateBuilding = PlayerInput.Player.RotateBuilding;
            OnNavigateBookmark = PlayerInput.UI.NavigateBookmarks;
            OpenQuests = PlayerInput.Player.OpenQuests;
            OpenNewspaper = PlayerInput.Player.OpenNewspaper;
            UIClose = PlayerInput.UI.Cancel;
            ToggleTooltips = PlayerInput.Player.ToggleTooltips;

            // Cards
            OnSelectCards = PlayerInput.Player.SelectCards;
            OnDeselectCards = PlayerInput.Player.DeselectCards;
            OnNavigateCards = PlayerInput.Player.NavigateCards;
            OnSelectCardIndex = PlayerInput.Player.SelectCardIndex;
            
            // Dialogue
            OnDialogueNext = PlayerInput.Player.DialogueNext;

            // Quests
            OpenQuests = PlayerInput.Player.OpenQuests;

            PlayerInput.UI.Enable();
            PlayerInput.Player.Enable();

            InputUser.onChange += InputUser_onChange;
            ControlScheme =  PlayerInput.controlSchemes[PlatformManager.Instance.Input.GetDefaultControlScheme()];
            PlayerInput.bindingMask = InputBinding.MaskByGroup(ControlScheme.bindingGroup);
            OnToggleBook.performed += OnToggleBook_performed;
        }

        private void OnToggleBook_performed(InputAction.CallbackContext obj)
        {
            if (obj.interaction is HoldInteraction) CenterCamera?.Invoke();
            else if (obj.interaction is PressInteraction) ToggleBook?.Invoke();
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

        public RaycastHit GetRaycast(Camera cam, float distance, int layerMask)
        {
            Ray ray = GetMouseRay(cam);
            RaycastHit hit;
            if (!UsingController)
                Physics.Raycast(ray, out hit, distance, layerMask);
            else
                Physics.SphereCast(ray, SPHERECAST_RADIUS, out hit, distance, layerMask);

            return hit;
        }
    }
}
