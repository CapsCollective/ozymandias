using Buildings;
using UnityEngine;
using UnityEngine.EventSystems;
using static GameState.GameManager;

namespace Inputs
{
    [RequireComponent(typeof(EventSystem))]
    public class UIEventController : MonoBehaviour
    {
        private static EventSystem _eventSystem;

        [SerializeField] private Canvas optionsMenu;
        [SerializeField] private GameObject optionsFirstSelected;

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
            Manager.Inputs.IA_SelectCards.performed += SelectCards;
            Manager.Inputs.IA_UINavigate.performed += Navigate;
            Manager.Inputs.IA_UICancel.performed += UICancel;
            Manager.Inputs.IA_ShowPause.performed += ShowPause;
            Manager.Inputs.IA_DeselectCards.performed += UICancel;
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
            if (Manager.InMenu) return;

            if (isSelectingCard)
            {
                _place.ImitateHover(-1);
                isSelectingCard = false;
            }
        }

        public void Navigate(UnityEngine.InputSystem.InputAction.CallbackContext obj)
        {
            if (Manager.InMenu || _place.ChangingCard(lastSelectedCard)) return;

            int dir = (int)obj.ReadValue<Vector2>().x;
            if (isSelectingCard)
            {
                lastSelectedCard = _place.NavigateCards(lastSelectedCard + dir);
                _place.ImitateHover(lastSelectedCard);
            }
        }

        private void SelectCards(UnityEngine.InputSystem.InputAction.CallbackContext obj)
        {
            if (Manager.InMenu) return;

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
