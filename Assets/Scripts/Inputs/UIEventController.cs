using Buildings;
using UnityEngine;
using UnityEngine.EventSystems;
using Utilities;
using static Managers.GameManager;

namespace Inputs
{
    [RequireComponent(typeof(EventSystem))]
    public class UIEventController : MonoBehaviour
    {
        private static EventSystem _eventSystem;

        private int lastSelectedCard = 1;
        private bool isSelectingCard = false;
        private Place _place;

        // Start is called before the first frame update
        private void Awake()
        {
            _eventSystem = GetComponent<EventSystem>();
            _place = FindObjectOfType<Place>();
            Place.OnBuildingPlaced += OnBuildingPlaced;
        }

        private void Start()
        {
            Manager.Inputs.OnSelectCards.performed += SelectCards;
            Manager.Inputs.OnUINavigate.performed += Navigate;
            Manager.Inputs.OnUICancel.performed += UICancel;
            Manager.Inputs.OnDeselectCards.performed += UICancel;
        }

        private void OnBuildingPlaced()
        {
            isSelectingCard = false;
        }
        
        private void UICancel(UnityEngine.InputSystem.InputAction.CallbackContext obj)
        {
            if (Manager.State.InIntro) return;

            if (isSelectingCard)
            {
                _place.ImitateHover(-1);
                isSelectingCard = false;
            }
        }

        private void Navigate(UnityEngine.InputSystem.InputAction.CallbackContext obj)
        {
            if (Manager.State.InIntro || _place.ChangingCard(lastSelectedCard)) return;

            int dir = (int)obj.ReadValue<Vector2>().x;
            if (isSelectingCard)
            {
                lastSelectedCard = _place.NavigateCards(lastSelectedCard + dir);
                _place.ImitateHover(lastSelectedCard);
            }
        }

        private void SelectCards(UnityEngine.InputSystem.InputAction.CallbackContext obj)
        {
            if (!Manager.State.InGame) return;

            if (!isSelectingCard)
            {
                _place.ImitateHover(lastSelectedCard);
                isSelectingCard = true;
            }
        }

        public static void SelectUI(GameObject gameObject)
        {
            _eventSystem.SetSelectedGameObject(gameObject);
        }
    }
}
