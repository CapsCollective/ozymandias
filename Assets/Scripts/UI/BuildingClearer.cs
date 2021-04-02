using System;
using System.Linq;
using Controllers;
using UnityEngine;
using Utilities;
using UnityEngine.UI;
using Entities;
using TMPro;
using UnityEngine.EventSystems;
using static Managers.GameManager;

namespace UI
{
    public class BuildingClearer : MonoBehaviour
    { 
        [SerializeField] int pixelsPerUnit;
        
        private Button _clearButton;
        private Cell _selected;
        private int _selectedDestroyCost;
        private Camera _mainCamera;
        private int _numberOfTerrainTilesDeleted;
        private TextMeshProUGUI _nameText, _costText;
        private Vector3 _selectedPosition;

        private const float ScaleSteps = 1.025f;
        // Modifier for building refunds. Currently set to provide players with a 75% refund. 
        private const float BuildingRefundModifier = 0.25f;

        private void Start()
        {
            _clearButton = GetComponentInChildren<Button>();

            TextMeshProUGUI[] textFields = transform.GetChild(0).GetComponentsInChildren<TextMeshProUGUI>();

            _nameText = textFields[0];
            _costText = textFields[1];

            Click.OnLeftClick += LeftClick;
            Click.OnRightClick += RightClick;
    
            CameraMovement.OnCameraMove += Clear;
    
            _mainCamera = Camera.main;
            
            Clear();
        }
    
        private void Update()
        {
            if (_selected == null || !_clearButton.gameObject.activeSelf) return;

            // Lerp the button to stay above the building.
            _clearButton.transform.position = Vector3.Lerp(
                _clearButton.transform.position,
                _mainCamera.WorldToScreenPoint(_selectedPosition) + 
                (Vector3.up * pixelsPerUnit), 
                0.5f);
        }
    
        // Clears all selections
        void Clear()
        {
            _clearButton.gameObject.SetActive(false);
            _selected = null;
            _selectedDestroyCost = 0;
        }

        private void LeftClick()
        {
            // TODO: Find a way to stop the mouse from clicking on grid elements through other UI elements.
            Clear();

            // Make sure that we don't bring up the button if we click on a UI element. 
            if (BuildingPlacement.Selected != -1 || _selected != null || EventSystem.current.IsPointerOverGameObject()) 
                return;
            
            _selected = Manager.Map.GetCellFromMouse();

            if (!_selected.occupant) return;

            // TODO: Clean this up.
            if (_selected.occupant.HasNeverBeenSelected)
            {
                _selected.occupant.HasNeverBeenSelected = false;
                return;
            }

            // Find building position and reposition clearButton to overlay on top of it.  
            Vector3 buildingPosition = _selected.occupant.transform.position;
            _clearButton.transform.position = _mainCamera.WorldToScreenPoint(buildingPosition) + (Vector3.up * pixelsPerUnit);
            _clearButton.gameObject.SetActive(true);

            int destructionCost;
            string selectedName = _selected.occupant.name;

            // If the selected element is terrain, apply the cost increase algorithm to the destruction cost.
            if (_selected.occupant.type == BuildingType.Terrain)
            {
                var range = Enumerable.Range(1, _numberOfTerrainTilesDeleted);
                destructionCost = (int) range.Select(i => 1.0f / Math.Pow(ScaleSteps, i)).Sum();
                selectedName = "Terrain";
            }
            else
            {
                // Set the cost to 25% of the original building cost (player gets 75% back)
                destructionCost = Mathf.FloorToInt(_selected.occupant.baseCost * BuildingRefundModifier);
            }

            // Disable the button if the player can't afford to clear the tile (visually changes opacity)
            _clearButton.interactable = Manager.Wealth >= destructionCost;
            float opacity = Manager.Wealth >= destructionCost ? 255f : 166f;
            
            // Change opacity of the text.
            Color oldColor = _costText.color;
            _costText.color = _nameText.color = new Color(oldColor.r, oldColor.g, oldColor.b, opacity);

            // Update UI
            _costText.text = destructionCost.ToString();
            _nameText.text = selectedName;

            _selectedDestroyCost = destructionCost;
            _selectedPosition = _selected.occupant.transform.position;
        }
    
        public void ClearBuilding()
        {
            Building occupant = _selected.occupant;
            
            if (_selected.occupant.indestructible || !Manager.Spend(_selectedDestroyCost)) return;

            if (_selected.occupant.type == BuildingType.Terrain)
            {
                _numberOfTerrainTilesDeleted += Manager.Map.GetCells(_selected.occupant).Length;
            }

            occupant.Clear();
            Manager.Buildings.Remove(occupant);
            Clear();
        }
    
        void RightClick()
        {
            Clear();
        }
    }
}

