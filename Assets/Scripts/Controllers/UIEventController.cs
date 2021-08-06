using UnityEngine;
using UnityEngine.EventSystems;
using static Managers.GameManager;

namespace Controllers
{
    public class UIEventController : MonoBehaviour
    {
        private static EventSystem eventSystem;

        [SerializeField] private Canvas optionsMenu;
        [SerializeField] private GameObject optionsFirstSelected;

        private int lastSelectedCard = 1;
        private bool isSelectingCard = false;
        private BuildingPlacement buildingPlacement;

        // Start is called before the first frame update
        private void Start()
        {
            eventSystem = GetComponent<EventSystem>();
            buildingPlacement = FindObjectOfType<BuildingPlacement>();
            Manager.Inputs.IA_SelectCards.performed += SelectCards;
            Manager.Inputs.IA_UINavigate.performed += Navigate;
            Manager.Inputs.IA_UICancel.performed += UICancel;
            Manager.Inputs.IA_ShowPause.performed += ShowPause;
            Manager.Inputs.IA_DeselectCards.performed += UICancel;
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
            if (Manager.InMenu) return;

            if (isSelectingCard)
            {
                buildingPlacement.ImitateHover(-1);
                isSelectingCard = false;
            }
        }

        public void Navigate(UnityEngine.InputSystem.InputAction.CallbackContext obj)
        {
            if (Manager.InMenu || buildingPlacement.ChangingCard(lastSelectedCard)) return;

            int dir = (int)obj.ReadValue<Vector2>().x;
            if (isSelectingCard)
            {
                lastSelectedCard = buildingPlacement.NavigateCards(lastSelectedCard + dir);
                buildingPlacement.ImitateHover(lastSelectedCard);
            }
        }

        private void SelectCards(UnityEngine.InputSystem.InputAction.CallbackContext obj)
        {
            if (Manager.InMenu) return;

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
}
