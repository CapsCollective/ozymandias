using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using static Managers.GameManager;

namespace Inputs
{
    public class Inputs
    {
        const float SPHERECAST_RADIUS = 0.4f;

        public bool UsingController => ControlScheme == PlayerInput.ControllerScheme;
        public static Action<InputControlScheme> OnControlChange;
        public static InputControlScheme ControlScheme;
        
        public Transform WorldSpaceCursor;

        // Player Input
        private PlayerInputs _playerInputs;
        public PlayerInputs PlayerInput { get => _playerInputs; }
        private InputAction MouseMoved { get; }
        public Vector2 MousePosition 
        {
            get
            {
                if(!UsingController)
                    return MouseMoved.ReadValue<Vector2>();
                return Camera.main.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
            }
        }
        
        public InputAction LeftMouse { get; }
        public InputAction LeftClick { get; }
        public InputAction RightMouse { get; }
        public InputAction RightClick { get; }
        public InputAction Scroll { get; }
        public InputAction ZoomCamera { get; }
        public InputAction RotateCamera { get; }
        public InputAction MoveCamera { get; }
        public InputAction DemolishBuilding { get; }
        public InputAction SelectQuest { get; }
        public InputAction NextTurn { get; }
        public InputAction ToggleBook { get; }
        public InputAction RotateBuilding { get; }
        public InputAction SelectCards { get; }
        public InputAction DeselectCards { get; }
        public InputAction NavigateCards { get; }
        public InputAction NavigateBookmark { get; }
        public InputAction SelectCardIndex { get; }
        public InputAction DialogueNext { get; }
        public InputAction OpenQuests { get; }
        public InputAction OpenNewspaper { get; }
        public InputAction Close { get; }
        public InputAction ReturnToTown { get; }
        public InputAction ToggleTooltips { get; }
        public InputAction NavigateTooltips { get; }
        public InputAction OnScreenshot{ get; }
        public InputAction OnDebugToggle{ get; }

        public Inputs()
        {
            _playerInputs = new PlayerInputs();

            MouseMoved = PlayerInput.Player.MousePosition;
            LeftMouse = PlayerInput.Player.LeftMouse;
            LeftClick = PlayerInput.Player.LeftClick;
            RightMouse = PlayerInput.Player.RightMouse;
            RightClick = PlayerInput.Player.RightClick;
            Scroll = PlayerInput.UI.Scroll;

            // Camera Controls
            MoveCamera = PlayerInput.Player.MoveCamera;
            RotateCamera = PlayerInput.Player.RotateCamera;
            ZoomCamera = PlayerInput.Player.ZoomCamera;
            
            // Menus
            ToggleBook = PlayerInput.UI.ToggleBook;
            NavigateBookmark = PlayerInput.UI.NavigateBookmarks;
            Close = PlayerInput.UI.Cancel;
            OpenNewspaper = PlayerInput.Player.OpenNewspaper;
            OpenQuests = PlayerInput.Player.OpenQuests;

            // Tooltips
            ToggleTooltips = PlayerInput.Player.ToggleTooltips;
            NavigateTooltips = PlayerInput.Player.NavigateTooltips;

            // Structures
            RotateBuilding = PlayerInput.Player.RotateBuilding;
            DemolishBuilding = PlayerInput.Player.DemolishBuilding;
            SelectQuest = PlayerInput.Player.SelectQuest;
            
            // Cards
            SelectCards = PlayerInput.Player.SelectCards;
            DeselectCards = PlayerInput.Player.DeselectCards;
            NavigateCards = PlayerInput.Player.NavigateCards;
            SelectCardIndex = PlayerInput.Player.SelectCardIndex;

            // Misc
            NextTurn = PlayerInput.Player.NextTurn;
            DialogueNext = PlayerInput.Player.DialogueNext;
            ReturnToTown = PlayerInput.Player.ReturnToTown;
            OnScreenshot = PlayerInput.Player.Screenshot;
            
            // Debug
            if (Debug.isDebugBuild)
            {
                OnDebugToggle = PlayerInput.UI.DebugToggle;
            }

            Manager.PlatformManager.Input.AddExtraBinds(ref _playerInputs);

            PlayerInput.UI.Enable();
            PlayerInput.Player.Enable();

            InputUser.onChange += InputUser_onChange;
            ControlScheme =  PlayerInput.controlSchemes[Manager.PlatformManager.Input.GetDefaultControlScheme()];
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
