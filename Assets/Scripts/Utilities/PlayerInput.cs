// GENERATED AUTOMATICALLY FROM 'Assets/Settings/PlayerControls.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @PlayerInput : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @PlayerInput()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""PlayerControls"",
    ""maps"": [
        {
            ""name"": ""Player"",
            ""id"": ""6204d203-e191-4cf7-aa67-8d57576b1d94"",
            ""actions"": [
                {
                    ""name"": ""Left Click"",
                    ""type"": ""Button"",
                    ""id"": ""48b20d88-baee-4019-aea3-e565b6e52b5c"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Right Click"",
                    ""type"": ""Button"",
                    ""id"": ""81fe7b64-a126-4391-84b7-69fe7c03ac20"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Scroll"",
                    ""type"": ""Value"",
                    ""id"": ""61557cdb-469e-403a-8ccd-07f5d962dbaa"",
                    ""expectedControlType"": """",
                    ""processors"": ""Normalize(max=1),Scale(factor=0.01)"",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Mouse Position"",
                    ""type"": ""Value"",
                    ""id"": ""71bbbbfa-9f59-45ec-9c5d-60ea66455924"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Move Camera"",
                    ""type"": ""Value"",
                    ""id"": ""7c71ed57-0d72-4094-b95f-dbf4f29ae3ca"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Rotate Camera"",
                    ""type"": ""Value"",
                    ""id"": ""0a44445c-aa72-442e-9ce7-78b7793f95b1"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""DeleteBuilding"",
                    ""type"": ""Button"",
                    ""id"": ""2940d26f-e08c-4a18-804f-8f913c5b61d7"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Screenshot"",
                    ""type"": ""Button"",
                    ""id"": ""2dd729ed-effd-4141-addd-a5abd2dcb05f"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""e92a8d6e-49ce-4b19-a503-18468fe94fb4"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mouse and Keyboard"",
                    ""action"": ""Left Click"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""38c39939-3c03-4da2-8056-f911b3ed76ce"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Controller"",
                    ""action"": ""Left Click"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""5a28b8e6-ea21-41f8-8b98-18e1a587184a"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mouse and Keyboard"",
                    ""action"": ""Right Click"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b357a7ad-016e-4555-b16b-a214876a3fb1"",
                    ""path"": ""<Mouse>/scroll/y"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mouse and Keyboard"",
                    ""action"": ""Scroll"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""59f19d06-3e47-48b9-84cd-09950509ff35"",
                    ""path"": ""<Gamepad>/rightStick/y"",
                    ""interactions"": """",
                    ""processors"": ""Scale(factor=2)"",
                    ""groups"": ""Controller"",
                    ""action"": ""Scroll"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""d9cca30c-6cde-4ef8-b32d-a4279be96b61"",
                    ""path"": ""<Mouse>/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mouse and Keyboard"",
                    ""action"": ""Mouse Position"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""1c0022d7-0321-4991-a467-463415c025c0"",
                    ""path"": ""<Gamepad>/leftStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Controller"",
                    ""action"": ""Move Camera"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""164e5272-5227-42e7-a733-bd672b23aa9d"",
                    ""path"": ""<Gamepad>/rightStick/x"",
                    ""interactions"": """",
                    ""processors"": ""Scale(factor=0.25)"",
                    ""groups"": ""Controller"",
                    ""action"": ""Rotate Camera"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""Button With One Modifier"",
                    ""id"": ""526fb184-f2dc-4128-b327-f2b9522023df"",
                    ""path"": ""ButtonWithOneModifier"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Rotate Camera"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""modifier"",
                    ""id"": ""7ce71aec-b1d1-46bc-acd3-e7d4e4cb48ad"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mouse and Keyboard"",
                    ""action"": ""Rotate Camera"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""button"",
                    ""id"": ""cf6dbc72-e5f6-4092-ba04-1ed9645dd532"",
                    ""path"": ""<Mouse>/delta/x"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mouse and Keyboard"",
                    ""action"": ""Rotate Camera"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""cc970a85-536d-41f1-a2fd-a272b252fae9"",
                    ""path"": ""<Keyboard>/delete"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mouse and Keyboard"",
                    ""action"": ""DeleteBuilding"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""5c32499e-11c4-47eb-8a32-e112a9763e3b"",
                    ""path"": ""<Gamepad>/buttonEast"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Controller"",
                    ""action"": ""DeleteBuilding"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""7c2fddbe-1afc-4232-9222-6328fdebe022"",
                    ""path"": ""<Keyboard>/f5"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Screenshot"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""Mouse and Keyboard"",
            ""bindingGroup"": ""Mouse and Keyboard"",
            ""devices"": [
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": false,
                    ""isOR"": false
                },
                {
                    ""devicePath"": ""<Mouse>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        },
        {
            ""name"": ""Controller"",
            ""bindingGroup"": ""Controller"",
            ""devices"": [
                {
                    ""devicePath"": ""<Gamepad>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
        // Player
        m_Player = asset.FindActionMap("Player", throwIfNotFound: true);
        m_Player_LeftClick = m_Player.FindAction("Left Click", throwIfNotFound: true);
        m_Player_RightClick = m_Player.FindAction("Right Click", throwIfNotFound: true);
        m_Player_Scroll = m_Player.FindAction("Scroll", throwIfNotFound: true);
        m_Player_MousePosition = m_Player.FindAction("Mouse Position", throwIfNotFound: true);
        m_Player_MoveCamera = m_Player.FindAction("Move Camera", throwIfNotFound: true);
        m_Player_RotateCamera = m_Player.FindAction("Rotate Camera", throwIfNotFound: true);
        m_Player_DeleteBuilding = m_Player.FindAction("DeleteBuilding", throwIfNotFound: true);
        m_Player_Screenshot = m_Player.FindAction("Screenshot", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    // Player
    private readonly InputActionMap m_Player;
    private IPlayerActions m_PlayerActionsCallbackInterface;
    private readonly InputAction m_Player_LeftClick;
    private readonly InputAction m_Player_RightClick;
    private readonly InputAction m_Player_Scroll;
    private readonly InputAction m_Player_MousePosition;
    private readonly InputAction m_Player_MoveCamera;
    private readonly InputAction m_Player_RotateCamera;
    private readonly InputAction m_Player_DeleteBuilding;
    private readonly InputAction m_Player_Screenshot;
    public struct PlayerActions
    {
        private @PlayerInput m_Wrapper;
        public PlayerActions(@PlayerInput wrapper) { m_Wrapper = wrapper; }
        public InputAction @LeftClick => m_Wrapper.m_Player_LeftClick;
        public InputAction @RightClick => m_Wrapper.m_Player_RightClick;
        public InputAction @Scroll => m_Wrapper.m_Player_Scroll;
        public InputAction @MousePosition => m_Wrapper.m_Player_MousePosition;
        public InputAction @MoveCamera => m_Wrapper.m_Player_MoveCamera;
        public InputAction @RotateCamera => m_Wrapper.m_Player_RotateCamera;
        public InputAction @DeleteBuilding => m_Wrapper.m_Player_DeleteBuilding;
        public InputAction @Screenshot => m_Wrapper.m_Player_Screenshot;
        public InputActionMap Get() { return m_Wrapper.m_Player; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PlayerActions set) { return set.Get(); }
        public void SetCallbacks(IPlayerActions instance)
        {
            if (m_Wrapper.m_PlayerActionsCallbackInterface != null)
            {
                @LeftClick.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnLeftClick;
                @LeftClick.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnLeftClick;
                @LeftClick.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnLeftClick;
                @RightClick.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnRightClick;
                @RightClick.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnRightClick;
                @RightClick.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnRightClick;
                @Scroll.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnScroll;
                @Scroll.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnScroll;
                @Scroll.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnScroll;
                @MousePosition.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMousePosition;
                @MousePosition.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMousePosition;
                @MousePosition.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMousePosition;
                @MoveCamera.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMoveCamera;
                @MoveCamera.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMoveCamera;
                @MoveCamera.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMoveCamera;
                @RotateCamera.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnRotateCamera;
                @RotateCamera.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnRotateCamera;
                @RotateCamera.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnRotateCamera;
                @DeleteBuilding.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnDeleteBuilding;
                @DeleteBuilding.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnDeleteBuilding;
                @DeleteBuilding.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnDeleteBuilding;
                @Screenshot.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnScreenshot;
                @Screenshot.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnScreenshot;
                @Screenshot.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnScreenshot;
            }
            m_Wrapper.m_PlayerActionsCallbackInterface = instance;
            if (instance != null)
            {
                @LeftClick.started += instance.OnLeftClick;
                @LeftClick.performed += instance.OnLeftClick;
                @LeftClick.canceled += instance.OnLeftClick;
                @RightClick.started += instance.OnRightClick;
                @RightClick.performed += instance.OnRightClick;
                @RightClick.canceled += instance.OnRightClick;
                @Scroll.started += instance.OnScroll;
                @Scroll.performed += instance.OnScroll;
                @Scroll.canceled += instance.OnScroll;
                @MousePosition.started += instance.OnMousePosition;
                @MousePosition.performed += instance.OnMousePosition;
                @MousePosition.canceled += instance.OnMousePosition;
                @MoveCamera.started += instance.OnMoveCamera;
                @MoveCamera.performed += instance.OnMoveCamera;
                @MoveCamera.canceled += instance.OnMoveCamera;
                @RotateCamera.started += instance.OnRotateCamera;
                @RotateCamera.performed += instance.OnRotateCamera;
                @RotateCamera.canceled += instance.OnRotateCamera;
                @DeleteBuilding.started += instance.OnDeleteBuilding;
                @DeleteBuilding.performed += instance.OnDeleteBuilding;
                @DeleteBuilding.canceled += instance.OnDeleteBuilding;
                @Screenshot.started += instance.OnScreenshot;
                @Screenshot.performed += instance.OnScreenshot;
                @Screenshot.canceled += instance.OnScreenshot;
            }
        }
    }
    public PlayerActions @Player => new PlayerActions(this);
    private int m_MouseandKeyboardSchemeIndex = -1;
    public InputControlScheme MouseandKeyboardScheme
    {
        get
        {
            if (m_MouseandKeyboardSchemeIndex == -1) m_MouseandKeyboardSchemeIndex = asset.FindControlSchemeIndex("Mouse and Keyboard");
            return asset.controlSchemes[m_MouseandKeyboardSchemeIndex];
        }
    }
    private int m_ControllerSchemeIndex = -1;
    public InputControlScheme ControllerScheme
    {
        get
        {
            if (m_ControllerSchemeIndex == -1) m_ControllerSchemeIndex = asset.FindControlSchemeIndex("Controller");
            return asset.controlSchemes[m_ControllerSchemeIndex];
        }
    }
    public interface IPlayerActions
    {
        void OnLeftClick(InputAction.CallbackContext context);
        void OnRightClick(InputAction.CallbackContext context);
        void OnScroll(InputAction.CallbackContext context);
        void OnMousePosition(InputAction.CallbackContext context);
        void OnMoveCamera(InputAction.CallbackContext context);
        void OnRotateCamera(InputAction.CallbackContext context);
        void OnDeleteBuilding(InputAction.CallbackContext context);
        void OnScreenshot(InputAction.CallbackContext context);
    }
}
