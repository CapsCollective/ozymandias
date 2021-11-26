// GENERATED AUTOMATICALLY FROM 'Assets/Scripts/Inputs/PlayerControls.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace Inputs
{
    public class @PlayerInputs : IInputActionCollection, IDisposable
    {
        public InputActionAsset asset { get; }
        public @PlayerInputs()
        {
            asset = InputActionAsset.FromJson(@"{
    ""name"": ""PlayerControls"",
    ""maps"": [
        {
            ""name"": ""Player"",
            ""id"": ""6204d203-e191-4cf7-aa67-8d57576b1d94"",
            ""actions"": [
                {
                    ""name"": ""Mouse Position"",
                    ""type"": ""Value"",
                    ""id"": ""71bbbbfa-9f59-45ec-9c5d-60ea66455924"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Mouse Delta"",
                    ""type"": ""Value"",
                    ""id"": ""890c83d4-f8ea-488e-b0b8-c78427c7ce5b"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Left Click"",
                    ""type"": ""Button"",
                    ""id"": ""48b20d88-baee-4019-aea3-e565b6e52b5c"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Tap(duration=0.3)""
                },
                {
                    ""name"": ""Left Mouse"",
                    ""type"": ""Button"",
                    ""id"": ""31a8fc31-16b9-4c27-afae-81f0bafd08a2"",
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
                    ""name"": ""Right Mouse"",
                    ""type"": ""Button"",
                    ""id"": ""a86eadf2-f04a-4ef0-890c-3514f8dfd90e"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Move Camera"",
                    ""type"": ""Value"",
                    ""id"": ""7c71ed57-0d72-4094-b95f-dbf4f29ae3ca"",
                    ""expectedControlType"": ""Vector2"",
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
                    ""name"": ""Zoom Camera"",
                    ""type"": ""Value"",
                    ""id"": ""61557cdb-469e-403a-8ccd-07f5d962dbaa"",
                    ""expectedControlType"": """",
                    ""processors"": ""Normalize(max=1),Scale"",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Toggle Book"",
                    ""type"": ""Button"",
                    ""id"": ""38061098-501b-4c09-af6a-8b10442c20ae"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Toggle Tooltips"",
                    ""type"": ""Button"",
                    ""id"": ""66c79df1-d6cc-4d63-9162-523c1c2b24dd"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Next Turn"",
                    ""type"": ""Button"",
                    ""id"": ""b9555a6f-7b47-4067-b0b5-7ed6ed3c7196"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Confirm Selected Structure"",
                    ""type"": ""PassThrough"",
                    ""id"": ""2940d26f-e08c-4a18-804f-8f913c5b61d7"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Rotate Building"",
                    ""type"": ""Button"",
                    ""id"": ""01d6d096-8732-49ae-81e2-e83f8abaa8b6"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Select Cards"",
                    ""type"": ""Button"",
                    ""id"": ""3688873b-b781-4552-a280-8318c205cd71"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Deselect Cards"",
                    ""type"": ""Button"",
                    ""id"": ""47714686-228f-4886-a09b-26c740e5bd54"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Navigate Cards"",
                    ""type"": ""Value"",
                    ""id"": ""3f065f30-0d1e-4f9c-854c-cf59b1871781"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Select Card Index"",
                    ""type"": ""Value"",
                    ""id"": ""272c9955-db7e-4e68-85fc-8d7cf6981830"",
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
                },
                {
                    ""name"": ""Navigate Bookmarks"",
                    ""type"": ""Button"",
                    ""id"": ""9282166e-cc5f-499a-b853-720b0b7bd76e"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Dialogue Next"",
                    ""type"": ""Button"",
                    ""id"": ""93f5370f-21ac-4d13-9ca7-8da09db794b7"",
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
                    ""interactions"": ""Tap(duration=0.3)"",
                    ""processors"": """",
                    ""groups"": ""Mouse and Keyboard"",
                    ""action"": ""Right Click"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""1c0022d7-0321-4991-a467-463415c025c0"",
                    ""path"": ""<Gamepad>/leftStick"",
                    ""interactions"": """",
                    ""processors"": ""StickDeadzone,ScaleVector2(x=10,y=10)"",
                    ""groups"": ""Controller"",
                    ""action"": ""Move Camera"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""Keyboard"",
                    ""id"": ""ec8071e7-80e4-4770-b152-759154f0fe4c"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": ""ScaleVector2(x=10,y=10)"",
                    ""groups"": """",
                    ""action"": ""Move Camera"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""b65a6343-5158-4e06-8637-6f68652064ff"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mouse and Keyboard"",
                    ""action"": ""Move Camera"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""98186f29-6f64-4fcb-9f53-bc62f4e998ba"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mouse and Keyboard"",
                    ""action"": ""Move Camera"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""0368c5a4-f2fe-439b-baa8-df6ae8d4c4f8"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mouse and Keyboard"",
                    ""action"": ""Move Camera"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""5df3fb8a-b5df-407a-9788-a80519ecdc19"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mouse and Keyboard"",
                    ""action"": ""Move Camera"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""164e5272-5227-42e7-a733-bd672b23aa9d"",
                    ""path"": ""<Gamepad>/rightStick/x"",
                    ""interactions"": """",
                    ""processors"": ""Invert,AxisDeadzone,Scale(factor=75)"",
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
                    ""processors"": ""Scale(factor=12.5)"",
                    ""groups"": ""Mouse and Keyboard"",
                    ""action"": ""Rotate Camera"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Keyboard"",
                    ""id"": ""b7690c4c-9b83-490d-94fc-9a389fb8408a"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Rotate Camera"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""e8ca6346-9311-4186-b1a5-cdf51dd87043"",
                    ""path"": ""<Keyboard>/q"",
                    ""interactions"": """",
                    ""processors"": ""Scale(factor=75)"",
                    ""groups"": ""Mouse and Keyboard"",
                    ""action"": ""Rotate Camera"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""587e54d4-52b4-4958-ab91-0b67c4529916"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": ""Scale(factor=75)"",
                    ""groups"": ""Mouse and Keyboard"",
                    ""action"": ""Rotate Camera"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""fd2c6981-53c6-45c4-90bb-9a8c08b6da5e"",
                    ""path"": ""<Gamepad>/start"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Controller"",
                    ""action"": ""Toggle Book"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""5a75c688-e4ec-4432-9692-be31b5164126"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mouse and Keyboard"",
                    ""action"": ""Toggle Book"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""Shoulder Buttons"",
                    ""id"": ""50e44425-7ee9-4bcd-8884-feb85973991b"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Rotate Building"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""bbe5c703-43fc-4bd6-9568-6cd0fb50ce37"",
                    ""path"": ""<Gamepad>/leftShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Controller"",
                    ""action"": ""Rotate Building"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""64f13172-95c0-47e5-8966-a31c431cb98b"",
                    ""path"": ""<Gamepad>/rightShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Controller"",
                    ""action"": ""Rotate Building"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""bfcbcb77-86fe-4b5d-8f19-38bc63be84f0"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": ""Tap(duration=0.3)"",
                    ""processors"": """",
                    ""groups"": ""Mouse and Keyboard"",
                    ""action"": ""Rotate Building"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""191d1206-a623-48a4-9195-106ab0f036c2"",
                    ""path"": ""<Keyboard>/r"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mouse and Keyboard"",
                    ""action"": ""Rotate Building"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""048798e2-bacf-4c0d-bb66-ced35fe06f18"",
                    ""path"": ""<Gamepad>/dpad/up"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Controller"",
                    ""action"": ""Select Cards"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""8a4805b4-d333-4691-86a6-9a5cc8c59f70"",
                    ""path"": ""<Gamepad>/dpad/down"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Controller"",
                    ""action"": ""Deselect Cards"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""9adc02fc-ee2b-4aa8-9c39-374503459508"",
                    ""path"": ""<Gamepad>/dpad"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Controller"",
                    ""action"": ""Navigate Cards"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f98e91fb-106b-4dc1-8adb-a58b3c6ea0d8"",
                    ""path"": ""<Gamepad>/select"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Controller"",
                    ""action"": ""Toggle Tooltips"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""cc970a85-536d-41f1-a2fd-a272b252fae9"",
                    ""path"": ""<Keyboard>/delete"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mouse and Keyboard"",
                    ""action"": ""Confirm Selected Structure"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""5c32499e-11c4-47eb-8a32-e112a9763e3b"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": ""Hold"",
                    ""processors"": """",
                    ""groups"": ""Controller"",
                    ""action"": ""Confirm Selected Structure"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""3392453a-a4ab-48d5-abac-119e7b4455be"",
                    ""path"": ""<Gamepad>/buttonNorth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Controller"",
                    ""action"": ""Next Turn"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""d2b02c9a-093a-4bb6-85f8-c97c2d800b04"",
                    ""path"": ""<Keyboard>/enter"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mouse and Keyboard"",
                    ""action"": ""Next Turn"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""37736fb9-ded0-40a2-9b46-044f7eaf7d8b"",
                    ""path"": ""<Mouse>/delta"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mouse and Keyboard"",
                    ""action"": ""Mouse Delta"",
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
                    ""id"": ""b357a7ad-016e-4555-b16b-a214876a3fb1"",
                    ""path"": ""<Mouse>/scroll/y"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mouse and Keyboard"",
                    ""action"": ""Zoom Camera"",
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
                    ""action"": ""Zoom Camera"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""Keyboard"",
                    ""id"": ""49ec3ced-10c8-4e38-beac-7f5ef2e9c85b"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": ""Scale(factor=5)"",
                    ""groups"": """",
                    ""action"": ""Zoom Camera"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""d4c8ed5a-618f-4433-bb7b-8ab2ed3e2ae6"",
                    ""path"": ""<Keyboard>/upArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mouse and Keyboard"",
                    ""action"": ""Zoom Camera"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""5ac557c2-cfb3-4d50-b899-45696f68878a"",
                    ""path"": ""<Keyboard>/downArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mouse and Keyboard"",
                    ""action"": ""Zoom Camera"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
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
                },
                {
                    ""name"": """",
                    ""id"": ""a086e9d9-9e59-4c02-8f04-9caa80ef30a7"",
                    ""path"": ""<Keyboard>/1"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mouse and Keyboard"",
                    ""action"": ""Select Card Index"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""2992a63f-0b9e-44c3-a352-9e2f6db1da6c"",
                    ""path"": ""<Keyboard>/2"",
                    ""interactions"": """",
                    ""processors"": ""Scale(factor=2)"",
                    ""groups"": ""Mouse and Keyboard"",
                    ""action"": ""Select Card Index"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a9dc7110-b4cf-4f96-b7d1-615cefb22bc4"",
                    ""path"": ""<Keyboard>/3"",
                    ""interactions"": """",
                    ""processors"": ""Scale(factor=3)"",
                    ""groups"": ""Mouse and Keyboard"",
                    ""action"": ""Select Card Index"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""56d3c50c-aa05-4202-8b91-e7fdb6918d84"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mouse and Keyboard"",
                    ""action"": ""Left Mouse"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""17fc50cd-0695-445d-b595-02efc789dd01"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mouse and Keyboard"",
                    ""action"": ""Right Mouse"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""Shoulder Buttons"",
                    ""id"": ""edc09bf7-f22d-4717-ac78-07f1802b87ef"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Navigate Bookmarks"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""5a1ce9b9-7dcd-4c00-a72f-7f3a4401cc1c"",
                    ""path"": ""<Gamepad>/leftShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Controller"",
                    ""action"": ""Navigate Bookmarks"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""9204114e-4eac-43ec-a3d7-b8000350d3cf"",
                    ""path"": ""<Gamepad>/rightShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Controller"",
                    ""action"": ""Navigate Bookmarks"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""3fa2d1b9-9baf-47b3-b528-eee9ad2458b9"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mouse and Keyboard"",
                    ""action"": ""Dialogue Next"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""2fcd4368-51f4-4c47-8e15-4ee192c4d6c4"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Controller"",
                    ""action"": ""Dialogue Next"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""UI"",
            ""id"": ""8ee93106-798f-4b3f-bb58-1103b89a56e7"",
            ""actions"": [
                {
                    ""name"": ""Navigate"",
                    ""type"": ""Value"",
                    ""id"": ""8a9357cc-284f-4969-9575-d1b9017cdb5f"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Submit"",
                    ""type"": ""Button"",
                    ""id"": ""5e7faee8-a80c-4de9-b224-525ca7934fa8"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Cancel"",
                    ""type"": ""Button"",
                    ""id"": ""f4b486a2-c0ff-4c4c-9645-ce0108ee5e9b"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Point"",
                    ""type"": ""PassThrough"",
                    ""id"": ""184850df-32f7-451a-a83d-6b75d8f7b75b"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Click"",
                    ""type"": ""PassThrough"",
                    ""id"": ""7ad002d5-abfd-444b-9ac9-1b911838f3f6"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""ScrollWheel"",
                    ""type"": ""PassThrough"",
                    ""id"": ""9f18eb80-e94c-423f-96a6-91a26a0a3342"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""MiddleClick"",
                    ""type"": ""PassThrough"",
                    ""id"": ""0dcb8870-2934-414d-8658-ec4eddbe065f"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""RightClick"",
                    ""type"": ""PassThrough"",
                    ""id"": ""8fda8e1a-3739-45af-b37c-448748eca144"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""TrackedDevicePosition"",
                    ""type"": ""PassThrough"",
                    ""id"": ""2dfdb4e7-14f0-4ae2-b503-b187abacdcd2"",
                    ""expectedControlType"": ""Vector3"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""TrackedDeviceOrientation"",
                    ""type"": ""PassThrough"",
                    ""id"": ""f343d9c8-4719-4eca-a21b-a43bcb75be2d"",
                    ""expectedControlType"": ""Quaternion"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""7f94f3c3-bfa4-4638-a965-f52cb6c18fbb"",
                    ""path"": ""<Gamepad>/dpad"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad;Controller"",
                    ""action"": ""Navigate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""Joystick"",
                    ""id"": ""d76ba9b4-15d5-44de-a94a-d5d6b2504b4c"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Navigate"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""c4165d12-73fd-4ecc-bbd8-1b15e3517f8c"",
                    ""path"": ""<Joystick>/stick/up"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Joystick"",
                    ""action"": ""Navigate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""1bc1b060-5050-45ef-9000-cd4b1e877b27"",
                    ""path"": ""<Joystick>/stick/down"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Joystick"",
                    ""action"": ""Navigate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""e2d233e1-137f-40ee-a56b-b3bfbae0e6a5"",
                    ""path"": ""<Joystick>/stick/left"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Joystick"",
                    ""action"": ""Navigate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""b31fc95a-5d70-4399-b8e8-1797c5daa0f9"",
                    ""path"": ""<Joystick>/stick/right"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Joystick"",
                    ""action"": ""Navigate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Keyboard"",
                    ""id"": ""bc905da0-32ff-42d5-8741-876887180b67"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Navigate"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""8884339e-59b3-47b1-bb63-fb3562db1126"",
                    ""path"": ""<Keyboard>/upArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard&Mouse;Mouse and Keyboard"",
                    ""action"": ""Navigate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""2aec5b52-80dc-4f4d-84b0-612a3bb83434"",
                    ""path"": ""<Keyboard>/downArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard&Mouse;Mouse and Keyboard"",
                    ""action"": ""Navigate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""b9e56b02-9d2d-4fa9-9323-82cd692dae80"",
                    ""path"": ""<Keyboard>/leftArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard&Mouse;Mouse and Keyboard"",
                    ""action"": ""Navigate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""97c03da9-295f-469f-996d-83503e05d6d9"",
                    ""path"": ""<Keyboard>/rightArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard&Mouse;Mouse and Keyboard"",
                    ""action"": ""Navigate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""f3a6240f-7799-43de-af61-87c8f3ea5d60"",
                    ""path"": ""*/{Submit}"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mouse and Keyboard;Controller"",
                    ""action"": ""Submit"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""d2051900-b4e9-4f22-a353-d2849ac76126"",
                    ""path"": ""*/{Cancel}"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mouse and Keyboard;Controller"",
                    ""action"": ""Cancel"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c1bbd824-a8e2-4381-90ac-ffee48890219"",
                    ""path"": ""<Mouse>/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard&Mouse;Mouse and Keyboard"",
                    ""action"": ""Point"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""50798b0b-8724-4fa7-80b3-fc9eba239cea"",
                    ""path"": ""<Pen>/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard&Mouse"",
                    ""action"": ""Point"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""71547fb2-ad33-4bc2-9637-737a51474a21"",
                    ""path"": ""<Touchscreen>/touch*/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Touch"",
                    ""action"": ""Point"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""8dc20971-7b04-4536-a834-46bf4ad354c2"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard&Mouse;Mouse and Keyboard"",
                    ""action"": ""Click"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""fedddfbb-2166-42f5-8750-7048cc7f80c7"",
                    ""path"": ""<Pen>/tip"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";Keyboard&Mouse"",
                    ""action"": ""Click"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""5e35c2f0-ac5b-4e4e-97dd-ea007915563c"",
                    ""path"": ""<Touchscreen>/touch*/press"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Touch"",
                    ""action"": ""Click"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""1603d67a-3a77-4f96-9056-781ba907e3c8"",
                    ""path"": ""<XRController>/trigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""XR"",
                    ""action"": ""Click"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e6eb5972-d916-475f-b647-7baef49aac76"",
                    ""path"": ""<Mouse>/scroll"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";Keyboard&Mouse"",
                    ""action"": ""ScrollWheel"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c15f95b3-7107-4342-a09c-33fdefe3ea1d"",
                    ""path"": ""<Mouse>/middleButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";Keyboard&Mouse"",
                    ""action"": ""MiddleClick"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""fc605c60-7ad3-41a3-88b8-8301ab5fc9ff"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";Keyboard&Mouse"",
                    ""action"": ""RightClick"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""44c3df4d-d347-49e6-8137-7683825d55d6"",
                    ""path"": ""<XRController>/devicePosition"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""XR"",
                    ""action"": ""TrackedDevicePosition"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""561a2149-2f3c-49a7-912a-788611cf938b"",
                    ""path"": ""<XRController>/deviceRotation"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""XR"",
                    ""action"": ""TrackedDeviceOrientation"",
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
            m_Player_MousePosition = m_Player.FindAction("Mouse Position", throwIfNotFound: true);
            m_Player_MouseDelta = m_Player.FindAction("Mouse Delta", throwIfNotFound: true);
            m_Player_LeftClick = m_Player.FindAction("Left Click", throwIfNotFound: true);
            m_Player_LeftMouse = m_Player.FindAction("Left Mouse", throwIfNotFound: true);
            m_Player_RightClick = m_Player.FindAction("Right Click", throwIfNotFound: true);
            m_Player_RightMouse = m_Player.FindAction("Right Mouse", throwIfNotFound: true);
            m_Player_MoveCamera = m_Player.FindAction("Move Camera", throwIfNotFound: true);
            m_Player_RotateCamera = m_Player.FindAction("Rotate Camera", throwIfNotFound: true);
            m_Player_ZoomCamera = m_Player.FindAction("Zoom Camera", throwIfNotFound: true);
            m_Player_ToggleBook = m_Player.FindAction("Toggle Book", throwIfNotFound: true);
            m_Player_ToggleTooltips = m_Player.FindAction("Toggle Tooltips", throwIfNotFound: true);
            m_Player_NextTurn = m_Player.FindAction("Next Turn", throwIfNotFound: true);
            m_Player_ConfirmSelectedStructure = m_Player.FindAction("Confirm Selected Structure", throwIfNotFound: true);
            m_Player_RotateBuilding = m_Player.FindAction("Rotate Building", throwIfNotFound: true);
            m_Player_SelectCards = m_Player.FindAction("Select Cards", throwIfNotFound: true);
            m_Player_DeselectCards = m_Player.FindAction("Deselect Cards", throwIfNotFound: true);
            m_Player_NavigateCards = m_Player.FindAction("Navigate Cards", throwIfNotFound: true);
            m_Player_SelectCardIndex = m_Player.FindAction("Select Card Index", throwIfNotFound: true);
            m_Player_Screenshot = m_Player.FindAction("Screenshot", throwIfNotFound: true);
            m_Player_NavigateBookmarks = m_Player.FindAction("Navigate Bookmarks", throwIfNotFound: true);
            m_Player_DialogueNext = m_Player.FindAction("Dialogue Next", throwIfNotFound: true);
            // UI
            m_UI = asset.FindActionMap("UI", throwIfNotFound: true);
            m_UI_Navigate = m_UI.FindAction("Navigate", throwIfNotFound: true);
            m_UI_Submit = m_UI.FindAction("Submit", throwIfNotFound: true);
            m_UI_Cancel = m_UI.FindAction("Cancel", throwIfNotFound: true);
            m_UI_Point = m_UI.FindAction("Point", throwIfNotFound: true);
            m_UI_Click = m_UI.FindAction("Click", throwIfNotFound: true);
            m_UI_ScrollWheel = m_UI.FindAction("ScrollWheel", throwIfNotFound: true);
            m_UI_MiddleClick = m_UI.FindAction("MiddleClick", throwIfNotFound: true);
            m_UI_RightClick = m_UI.FindAction("RightClick", throwIfNotFound: true);
            m_UI_TrackedDevicePosition = m_UI.FindAction("TrackedDevicePosition", throwIfNotFound: true);
            m_UI_TrackedDeviceOrientation = m_UI.FindAction("TrackedDeviceOrientation", throwIfNotFound: true);
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
        private readonly InputAction m_Player_MousePosition;
        private readonly InputAction m_Player_MouseDelta;
        private readonly InputAction m_Player_LeftClick;
        private readonly InputAction m_Player_LeftMouse;
        private readonly InputAction m_Player_RightClick;
        private readonly InputAction m_Player_RightMouse;
        private readonly InputAction m_Player_MoveCamera;
        private readonly InputAction m_Player_RotateCamera;
        private readonly InputAction m_Player_ZoomCamera;
        private readonly InputAction m_Player_ToggleBook;
        private readonly InputAction m_Player_ToggleTooltips;
        private readonly InputAction m_Player_NextTurn;
        private readonly InputAction m_Player_ConfirmSelectedStructure;
        private readonly InputAction m_Player_RotateBuilding;
        private readonly InputAction m_Player_SelectCards;
        private readonly InputAction m_Player_DeselectCards;
        private readonly InputAction m_Player_NavigateCards;
        private readonly InputAction m_Player_SelectCardIndex;
        private readonly InputAction m_Player_Screenshot;
        private readonly InputAction m_Player_NavigateBookmarks;
        private readonly InputAction m_Player_DialogueNext;
        public struct PlayerActions
        {
            private @PlayerInputs m_Wrapper;
            public PlayerActions(@PlayerInputs wrapper) { m_Wrapper = wrapper; }
            public InputAction @MousePosition => m_Wrapper.m_Player_MousePosition;
            public InputAction @MouseDelta => m_Wrapper.m_Player_MouseDelta;
            public InputAction @LeftClick => m_Wrapper.m_Player_LeftClick;
            public InputAction @LeftMouse => m_Wrapper.m_Player_LeftMouse;
            public InputAction @RightClick => m_Wrapper.m_Player_RightClick;
            public InputAction @RightMouse => m_Wrapper.m_Player_RightMouse;
            public InputAction @MoveCamera => m_Wrapper.m_Player_MoveCamera;
            public InputAction @RotateCamera => m_Wrapper.m_Player_RotateCamera;
            public InputAction @ZoomCamera => m_Wrapper.m_Player_ZoomCamera;
            public InputAction @ToggleBook => m_Wrapper.m_Player_ToggleBook;
            public InputAction @ToggleTooltips => m_Wrapper.m_Player_ToggleTooltips;
            public InputAction @NextTurn => m_Wrapper.m_Player_NextTurn;
            public InputAction @ConfirmSelectedStructure => m_Wrapper.m_Player_ConfirmSelectedStructure;
            public InputAction @RotateBuilding => m_Wrapper.m_Player_RotateBuilding;
            public InputAction @SelectCards => m_Wrapper.m_Player_SelectCards;
            public InputAction @DeselectCards => m_Wrapper.m_Player_DeselectCards;
            public InputAction @NavigateCards => m_Wrapper.m_Player_NavigateCards;
            public InputAction @SelectCardIndex => m_Wrapper.m_Player_SelectCardIndex;
            public InputAction @Screenshot => m_Wrapper.m_Player_Screenshot;
            public InputAction @NavigateBookmarks => m_Wrapper.m_Player_NavigateBookmarks;
            public InputAction @DialogueNext => m_Wrapper.m_Player_DialogueNext;
            public InputActionMap Get() { return m_Wrapper.m_Player; }
            public void Enable() { Get().Enable(); }
            public void Disable() { Get().Disable(); }
            public bool enabled => Get().enabled;
            public static implicit operator InputActionMap(PlayerActions set) { return set.Get(); }
            public void SetCallbacks(IPlayerActions instance)
            {
                if (m_Wrapper.m_PlayerActionsCallbackInterface != null)
                {
                    @MousePosition.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMousePosition;
                    @MousePosition.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMousePosition;
                    @MousePosition.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMousePosition;
                    @MouseDelta.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMouseDelta;
                    @MouseDelta.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMouseDelta;
                    @MouseDelta.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMouseDelta;
                    @LeftClick.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnLeftClick;
                    @LeftClick.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnLeftClick;
                    @LeftClick.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnLeftClick;
                    @LeftMouse.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnLeftMouse;
                    @LeftMouse.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnLeftMouse;
                    @LeftMouse.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnLeftMouse;
                    @RightClick.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnRightClick;
                    @RightClick.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnRightClick;
                    @RightClick.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnRightClick;
                    @RightMouse.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnRightMouse;
                    @RightMouse.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnRightMouse;
                    @RightMouse.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnRightMouse;
                    @MoveCamera.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMoveCamera;
                    @MoveCamera.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMoveCamera;
                    @MoveCamera.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMoveCamera;
                    @RotateCamera.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnRotateCamera;
                    @RotateCamera.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnRotateCamera;
                    @RotateCamera.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnRotateCamera;
                    @ZoomCamera.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnZoomCamera;
                    @ZoomCamera.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnZoomCamera;
                    @ZoomCamera.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnZoomCamera;
                    @ToggleBook.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnToggleBook;
                    @ToggleBook.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnToggleBook;
                    @ToggleBook.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnToggleBook;
                    @ToggleTooltips.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnToggleTooltips;
                    @ToggleTooltips.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnToggleTooltips;
                    @ToggleTooltips.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnToggleTooltips;
                    @NextTurn.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnNextTurn;
                    @NextTurn.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnNextTurn;
                    @NextTurn.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnNextTurn;
                    @ConfirmSelectedStructure.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnConfirmSelectedStructure;
                    @ConfirmSelectedStructure.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnConfirmSelectedStructure;
                    @ConfirmSelectedStructure.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnConfirmSelectedStructure;
                    @RotateBuilding.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnRotateBuilding;
                    @RotateBuilding.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnRotateBuilding;
                    @RotateBuilding.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnRotateBuilding;
                    @SelectCards.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnSelectCards;
                    @SelectCards.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnSelectCards;
                    @SelectCards.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnSelectCards;
                    @DeselectCards.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnDeselectCards;
                    @DeselectCards.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnDeselectCards;
                    @DeselectCards.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnDeselectCards;
                    @NavigateCards.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnNavigateCards;
                    @NavigateCards.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnNavigateCards;
                    @NavigateCards.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnNavigateCards;
                    @SelectCardIndex.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnSelectCardIndex;
                    @SelectCardIndex.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnSelectCardIndex;
                    @SelectCardIndex.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnSelectCardIndex;
                    @Screenshot.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnScreenshot;
                    @Screenshot.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnScreenshot;
                    @Screenshot.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnScreenshot;
                    @NavigateBookmarks.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnNavigateBookmarks;
                    @NavigateBookmarks.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnNavigateBookmarks;
                    @NavigateBookmarks.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnNavigateBookmarks;
                    @DialogueNext.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnDialogueNext;
                    @DialogueNext.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnDialogueNext;
                    @DialogueNext.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnDialogueNext;
                }
                m_Wrapper.m_PlayerActionsCallbackInterface = instance;
                if (instance != null)
                {
                    @MousePosition.started += instance.OnMousePosition;
                    @MousePosition.performed += instance.OnMousePosition;
                    @MousePosition.canceled += instance.OnMousePosition;
                    @MouseDelta.started += instance.OnMouseDelta;
                    @MouseDelta.performed += instance.OnMouseDelta;
                    @MouseDelta.canceled += instance.OnMouseDelta;
                    @LeftClick.started += instance.OnLeftClick;
                    @LeftClick.performed += instance.OnLeftClick;
                    @LeftClick.canceled += instance.OnLeftClick;
                    @LeftMouse.started += instance.OnLeftMouse;
                    @LeftMouse.performed += instance.OnLeftMouse;
                    @LeftMouse.canceled += instance.OnLeftMouse;
                    @RightClick.started += instance.OnRightClick;
                    @RightClick.performed += instance.OnRightClick;
                    @RightClick.canceled += instance.OnRightClick;
                    @RightMouse.started += instance.OnRightMouse;
                    @RightMouse.performed += instance.OnRightMouse;
                    @RightMouse.canceled += instance.OnRightMouse;
                    @MoveCamera.started += instance.OnMoveCamera;
                    @MoveCamera.performed += instance.OnMoveCamera;
                    @MoveCamera.canceled += instance.OnMoveCamera;
                    @RotateCamera.started += instance.OnRotateCamera;
                    @RotateCamera.performed += instance.OnRotateCamera;
                    @RotateCamera.canceled += instance.OnRotateCamera;
                    @ZoomCamera.started += instance.OnZoomCamera;
                    @ZoomCamera.performed += instance.OnZoomCamera;
                    @ZoomCamera.canceled += instance.OnZoomCamera;
                    @ToggleBook.started += instance.OnToggleBook;
                    @ToggleBook.performed += instance.OnToggleBook;
                    @ToggleBook.canceled += instance.OnToggleBook;
                    @ToggleTooltips.started += instance.OnToggleTooltips;
                    @ToggleTooltips.performed += instance.OnToggleTooltips;
                    @ToggleTooltips.canceled += instance.OnToggleTooltips;
                    @NextTurn.started += instance.OnNextTurn;
                    @NextTurn.performed += instance.OnNextTurn;
                    @NextTurn.canceled += instance.OnNextTurn;
                    @ConfirmSelectedStructure.started += instance.OnConfirmSelectedStructure;
                    @ConfirmSelectedStructure.performed += instance.OnConfirmSelectedStructure;
                    @ConfirmSelectedStructure.canceled += instance.OnConfirmSelectedStructure;
                    @RotateBuilding.started += instance.OnRotateBuilding;
                    @RotateBuilding.performed += instance.OnRotateBuilding;
                    @RotateBuilding.canceled += instance.OnRotateBuilding;
                    @SelectCards.started += instance.OnSelectCards;
                    @SelectCards.performed += instance.OnSelectCards;
                    @SelectCards.canceled += instance.OnSelectCards;
                    @DeselectCards.started += instance.OnDeselectCards;
                    @DeselectCards.performed += instance.OnDeselectCards;
                    @DeselectCards.canceled += instance.OnDeselectCards;
                    @NavigateCards.started += instance.OnNavigateCards;
                    @NavigateCards.performed += instance.OnNavigateCards;
                    @NavigateCards.canceled += instance.OnNavigateCards;
                    @SelectCardIndex.started += instance.OnSelectCardIndex;
                    @SelectCardIndex.performed += instance.OnSelectCardIndex;
                    @SelectCardIndex.canceled += instance.OnSelectCardIndex;
                    @Screenshot.started += instance.OnScreenshot;
                    @Screenshot.performed += instance.OnScreenshot;
                    @Screenshot.canceled += instance.OnScreenshot;
                    @NavigateBookmarks.started += instance.OnNavigateBookmarks;
                    @NavigateBookmarks.performed += instance.OnNavigateBookmarks;
                    @NavigateBookmarks.canceled += instance.OnNavigateBookmarks;
                    @DialogueNext.started += instance.OnDialogueNext;
                    @DialogueNext.performed += instance.OnDialogueNext;
                    @DialogueNext.canceled += instance.OnDialogueNext;
                }
            }
        }
        public PlayerActions @Player => new PlayerActions(this);

        // UI
        private readonly InputActionMap m_UI;
        private IUIActions m_UIActionsCallbackInterface;
        private readonly InputAction m_UI_Navigate;
        private readonly InputAction m_UI_Submit;
        private readonly InputAction m_UI_Cancel;
        private readonly InputAction m_UI_Point;
        private readonly InputAction m_UI_Click;
        private readonly InputAction m_UI_ScrollWheel;
        private readonly InputAction m_UI_MiddleClick;
        private readonly InputAction m_UI_RightClick;
        private readonly InputAction m_UI_TrackedDevicePosition;
        private readonly InputAction m_UI_TrackedDeviceOrientation;
        public struct UIActions
        {
            private @PlayerInputs m_Wrapper;
            public UIActions(@PlayerInputs wrapper) { m_Wrapper = wrapper; }
            public InputAction @Navigate => m_Wrapper.m_UI_Navigate;
            public InputAction @Submit => m_Wrapper.m_UI_Submit;
            public InputAction @Cancel => m_Wrapper.m_UI_Cancel;
            public InputAction @Point => m_Wrapper.m_UI_Point;
            public InputAction @Click => m_Wrapper.m_UI_Click;
            public InputAction @ScrollWheel => m_Wrapper.m_UI_ScrollWheel;
            public InputAction @MiddleClick => m_Wrapper.m_UI_MiddleClick;
            public InputAction @RightClick => m_Wrapper.m_UI_RightClick;
            public InputAction @TrackedDevicePosition => m_Wrapper.m_UI_TrackedDevicePosition;
            public InputAction @TrackedDeviceOrientation => m_Wrapper.m_UI_TrackedDeviceOrientation;
            public InputActionMap Get() { return m_Wrapper.m_UI; }
            public void Enable() { Get().Enable(); }
            public void Disable() { Get().Disable(); }
            public bool enabled => Get().enabled;
            public static implicit operator InputActionMap(UIActions set) { return set.Get(); }
            public void SetCallbacks(IUIActions instance)
            {
                if (m_Wrapper.m_UIActionsCallbackInterface != null)
                {
                    @Navigate.started -= m_Wrapper.m_UIActionsCallbackInterface.OnNavigate;
                    @Navigate.performed -= m_Wrapper.m_UIActionsCallbackInterface.OnNavigate;
                    @Navigate.canceled -= m_Wrapper.m_UIActionsCallbackInterface.OnNavigate;
                    @Submit.started -= m_Wrapper.m_UIActionsCallbackInterface.OnSubmit;
                    @Submit.performed -= m_Wrapper.m_UIActionsCallbackInterface.OnSubmit;
                    @Submit.canceled -= m_Wrapper.m_UIActionsCallbackInterface.OnSubmit;
                    @Cancel.started -= m_Wrapper.m_UIActionsCallbackInterface.OnCancel;
                    @Cancel.performed -= m_Wrapper.m_UIActionsCallbackInterface.OnCancel;
                    @Cancel.canceled -= m_Wrapper.m_UIActionsCallbackInterface.OnCancel;
                    @Point.started -= m_Wrapper.m_UIActionsCallbackInterface.OnPoint;
                    @Point.performed -= m_Wrapper.m_UIActionsCallbackInterface.OnPoint;
                    @Point.canceled -= m_Wrapper.m_UIActionsCallbackInterface.OnPoint;
                    @Click.started -= m_Wrapper.m_UIActionsCallbackInterface.OnClick;
                    @Click.performed -= m_Wrapper.m_UIActionsCallbackInterface.OnClick;
                    @Click.canceled -= m_Wrapper.m_UIActionsCallbackInterface.OnClick;
                    @ScrollWheel.started -= m_Wrapper.m_UIActionsCallbackInterface.OnScrollWheel;
                    @ScrollWheel.performed -= m_Wrapper.m_UIActionsCallbackInterface.OnScrollWheel;
                    @ScrollWheel.canceled -= m_Wrapper.m_UIActionsCallbackInterface.OnScrollWheel;
                    @MiddleClick.started -= m_Wrapper.m_UIActionsCallbackInterface.OnMiddleClick;
                    @MiddleClick.performed -= m_Wrapper.m_UIActionsCallbackInterface.OnMiddleClick;
                    @MiddleClick.canceled -= m_Wrapper.m_UIActionsCallbackInterface.OnMiddleClick;
                    @RightClick.started -= m_Wrapper.m_UIActionsCallbackInterface.OnRightClick;
                    @RightClick.performed -= m_Wrapper.m_UIActionsCallbackInterface.OnRightClick;
                    @RightClick.canceled -= m_Wrapper.m_UIActionsCallbackInterface.OnRightClick;
                    @TrackedDevicePosition.started -= m_Wrapper.m_UIActionsCallbackInterface.OnTrackedDevicePosition;
                    @TrackedDevicePosition.performed -= m_Wrapper.m_UIActionsCallbackInterface.OnTrackedDevicePosition;
                    @TrackedDevicePosition.canceled -= m_Wrapper.m_UIActionsCallbackInterface.OnTrackedDevicePosition;
                    @TrackedDeviceOrientation.started -= m_Wrapper.m_UIActionsCallbackInterface.OnTrackedDeviceOrientation;
                    @TrackedDeviceOrientation.performed -= m_Wrapper.m_UIActionsCallbackInterface.OnTrackedDeviceOrientation;
                    @TrackedDeviceOrientation.canceled -= m_Wrapper.m_UIActionsCallbackInterface.OnTrackedDeviceOrientation;
                }
                m_Wrapper.m_UIActionsCallbackInterface = instance;
                if (instance != null)
                {
                    @Navigate.started += instance.OnNavigate;
                    @Navigate.performed += instance.OnNavigate;
                    @Navigate.canceled += instance.OnNavigate;
                    @Submit.started += instance.OnSubmit;
                    @Submit.performed += instance.OnSubmit;
                    @Submit.canceled += instance.OnSubmit;
                    @Cancel.started += instance.OnCancel;
                    @Cancel.performed += instance.OnCancel;
                    @Cancel.canceled += instance.OnCancel;
                    @Point.started += instance.OnPoint;
                    @Point.performed += instance.OnPoint;
                    @Point.canceled += instance.OnPoint;
                    @Click.started += instance.OnClick;
                    @Click.performed += instance.OnClick;
                    @Click.canceled += instance.OnClick;
                    @ScrollWheel.started += instance.OnScrollWheel;
                    @ScrollWheel.performed += instance.OnScrollWheel;
                    @ScrollWheel.canceled += instance.OnScrollWheel;
                    @MiddleClick.started += instance.OnMiddleClick;
                    @MiddleClick.performed += instance.OnMiddleClick;
                    @MiddleClick.canceled += instance.OnMiddleClick;
                    @RightClick.started += instance.OnRightClick;
                    @RightClick.performed += instance.OnRightClick;
                    @RightClick.canceled += instance.OnRightClick;
                    @TrackedDevicePosition.started += instance.OnTrackedDevicePosition;
                    @TrackedDevicePosition.performed += instance.OnTrackedDevicePosition;
                    @TrackedDevicePosition.canceled += instance.OnTrackedDevicePosition;
                    @TrackedDeviceOrientation.started += instance.OnTrackedDeviceOrientation;
                    @TrackedDeviceOrientation.performed += instance.OnTrackedDeviceOrientation;
                    @TrackedDeviceOrientation.canceled += instance.OnTrackedDeviceOrientation;
                }
            }
        }
        public UIActions @UI => new UIActions(this);
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
            void OnMousePosition(InputAction.CallbackContext context);
            void OnMouseDelta(InputAction.CallbackContext context);
            void OnLeftClick(InputAction.CallbackContext context);
            void OnLeftMouse(InputAction.CallbackContext context);
            void OnRightClick(InputAction.CallbackContext context);
            void OnRightMouse(InputAction.CallbackContext context);
            void OnMoveCamera(InputAction.CallbackContext context);
            void OnRotateCamera(InputAction.CallbackContext context);
            void OnZoomCamera(InputAction.CallbackContext context);
            void OnToggleBook(InputAction.CallbackContext context);
            void OnToggleTooltips(InputAction.CallbackContext context);
            void OnNextTurn(InputAction.CallbackContext context);
            void OnConfirmSelectedStructure(InputAction.CallbackContext context);
            void OnRotateBuilding(InputAction.CallbackContext context);
            void OnSelectCards(InputAction.CallbackContext context);
            void OnDeselectCards(InputAction.CallbackContext context);
            void OnNavigateCards(InputAction.CallbackContext context);
            void OnSelectCardIndex(InputAction.CallbackContext context);
            void OnScreenshot(InputAction.CallbackContext context);
            void OnNavigateBookmarks(InputAction.CallbackContext context);
            void OnDialogueNext(InputAction.CallbackContext context);
        }
        public interface IUIActions
        {
            void OnNavigate(InputAction.CallbackContext context);
            void OnSubmit(InputAction.CallbackContext context);
            void OnCancel(InputAction.CallbackContext context);
            void OnPoint(InputAction.CallbackContext context);
            void OnClick(InputAction.CallbackContext context);
            void OnScrollWheel(InputAction.CallbackContext context);
            void OnMiddleClick(InputAction.CallbackContext context);
            void OnRightClick(InputAction.CallbackContext context);
            void OnTrackedDevicePosition(InputAction.CallbackContext context);
            void OnTrackedDeviceOrientation(InputAction.CallbackContext context);
        }
    }
}
