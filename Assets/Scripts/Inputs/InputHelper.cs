using Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using Utilities;
using static Managers.GameManager;

namespace Inputs
{
    // The sole purpose of this script is to help give access to PlayerInput
    public class InputHelper : MonoBehaviour
    {
        public static PlayerInput PlayerInput;
        public static EventSystem EventSystem;
        public static Action<GameObject> OnNewSelection;
        public static Dictionary<GameObject, Vector2> CursorOffsetOverrides = new Dictionary<GameObject, Vector2>();

        [SerializeField] private RectTransform selectionHelper;

        private GameObject worldSpaceCursor;
        private GameObject lastSelectedGameObject;
        private Dictionary<GameState, GameObject> previousSelections = new Dictionary<GameState, GameObject>();

        // Start is called before the first frame update
        void Start()
        {
            EventSystem = FindObjectOfType<EventSystem>();
            PlayerInput = GetComponent<PlayerInput>();

            State.OnEnterState += StateChecks;
            Inputs.OnControlChange += OnControlChanged;
            OnNewSelection += NewSelection;
            UIController.OnUIOpen += (g, b) =>
            {
                EventSystem.SetSelectedGameObject(g);
                selectionHelper.gameObject.SetActive(b);
            };

            worldSpaceCursor = GetComponent<Cinemachine.CinemachineFreeLook>().m_Follow.GetChild(0).gameObject;
            Manager.Inputs.WorldSpaceCursor = worldSpaceCursor.transform;
            worldSpaceCursor.SetActive(false);
            selectionHelper.gameObject.SetActive(false);

        }

        private void Update()
        {
            if (EventSystem.currentSelectedGameObject != lastSelectedGameObject)
            {
                OnNewSelection?.Invoke(EventSystem.currentSelectedGameObject);
            }
                
            lastSelectedGameObject = EventSystem.currentSelectedGameObject;
        }

        private void NewSelection(GameObject obj)
        {
            if (Manager.Inputs.UsingController && EventSystem.currentSelectedGameObject != null)
            {
                previousSelections[Manager.State.Current] = obj;
                var rt = obj.transform as RectTransform;
                var pos = rt.transform.position;
                if(CursorOffsetOverrides.ContainsKey(obj))
                    pos += (Vector3)CursorOffsetOverrides[obj];
                selectionHelper.anchoredPosition = pos;
            }
        }

        private void OnControlChanged(InputControlScheme obj)
        {
            bool isController = Manager.Inputs.UsingController;
            if(Manager.State.Current == GameState.InGame)
                worldSpaceCursor.gameObject.SetActive(isController);
            if (Manager.State.Current == GameState.InMenu)
            {
                selectionHelper.gameObject.SetActive(isController);
                Cursor.visible = !isController;
            }
        }

        public void ResetSelection(GameState state)
        {
            previousSelections[state] = null;
        }

        // I'll fill this up with stuff when I can.
        private void StateChecks(GameState state)
        {
            if (Manager.Inputs.UsingController)
            {
                switch (state)
                {
                    case GameState.InCredits:
                        EventSystem.SetSelectedGameObject(null);
                        break;
                    case GameState.InIntro:
                        break;
                    case GameState.ToCredits:
                        EventSystem.SetSelectedGameObject(null);
                        break;
                    case GameState.Loading:
                        break;
                    case GameState.ToIntro:
                        break;
                    case GameState.ToGame:
                        EventSystem.SetSelectedGameObject(null);
                        selectionHelper.gameObject.SetActive(false);
                        break;
                    case GameState.InGame:
                        if (Manager.Inputs.UsingController)
                        {
                            worldSpaceCursor.gameObject.SetActive(true);
                            selectionHelper.gameObject.SetActive(false);
                        }
                        break;
                    case GameState.NextTurn:
                        break;
                    case GameState.InMenu:
                        break;
                    case GameState.EndGame:
                        break;
                    case GameState.InDialogue:
                        break;
                }
            }
            else
            {
                EventSystem.SetSelectedGameObject(null);
            }
        }
    }
}
