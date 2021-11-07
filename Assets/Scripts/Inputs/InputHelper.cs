using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static Managers.GameManager;

namespace Inputs
{
    // The sole purpose of this script is to help give access to PlayerInput
    public class InputSwapper : MonoBehaviour
    {
        public static PlayerInput PlayerInput;

        // Start is called before the first frame update
        void Start()
        {
            PlayerInput = GetComponent<PlayerInput>();
        }
    }
}
