using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using Managers;

public class InputManager
{
    public static InputManager Instance;
    public static bool UsingController => Instance.ControlScheme == Instance.PlayerInput.ControllerScheme;
    public static Vector2 MousePosition 
    {
        get
        {
            if(!UsingController)
                return Instance.IA_MousePosition.ReadValue<Vector2>();
            return Camera.main.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        }
    }

    public Action<InputControlScheme> OnControlChange;

    public InputControlScheme ControlScheme;

    // Player Input
    public PlayerInput PlayerInput { get; private set; }
    public InputAction IA_OnLeftClick { get; private set; }
    public InputAction IA_OnRightClick { get; private set; }
    public InputAction IA_OnScroll { get; private set; }
    public InputAction IA_RotateCamera { get; private set; }
    public InputAction IA_MousePosition { get; private set; }
    public InputAction IA_MoveCamera { get; private set; }
    public InputAction IA_DeleteBuilding { get; private set; }
    public InputAction IA_NextTurn { get; private set; }
    public InputAction IA_ShowPause { get; private set; }
    public InputAction IA_RotateBuilding { get; private set; }
    public InputAction IA_DeselectCards { get; private set; }

    // UI Input
    public InputAction IA_SelectCards { get; private set; }
    public InputAction IA_CancelBuild { get; private set; }
    public InputAction IA_UINavigate { get; private set; }
    public InputAction IA_UICancel { get; private set; }

    public InputManager()
    {
        if (Instance == null)
            Instance = this;
        else
            return;

        PlayerInput = new PlayerInput();

        IA_OnLeftClick = PlayerInput.Player.LeftClick;
        IA_OnRightClick = PlayerInput.Player.RightClick;
        IA_OnScroll = PlayerInput.Player.Scroll;
        IA_RotateCamera = PlayerInput.Player.RotateCamera;
        IA_MousePosition = PlayerInput.Player.MousePosition;
        IA_MoveCamera = PlayerInput.Player.MoveCamera;
        IA_DeleteBuilding = PlayerInput.Player.DeleteBuilding;
        IA_NextTurn = PlayerInput.Player.NextTurn;
        IA_ShowPause = PlayerInput.Player.ShowPause;
        IA_RotateBuilding = PlayerInput.Player.BuildingRotate;
        IA_DeselectCards = PlayerInput.Player.DeselectCards;

        // UI
        IA_SelectCards = PlayerInput.UI.SelectCards;
        IA_UINavigate = PlayerInput.UI.Navigate;
        IA_UICancel = PlayerInput.UI.Cancel;

        PlayerInput.UI.Enable();

        // Remove this line to disable Player Input on launch
        PlayerInput.Player.Enable();

        InputUser.onChange += InputUser_onChange;
        Debug.Log("InputManager running.");
    }

    public static void TogglePlayerInput(bool toggle)
    {
        if (toggle) Instance.PlayerInput.Player.Enable();
        else Instance.PlayerInput.Player.Disable();
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

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void OnGameLoad()
    {
        new InputManager();
    }
}
