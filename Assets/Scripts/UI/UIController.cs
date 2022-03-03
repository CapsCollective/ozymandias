using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public static Action<GameObject> OnUIOpen;
    public static Action OnUIClose;

    [SerializeField] private GameObject firstSelected;
    private GameObject currentSelected;

    public void OnOpen()
    {
        OnUIOpen?.Invoke(firstSelected);
    }

    public void OnClose()
    {
        OnUIClose?.Invoke();
    }
}
