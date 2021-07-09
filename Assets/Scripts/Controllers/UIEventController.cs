using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UI;

public class UIEventController : MonoBehaviour
{
    [SerializeField] GameObject lastSelectedCard;

    private bool isSelectingCard = false;
    private EventSystem eventSystem;

    // Start is called before the first frame update
    void Start()
    {
        eventSystem = GetComponent<EventSystem>();
        InputManager.Instance.SelectCards.performed += SelectCards;
        InputManager.Instance.UINavigate.canceled += Navigate;
    }

    public void Navigate(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (isSelectingCard)
        {
            lastSelectedCard.GetComponent<BuildingCard>().DeselectCard();
            lastSelectedCard.GetComponent<BuildingCard>().toggle.isOn = false;
            lastSelectedCard = eventSystem.currentSelectedGameObject;
            lastSelectedCard.GetComponent<BuildingCard>().SelectCard();
            lastSelectedCard.GetComponent<BuildingCard>().toggle.isOn = true;
        }
    }

    private void SelectCards(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (!isSelectingCard)
        {
            eventSystem.SetSelectedGameObject(lastSelectedCard.gameObject);
            lastSelectedCard.GetComponent<BuildingCard>().SelectCard();
            isSelectingCard = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
    }
}
