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

        // Start is called before the first frame update
        void Awake()
        {
            EventSystem = FindObjectOfType<EventSystem>();
            PlayerInput = GetComponent<PlayerInput>();

            State.OnEnterState += AutoSelect;
        }

        // I'll fill this up with stuff when I can.
        private void AutoSelect(GameState state)
        {
            switch (state)
            {
                case GameState.InCredits:
                    EventSystem.SetSelectedGameObject(null);
                    break;
                case GameState.InIntro:
                    EventSystem.SetSelectedGameObject(null);
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
    }
}
