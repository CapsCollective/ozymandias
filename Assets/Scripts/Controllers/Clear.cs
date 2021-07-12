using System;
using System.Linq;
using UnityEngine;
using Utilities;
using UnityEngine.UI;
using Entities;
using Managers;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.PostProcessing;
using static Managers.GameManager;

namespace Controllers
{
    public class Clear : MonoBehaviour
    {
        private const int BaseCost = 5;
        private const float CostScale = 1.025f;
        private const float RefundPercentage = 0.75f;
        
        public static int TerrainClearCount;
        
        private class SelectedBuildingConfig
        {
            public bool IsRefund;
            public int DestructionCost;
            public string BuildingName;
        }
        
        [SerializeField] private int yOffset;
        [SerializeField] private Image buttonImage;
        [SerializeField] private TextMeshProUGUI nameText, costText;
        [SerializeField] private LayerMask collisionMask;
        [SerializeField] [ColorUsage(false, true)] private Color hoverColor;
        [SerializeField] [ColorUsage(false, true)] private Color selectColor;
        [SerializeField] private float raycastInterval;

        private Canvas _canvas;
        private Camera _cam;
        private OutlinePostProcess _outline;
        private Building _hoveredBuilding, _selectedBuilding;
        private SelectedBuildingConfig _config;
        
        private Vector3 _velocity = Vector3.zero;
        private float _timeSinceRaycast;
        
        // TODO extract properties into member methods
        private Building HoveredBuilding
        {
            get => _hoveredBuilding;
            set {
                // Deselection should only happen when the un-hovered building
                // is not the same as the selected building
                if (_hoveredBuilding && !_selectedBuilding)
                {
                    _hoveredBuilding.selected = false;
                }
                _hoveredBuilding = value;
                if (!_hoveredBuilding) return;
                _hoveredBuilding.selected = true;
                SetHighlightColor(hoverColor);
            }
        }
        
        private Building SelectedBuilding
        {
            get => _selectedBuilding;
            set
            {
                if (_selectedBuilding) _selectedBuilding.selected = false;

                _selectedBuilding = value;
                _canvas.enabled = _selectedBuilding;
                if (!_selectedBuilding) return;
                
                _selectedBuilding.selected = true;
                SetHighlightColor(selectColor);
                
                // Find building position and reposition clearButton to overlay on top of it.  
                RepositionClearButton();

                // Get UI config information
                _config = GetClearButtonConfiguration();

                // Set button opacity (based on whether the player can afford to destroy a building) and text
                SetButtonOpacity(_config.IsRefund || Manager.Wealth >= _config.DestructionCost ? 255f : 166f);
            
                nameText.text = _config.BuildingName;
                if (_config.BuildingName == "Guild Hall") costText.text = "Cost: Don't";
                else costText.text = (_config.IsRefund ? "Refund: " : "Cost: ") + _config.DestructionCost;
            }
        }
        
        private void DeselectBuilding()
        {
            SelectedBuilding = null;
        }
        
        private void Start()
        {
            _canvas = GetComponent<Canvas>();
            _cam = Camera.main;
            if (_cam) _cam.GetComponentInChildren<PostProcessVolume>().profile.TryGetSettings(out _outline);
            
            Click.OnLeftClick += LeftClick;
            Click.OnRightClick += DeselectBuilding;
            InputManager.Instance.IA_DeleteBuilding.performed += DeleteBuildingInput;
            InputManager.Instance.IA_DeleteBuilding.started += DeleteBuildingInput;
            InputManager.Instance.IA_DeleteBuilding.canceled += DeleteBuildingInput;

            ClickOnButtonDown.OnUIClick += DeselectBuilding;
            //CameraMovement.OnCameraMove += DeselectBuilding;
            Shade.OnShadeOpened += () => HoveredBuilding = null;
        }

        private void DeleteBuildingInput(UnityEngine.InputSystem.InputAction.CallbackContext obj)
        {
            if(obj.performed)
                ClearBuilding();
        }


        private void Update()
        {
            // Don't hover new buildings while a building is selected, the camera is moving, or in the UI
            if (_selectedBuilding || CameraMovement.IsMoving || IsSelectionDisabled())
            {
                HoveredBuilding = null;
                return;
            }

            // Time-slice the hovered building check
            _timeSinceRaycast += Time.deltaTime;
            if (!(_timeSinceRaycast >= raycastInterval)) return;
            _timeSinceRaycast = 0f;
            
            HoveredBuilding = SelectHoveredBuilding();
        }

        private void LateUpdate()
        {
            // Keep track of clear button position above selected building
            if (!_selectedBuilding) return;

            transform.position = _cam.WorldToScreenPoint(_selectedBuilding.transform.position) + (Vector3.up * yOffset);
        }

        // Returns the building the cursor is hovering over if exists
        private Building SelectHoveredBuilding()
        {
            Ray ray = _cam.ScreenPointToRay(
                new Vector3(InputManager.MousePosition.x, InputManager.MousePosition.y, _cam.nearClipPlane));
            Physics.Raycast(ray, out RaycastHit hit, 200f, collisionMask);

            return hit.collider ? hit.collider.GetComponentInParent<Building>() : null;
        }
        
        private void LeftClick()
        {
            if (Click.PlacingBuilding)
                return;

            // Make sure that we don't bring up the button if we click on a UI element. 
            if (IsSelectionDisabled()) return;
            
            if (_selectedBuilding) // Deselect current building if already selected
            {
                DeselectBuilding();
                return;
            }
            
            SelectedBuilding = SelectHoveredBuilding();
        }

        private void SetHighlightColor(Color color)
        {
            _outline.color.value = color;
        }

        private static bool IsSelectionDisabled()
        {
            return BuildingPlacement.Selected != BuildingPlacement.Deselected || EventSystem.current.IsPointerOverGameObject();
        }
        
        private SelectedBuildingConfig GetClearButtonConfiguration()
        {
            SelectedBuildingConfig config = new SelectedBuildingConfig();

            // If the selected element is terrain, apply the cost increase algorithm to the destruction cost.
            if (_selectedBuilding.type == BuildingType.Terrain)
            {
                config.IsRefund = false;
                config.DestructionCost = CalculateTerrainClearCost(_selectedBuilding.SectionCount);
                config.BuildingName = "Terrain";
            }
            else
            {
                config.IsRefund = true;
                config.DestructionCost = CalculateBuildingClearCost();
                config.BuildingName = _selectedBuilding.name;
            }

            return config;
        }

        private void SetButtonOpacity(float opacity)
        {
            // Set for button text
            Color oldColor = costText.color;
            costText.color = nameText.color = new Color(oldColor.r, oldColor.g, oldColor.b, opacity);

            // Set for button colour
            Color oldButtonColor = buttonImage.color;
            buttonImage.color = new Color(oldButtonColor.r, oldButtonColor.g, oldButtonColor.b, opacity / 255f);
        }

        private int CalculateBuildingClearCost()
        {
            return Mathf.FloorToInt(_selectedBuilding.baseCost * RefundPercentage);
        }
        
        private int CalculateTerrainClearCost(int count)
        {
            return (int) (Enumerable
                .Range(TerrainClearCount, count) // TODO: Replace 4 with tile count
                .Sum(i => Math.Pow(CostScale, i)) * BaseCost);
        }

        private void RepositionClearButton()
        {
            Vector3 buildingPosition = _selectedBuilding.transform.position;
            transform.position = _cam.WorldToScreenPoint(buildingPosition) + (Vector3.up * yOffset);
            _canvas.enabled = true;
        }

        public void ClearBuilding()
        {
            if (
                !_selectedBuilding  ||
                _config == null ||
                _selectedBuilding.indestructible || 
                !Manager.Spend(_config.DestructionCost * (_config.IsRefund ? -1 : 1))
            ) return;
            
            if (_selectedBuilding.type == BuildingType.Terrain)
                TerrainClearCount += _selectedBuilding.SectionCount;

            Manager.Map.ClearBuilding(_selectedBuilding);
            _selectedBuilding.Clear();
            DeselectBuilding();
        }
    }
}

