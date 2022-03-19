using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Utilities;
using Inputs;
using static Managers.GameManager;

public class UIController : MonoBehaviour
{
    public static Action<GameObject, bool> OnUIOpen;
    public static Action OnUIClose;

    [SerializeField] private bool showCursor = true;
    [SerializeField] private GameObject firstSelected;
    private GameObject currentSelected;

    [SerializeField] private SerializedDictionary<GameObject, Vector2> cursorOffsetOverrides = new SerializedDictionary<GameObject, Vector2>();

    private void Awake()
    {
        foreach (KeyValuePair<GameObject, Vector2> pair in cursorOffsetOverrides)
        {
            InputHelper.CursorOffsetOverrides.Add(pair.Key, pair.Value);
        }
    }

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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        foreach(KeyValuePair<GameObject, Vector2> pair in cursorOffsetOverrides)
        {
            Gizmos.DrawSphere(pair.Key.transform.position + (Vector3)pair.Value, 10.0f);
        }
    }
}
