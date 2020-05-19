using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowOnNewTurn : MonoBehaviour
{
    void Awake()
    {
        GameManager.OnNewTurn += () => GetComponent<Canvas>().enabled = true;
    }

    void Start()
    {
        GetComponent<Canvas>().enabled = false;
    }

}
