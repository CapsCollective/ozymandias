using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static Managers.GameManager;

public class UIController : MonoBehaviour
{
    public static Action<GameObject, bool> OnUIOpen;
    public static Action OnUIClose;

    [SerializeField] private bool showCursor = true;
    [SerializeField] private GameObject firstSelected;
    private GameObject currentSelected;

    public void OnOpen()
    {
        Inputs.Inputs.OnControlChange += ControllerFocus;
        OnUIOpen?.Invoke(firstSelected, showCursor);
    }

    public void OnClose()
    {
        Inputs.Inputs.OnControlChange -= ControllerFocus;
        OnUIClose?.Invoke();
    }

    public void ControllerFocus(InputControlScheme scheme)
    {
        if(scheme == Manager.Inputs.PlayerInput.ControllerScheme)
        {
            OnUIOpen.Invoke(firstSelected, showCursor);
        }
    }
}
