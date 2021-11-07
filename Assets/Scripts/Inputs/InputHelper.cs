using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
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
        }
    }
}
