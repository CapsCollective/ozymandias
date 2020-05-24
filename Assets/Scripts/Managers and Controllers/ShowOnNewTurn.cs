using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowOnNewTurn : MonoBehaviour
{
    void Awake()
    {
        GameManager.OnNewTurn += OnNewTurn;
    }

    private void OnDestroy()
    {
        GameManager.OnNewTurn -= OnNewTurn;
    }

    void Start()
    {
        GetComponent<Canvas>().enabled = false;
    }

    private void OnNewTurn()
    {
        GetComponent<Canvas>().enabled = true;
    }
}
