using System;
using System.Linq;
using Controllers;
using UnityEngine;
using Utilities;
using UnityEngine.UI;
using Entities;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.PostProcessing;
using static Managers.GameManager;

namespace UI
{
    public class BuildingClearer : MonoBehaviour
    { 
        [Header("Button Config")]
        [SerializeField] private int yOffset;
        [SerializeField] private Image buttonImage;
        [SerializeField] private LayerMask collisionMask;
        [SerializeField] private PostProcessVolume postProcessVolume;
        [SerializeField] [ColorUsage(false, true)] private Color hoverColor;
        [SerializeField] [ColorUsage(false, true)] private Color selectColor;

        private bool _highlightActive = true;
        private Transform _clearButton;
        private Building _selectedBuilding;
        private int _selectedDestroyCost;
        private Camera _mainCamera;
        private int _numberOfTerrainTilesDeleted;
        private TextMeshProUGUI _nameText, _costText;
        private Vector3 _selectedPosition;
        private Vector3 _velocity = Vector3.zero;

        private OutlinePostProcess _outline;

        private const float ScaleSteps = 1.025f;
        
        private const float BuildingRefundModifier = 0.25f;

        [Header("Hover Options")] 
        [SerializeField] private float raycastInterval;
        
        private float _timeSinceRaycast;
        private Building _hoveredBuilding;
        
        private struct ClearButtonConfig
        {
            public int DestructionCost;
            public string BuildingName;
        }

        private void Start()
        {
            _clearButton = transform.GetChild(0);

            var textFields = transform.GetChild(0).GetComponentsInChildren<TextMeshProUGUI>();

            _nameText = textFields[0];
            _costText = textFields[1];

            Click.OnLeftClick += LeftClick;
            Click.OnRightClick += RightClick;
            
            ClickOnButtonDown.OnUIClick += DeselectBuilding;
            CameraMovement.OnCameraMove += DeselectBuilding;
    
            _mainCamera = Camera.main;

            if (!postProcessVolume && _mainCamera)
            {
                postProcessVolume = _mainCamera.GetComponentInChildren<PostProcessVolume>();
            }

            postProcessVolume.profile.TryGetSettings(out _outline);

            DeselectBuilding();
        }

        private void Update()
        {
            // Time-slice the hovered building check
            _timeSinceRaycast += Time.deltaTime;
            if (!(_timeSinceRaycast >= raycastInterval)) return;
            _timeSinceRaycast = 0f;
            
            CheckBuildingsHovered();
        }

        private void FixedUpdate()
        {
            if (!_clearButton.gameObject.activeSelf) return;
            
            _clearButton.position = Vector3.SmoothDamp(
                _clearButton.position,
                _mainCamera.WorldToScreenPoint(_selectedPosition) + (Vector3.up * yOffset), 
                ref _velocity, 0.035f);
        }

        private void CheckBuildingsHovered()
        {
            if (!_highlightActive || _selectedBuilding || SelectionIsDisabled()) return;
            
            var hoveredBuilding = GetBuildingOnClick();

            TryInitialiseBuilding(hoveredBuilding);

            SetHoveredBuilding(hoveredBuilding);
        }

        private void LeftClick()
        { 
            // Make sure that we don't bring up the button if we click on a UI element. 
            if (SelectionIsDisabled()) return;
            
            var selectedBuilding = GetBuildingOnClick();

            // Make sure the building has not just been spawned (such as when it's just been built)
            if (selectedBuilding && selectedBuilding.HasNeverBeenSelected) return;

            TryInitialiseBuilding(selectedBuilding);

            // Assign only if it is not the currently selected building
            if (selectedBuilding == _selectedBuilding) return;
            
            // Select the building
            SetSelectedBuilding(selectedBuilding);
            if (selectedBuilding) SetHighlightColor(selectColor);
            UpdateClearButton(selectedBuilding);
        }

        private void RightClick()
        {
            DeselectBuilding();
        }
        
        private void SetHoveredBuilding(Building building)
        {
            // Deselect the previously selected building
            if (_hoveredBuilding) _hoveredBuilding.selected = false;

            // Select the new building
            _hoveredBuilding = building;
            if (!_hoveredBuilding) return;
            _hoveredBuilding.selected = building;
            SetHighlightColor(hoverColor);
        }
        
        private void SetSelectedBuilding(Building building)
        {
            // Deselect the previously selected building
            if (_selectedBuilding) _selectedBuilding.selected = false;

            // Select the new building
            _selectedBuilding = building;
            
            if (!building) return;
            building.selected = building;
            building.HasNeverBeenSelected = false;
        }

        private void DeselectBuilding()
        {
            SetSelectedBuilding(null);
            UpdateClearButton(null);
        }

        private static void TryInitialiseBuilding(Building building)
        {
            // Initialise building segments if not done so already 
            if (!building || building.segmentsLoaded) return;
            building.InitialiseBuildingSegments();
        }

        private void SetHighlightColor(Color color)
        {
            _outline.color.value = color;
        }

        private static bool SelectionIsDisabled()
        {
            return BuildingPlacement.Selected != -1 || EventSystem.current.IsPointerOverGameObject();
        }
        
        public void SetHighlightActive(bool active)
        {
            _highlightActive = active;
            if (!active) SetHoveredBuilding(null);
        }

        private ClearButtonConfig GetClearButtonConfiguration()
        {
            var config = new ClearButtonConfig();
            
            config.DestructionCost = CalculateBuildingClearCost();
            config.BuildingName = _selectedBuilding.name;

            // If the selected element is terrain, apply the cost increase algorithm to the destruction cost.
            if (_selectedBuilding.type == BuildingType.Terrain)
            {
                config.DestructionCost = CalculateTerrainClearCost();
                config.BuildingName = "Terrain";
            }

            return config;
        }

        private void UpdateClearButton(Building building)
        {
            if (building)
            {
                // Find building position and reposition clearButton to overlay on top of it.  
                RepositionClearButton();

                // Get UI config information
                var config = GetClearButtonConfiguration();

                // Set button opacity (based on whether the player can afford to destroy a building) and text
                SetButtonOpacity(Manager.Wealth >= config.DestructionCost ? 255f : 166f);
                SetButtonText(config.DestructionCost, config.BuildingName);
            
                // Store selected button position
                _selectedDestroyCost = config.DestructionCost;
                _selectedPosition = building.transform.position;   
            }
            else
            {
                _clearButton.gameObject.SetActive(false);
                _selectedDestroyCost = 0;   
            }
        }

        private void SetButtonOpacity(float opacity)
        {
            // Set for button text
            var oldColor = _costText.color;
            _costText.color = _nameText.color = new Color(oldColor.r, oldColor.g, oldColor.b, opacity);

            // Set for button colour
            var oldButtonColor = buttonImage.color;
            buttonImage.color = new Color(oldButtonColor.r, oldButtonColor.g, oldButtonColor.b, opacity / 255f);
        }

        private void SetButtonText(int destructionCost, string selectedName)
        {
            var costText = destructionCost > 0 ? "Cost: " : "Refund: ";
            _costText.text = costText + destructionCost;
            _nameText.text = selectedName;
        }

        private int CalculateBuildingClearCost()
        {
            return Mathf.FloorToInt(_selectedBuilding.baseCost * BuildingRefundModifier);
        }
        
        private int CalculateTerrainClearCost()
        {
            var range = Enumerable.Range(1, _numberOfTerrainTilesDeleted);
            return (int) range.Select(i => 1.0f / Math.Pow(ScaleSteps, i)).Sum();
        }

        private void RepositionClearButton()
        {
            var buildingPosition = _selectedBuilding.transform.position;
            _clearButton.position = _mainCamera.WorldToScreenPoint(buildingPosition) + (Vector3.up * yOffset);
            _clearButton.gameObject.SetActive(true);
        }

        private Building GetBuildingOnClick()
        {
            var ray = _mainCamera.ScreenPointToRay(
                new Vector3(Input.mousePosition.x, Input.mousePosition.y, _mainCamera.nearClipPlane));
            Physics.Raycast(ray, out var hit, 200f, collisionMask);

            if (hit.collider == null) return null;

            return hit.collider.GetComponentInParent<Building>();
        }

        public void ClearBuilding()
        {
            var occupant = _selectedBuilding;
            
            if (_selectedBuilding.indestructible || !Manager.Spend(_selectedDestroyCost)) return;

            if (_selectedBuilding.type == BuildingType.Terrain)
            {
                _numberOfTerrainTilesDeleted += Manager.Map.GetCells(_selectedBuilding).Length;
            }

            occupant.Clear();
            //Manager.Buildings.Remove(occupant); Being done on the occupant instead
            DeselectBuilding();
        }
    }
}

