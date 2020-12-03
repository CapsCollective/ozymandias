using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class TurnEventController : MonoBehaviour
{

    public UnityEvent OnNewTurnPressed;
    public UnityEvent OnNewTurn;

    // Start is called before the first frame update
    void Start()
    {
        GameManager.OnNextTurn += InvokeNewTurn;
        GameManager.OnNewTurn += InvokeEventsProcessed;
    }

    private void InvokeNewTurn() => OnNewTurnPressed.Invoke();

    private void InvokeEventsProcessed() => OnNewTurn.Invoke();
    
    //TODO: Move Game manager turn stuff here
}
