using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UI;
using Controllers;

public class UIEventController : MonoBehaviour
{
    private int lastSelectedCard = 1;
    private bool isSelectingCard = false;
    private EventSystem eventSystem;
    private BuildingPlacement buildingPlacement;

    // Start is called before the first frame update
    void Start()
    {
        eventSystem = GetComponent<EventSystem>();
        buildingPlacement = FindObjectOfType<BuildingPlacement>();
        InputManager.Instance.IA_SelectCards.performed += SelectCards;
        InputManager.Instance.IA_UINavigate.performed += Navigate;
        InputManager.Instance.IA_UICancel.performed += UICancel;
    }

    private void UICancel(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (isSelectingCard)
        {
            buildingPlacement.ImitateHover(-1);
            isSelectingCard = false;
        }
    }

    public void Navigate(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        int dir = (int)obj.ReadValue<Vector2>().x;
        if (isSelectingCard)
        {
            lastSelectedCard = buildingPlacement.NavigateCards(lastSelectedCard + dir);
            Debug.Log($"{lastSelectedCard} / {dir}");
            buildingPlacement.ImitateHover(lastSelectedCard);
        }
    }

    private void SelectCards(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (!isSelectingCard)
        {
            buildingPlacement.ImitateHover(lastSelectedCard);
            isSelectingCard = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
    }
}
