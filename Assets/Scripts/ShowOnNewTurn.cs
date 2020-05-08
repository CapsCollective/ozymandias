using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowOnNewTurn : MonoBehaviour
{
    void Awake()
    {
        GameManager.OnNewTurn += () => gameObject.SetActive(true);
    }

    void Start()
    {
        gameObject.SetActive(false);
    }

}
