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

        [SerializeField] private SerializedDictionary<GameState, GameObject> firstSelections = new SerializedDictionary<GameState, GameObject>();
        
        private GameObject lastSelectedGameObject;
        private Dictionary<GameState, GameObject> previousSelections = new Dictionary<GameState, GameObject>();

        // Start is called before the first frame update
        void Awake()
        {
            EventSystem = FindObjectOfType<EventSystem>();
            PlayerInput = GetComponent<PlayerInput>();

            State.OnEnterState += AutoSelect;
            Inputs.OnControlChange += AutoSelectControl;
            OnNewSelection += NewSelection;
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
            if(Manager.Inputs.UsingController)
                previousSelections[Manager.State.Current] = obj;   
        }

        private void AutoSelectControl(InputControlScheme obj)
        {
            if (previousSelections[Manager.State.Current] == null)
                AutoSelect(Manager.State.Current);
            else
            {
                EventSystem.SetSelectedGameObject(previousSelections[Manager.State.Current]);
            }
        }

        public void ResetSelection(GameState state)
        {
            previousSelections[state] = null;
        }

        // I'll fill this up with stuff when I can.
        private void AutoSelect(GameState state)
        {
            if (Manager.Inputs.UsingController)
            {
                switch (state)
                {
                    case GameState.InCredits:
                        EventSystem.SetSelectedGameObject(null);
                        break;
                    case GameState.InIntro:
                        EventSystem.SetSelectedGameObject(firstSelections[state]);
                        break;
                    case GameState.ToCredits:
                        EventSystem.SetSelectedGameObject(null);
                        break;
                    case GameState.Loading:
                        break;
                    case GameState.ToIntro:
                        break;
                    case GameState.ToGame:
                        break;
                    case GameState.InGame:
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
