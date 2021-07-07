using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

public class InputManager
{
    public static InputManager Instance;
    public Action<InputControlScheme> OnControlChange;

    public InputControlScheme ControlScheme;
    public PlayerInput PlayerInput { get; private set; }
    public InputAction OnLeftClick { get; private set; }
    public InputAction OnRightClick { get; private set; }
    public InputAction OnScroll { get; private set; }
    public InputAction RotateCamera { get; private set; }
    public InputAction MousePosition { get; private set; }
    public InputAction MoveCamera { get; private set; }

    public InputManager()
    {
        if (Instance == null)
            Instance = this;
        else
            return;

        PlayerInput = new PlayerInput();

        OnLeftClick = PlayerInput.Player.LeftClick;
        OnLeftClick.Enable();
        OnRightClick = PlayerInput.Player.RightClick;
        OnRightClick.Enable();
        OnScroll = PlayerInput.Player.Scroll;
        OnScroll.Enable();
        RotateCamera = PlayerInput.Player.RotateCamera;
        RotateCamera.Enable();
        MousePosition = PlayerInput.Player.MousePosition;
        MousePosition.Enable();
        MoveCamera = PlayerInput.Player.MoveCamera;
        MoveCamera.Enable();

        PlayerInput.Enable();
        InputUser.onChange += InputUser_onChange;
        Debug.Log("InputManager running.");
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
