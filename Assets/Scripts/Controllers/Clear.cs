using Entities;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utilities;
using static Managers.GameManager;
using static Controllers.CursorSelect;

namespace Controllers
{
    public class Clear : UiUpdater
    {
        private const float CostScale = 1.025f;
        public static int ClearCount;

        private Cell[] _highlighted = new Cell[1];
        private EventSystem _eventSystem;
        private Building _selectedBuilding;
    
        [SerializeField] private int baseCost = 30;

        [SerializeField] private Image icon;
        [SerializeField] private Sprite deselected;
        [SerializeField] private Sprite selected;
        
        [SerializeField] private Image costBadge;
        [SerializeField] private Color gold, grey;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private TextMeshProUGUI cost;

        public Toggle toggle;
        
        private int ScaledCost => Mathf.FloorToInt( baseCost * Mathf.Pow(CostScale, ClearCount));

        protected override void UpdateUi()
        {
            cost.text = ScaledCost.ToString();
            bool active = Manager.Wealth >= ScaledCost;
            if (!active)
            {
                toggle.isOn = false;
                ExitClearMode();
            }
            toggle.interactable = active;
            costBadge.color = active ? gold : grey;
            canvasGroup.alpha = active ? 1 : 0.4f;
        }
    
        private void Start()
        {
            _eventSystem = EventSystem.current;

            Click.OnLeftClick += LeftClick;
        }

        // Update is called once per frame
        void Update()
        {
            _selectedBuilding = null;
            if (!toggle.isOn) return;
        
            Manager.Map.Highlight(_highlighted, Map.HighlightState.Inactive);
            _highlighted = new Cell[1];
        
            if (_eventSystem.IsPointerOverGameObject()) return;
        
            Cell closest = Manager.Map.GetCellFromMouse();

            _selectedBuilding = closest.occupant;
            if (_selectedBuilding) _highlighted = Manager.Map.GetCells(_selectedBuilding);
            else _highlighted[0] = closest;
        
            Map.HighlightState state = _selectedBuilding && !_selectedBuilding.indestructible ? Map.HighlightState.Valid : Map.HighlightState.Invalid;
            Manager.Map.Highlight(_highlighted, state);
        }

        private void LeftClick()
        {
            if (toggle.isOn && _selectedBuilding) ClearBuilding();
        }
    
        public void ToggleClearMode()
        {
            if (!toggle.isOn) ExitClearMode();
            else EnterClearMode();
        }

        private void EnterClearMode()
        {
            icon.sprite = selected;
            CursorSelect.Cursor.Select(CursorType.Destroy);
        }

        private void ExitClearMode()
        {
            Manager.Map.Highlight(_highlighted, Map.HighlightState.Inactive);
            _highlighted = new Cell[1];
            icon.sprite = deselected;
            if (CursorSelect.Cursor.currentCursor == CursorType.Destroy)
                CursorSelect.Cursor.Select(CursorType.Pointer);
        }

        private void ClearBuilding()
        {
            if (_selectedBuilding.indestructible) return;
            //BuildingStats building = _selectedBuilding.GetComponent<BuildingStats>();
            Building building = _selectedBuilding.GetComponent<Building>();
            if (!Manager.Spend(ScaledCost)) return;
            if (building.type == BuildingType.Terrain) ClearCount++;
            building.Clear();
            Manager.Buildings.Remove(building);
        }
    
        private void OnDestroy()
        {
            ClearCount = 0;
        }
    
    }
}
