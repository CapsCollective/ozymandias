using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UI;
using Controllers;
using static Managers.GameManager;
using System;

public class UIEventController : MonoBehaviour
{
    private static EventSystem eventSystem;

    [SerializeField] private Canvas optionsMenu;
    [SerializeField] private GameObject optionsFirstSelected;

    private int lastSelectedCard = 1;
    private bool isSelectingCard = false;
    private BuildingPlacement buildingPlacement;

    // Start is called before the first frame update
    void Start()
    {
        eventSystem = GetComponent<EventSystem>();
        buildingPlacement = FindObjectOfType<BuildingPlacement>();
        InputManager.Instance.IA_SelectCards.performed += SelectCards;
        InputManager.Instance.IA_UINavigate.performed += Navigate;
        InputManager.Instance.IA_UICancel.performed += UICancel;
        InputManager.Instance.IA_ShowPause.performed += ShowPause;
        BuildingPlacement.OnBuildingPlaced += OnBuildingPlaced;
    }

    private void OnBuildingPlaced()
    {
        isSelectingCard = false;
    }

    private void ShowPause(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (!optionsMenu.enabled)
        {
            optionsMenu.enabled = true;
            Manager.EnterMenu();
            SelectUI(optionsFirstSelected);
        }
        else
        {
            optionsMenu.enabled = false;
            Manager.ExitMenu();
            SelectUI(null);
        }
    }

    private void UICancel(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (Manager.inMenu) return;

        if (isSelectingCard)
        {
            buildingPlacement.ImitateHover(-1);
            isSelectingCard = false;
        }
    }

    public void Navigate(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (Manager.inMenu || buildingPlacement.ChangingCard(lastSelectedCard)) return;

        int dir = (int)obj.ReadValue<Vector2>().x;
        if (isSelectingCard)
        {
            lastSelectedCard = buildingPlacement.NavigateCards(lastSelectedCard + dir);
            buildingPlacement.ImitateHover(lastSelectedCard);
        }
    }

    private void SelectCards(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (Manager.inMenu) return;

        if (!isSelectingCard)
        {
            buildingPlacement.ImitateHover(lastSelectedCard);
            isSelectingCard = true;
        }
    }

    public static void SelectUI(GameObject gameObject)
    {
        eventSystem.SetSelectedGameObject(gameObject);
    }
}
