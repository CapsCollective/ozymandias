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
        [SerializeField] int yOffset;
        [SerializeField] private Image buttonImage;
        [SerializeField] private LayerMask _collisionMask;
        
        private Transform _clearButton;
        private Building _selected;
        private int _selectedDestroyCost;
        private Camera _mainCamera;
        private int _numberOfTerrainTilesDeleted;
        private TextMeshProUGUI _nameText, _costText;
        private Vector3 _selectedPosition;
        private Vector3 velocity = Vector3.zero;

        private const float ScaleSteps = 1.025f;
        // Modifier for building refunds. Currently set to provide players with a 75% refund. 
        private const float BuildingRefundModifier = 0.25f;

        // Struct for storing button configurations
        private struct ClearButtonConfig
        {
            public int destructionCost;
            public string buildingName;
        }

        private void Start()
        {
            _clearButton = transform.GetChild(0);

            TextMeshProUGUI[] textFields = transform.GetChild(0).GetComponentsInChildren<TextMeshProUGUI>();

            _nameText = textFields[0];
            _costText = textFields[1];

            Click.OnLeftClick += LeftClick;
            Click.OnRightClick += RightClick;
    
            CameraMovement.OnCameraMove += Clear;
    
            _mainCamera = Camera.main;
            
            Clear();
        }
        
        private void FixedUpdate()
        {
            if (_selected == null || !_clearButton.gameObject.activeSelf) return;
            
            // Smooth Damp the button to stay above the building.
            _clearButton.position = Vector3.SmoothDamp(
                _clearButton.position,
                _mainCamera.WorldToScreenPoint(_selectedPosition) + (Vector3.up * yOffset), 
                ref velocity, 0.035f);
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
            // Clear all prior selections 
            Clear();

            // Make sure that we don't bring up the button if we click on a UI element. 
            if (selectionIsDisabled()) return;

            _selected = getBuildingOnClick();

            if (!_selected) return;
            
            // Make sure the building has not just been spawned (such as when it's just been built)
            if (_selected.HasNeverBeenSelected)
            {
                _selected.HasNeverBeenSelected = false;
                return;
            }

            // Find building position and reposition clearButton to overlay on top of it.  
            repositionClearButton();

            // Get UI config information
            ClearButtonConfig config = getClearButtonConfiguration();

            // Set button opacity (based on whether the player can afford to destroy a building)
            setButtonOpacity(Manager.Wealth >= config.destructionCost ? 255f : 166f);

            // Update UI
            updateUI(config.destructionCost, config.buildingName);
        }

        private bool selectionIsDisabled()
        {
            return BuildingPlacement.Selected != -1 || _selected != null ||
                   EventSystem.current.IsPointerOverGameObject();
        }

        private ClearButtonConfig getClearButtonConfiguration()
        {
            ClearButtonConfig config = new ClearButtonConfig();
            
            config.destructionCost = calculateBuildingClearCost();
            config.buildingName = _selected.name;

            // If the selected element is terrain, apply the cost increase algorithm to the destruction cost.
            if (_selected.type == BuildingType.Terrain)
            {
                config.destructionCost = calculateTerrainClearCost();
                config.buildingName = "Terrain";
            }

            return config;
        }

        private void setButtonOpacity(float opacity)
        {
            // Change opacity of the text.
            setButtonTextOpacity(opacity);

            // Change opacity of button
            setButtonColorOpacity(opacity);
        }

        private void setButtonTextOpacity(float opacity)
        {
            Color oldColor = _costText.color;
            _costText.color = _nameText.color = new Color(oldColor.r, oldColor.g, oldColor.b, opacity);
        }

        private void setButtonColorOpacity(float opacity)
        {
            Color oldButtonColor = buttonImage.color;
            buttonImage.color = new Color(oldButtonColor.r, oldButtonColor.g, oldButtonColor.b, opacity / 255f);
        }

        private void updateUI(int destructionCost, string selectedName)
        {
            _costText.text = destructionCost.ToString();
            _nameText.text = selectedName;
            
            _selectedDestroyCost = destructionCost;
            _selectedPosition = _selected.transform.position;
        }

        private int calculateBuildingClearCost()
        {
            return Mathf.FloorToInt(_selected.baseCost * BuildingRefundModifier);
        }
        
        private int calculateTerrainClearCost()
        {
            var range = Enumerable.Range(1, _numberOfTerrainTilesDeleted);
            return (int) range.Select(i => 1.0f / Math.Pow(ScaleSteps, i)).Sum();
        }

        private void repositionClearButton()
        {
            Vector3 buildingPosition = _selected.transform.position;
            _clearButton.position = _mainCamera.WorldToScreenPoint(buildingPosition) + (Vector3.up * yOffset);
            _clearButton.gameObject.SetActive(true);
        }

        private Building getBuildingOnClick()
        {
            Ray ray = _mainCamera.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y, _mainCamera.nearClipPlane));
            RaycastHit hit;
            Physics.Raycast(ray, out hit, 200f, _collisionMask);

            if (hit.collider == null) return null;

            return hit.collider.GetComponentInParent<Building>();
        }

        public void ClearBuilding()
        {
            Building occupant = _selected;
            
            if (_selected.indestructible || !Manager.Spend(_selectedDestroyCost)) return;

            if (_selected.type == BuildingType.Terrain)
            {
                _numberOfTerrainTilesDeleted += Manager.Map.GetCells(_selected).Length;
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

