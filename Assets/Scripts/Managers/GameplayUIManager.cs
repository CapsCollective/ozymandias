using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameplayUIManager : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;

    // Start is called before the first frame update
    void Start()
    {
        Managers.GameManager.OnNewTurn += OnNewTurn;
        Managers.GameManager.OnNextTurn += OnNextTurn;
    }

    private void OnNewTurn()
    {
        canvasGroup.interactable = true;
    }

    private void OnNextTurn()
    {
        canvasGroup.interactable = false;
    }
}
