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
        [SerializeField] int yOffset;
        [SerializeField] private Image buttonImage;
        [SerializeField] private LayerMask collisionMask;
        [SerializeField] private PostProcessVolume postProcessVolume;
        [SerializeField] [ColorUsage(false, true)] Color hoverColor;
        [SerializeField] [ColorUsage(false, true)] Color selectColor;

        private Transform _clearButton;
        private Building _selected;
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

        private float _elapsed;
        private Building _hoveringBuilding;
        
        private struct ClearButtonConfig
        {
            public int DestructionCost;
            public string BuildingName;
        }

        private void Start()
        {
            _clearButton = transform.GetChild(0);

            TextMeshProUGUI[] textFields = transform.GetChild(0).GetComponentsInChildren<TextMeshProUGUI>();

            _nameText = textFields[0];
            _costText = textFields[1];

            Click.OnLeftClick += LeftClick;
            Click.OnRightClick += RightClick;
    
            CameraMovement.OnCameraMove += ClearSelection;
    
            _mainCamera = Camera.main;

            if (!postProcessVolume)
            {
                postProcessVolume = _mainCamera.GetComponentInChildren<PostProcessVolume>();
            }

            postProcessVolume.profile.TryGetSettings(out _outline);

            ClearSelection();
        }

        private void Update()
        {
            CheckForBuildingsAfterInterval();
            SetHighlightForSelectedElement();
        }

            private void FixedUpdate()
        {
            if (_selected == null || !_clearButton.gameObject.activeSelf) return;
            
            _clearButton.position = Vector3.SmoothDamp(
                _clearButton.position,
                _mainCamera.WorldToScreenPoint(_selectedPosition) + (Vector3.up * yOffset), 
                ref _velocity, 0.035f);
        }

        private void ClearSelection()
        {
            _clearButton.gameObject.SetActive(false);
            if (_selected) _selected.selected = false;
            _selected = null;
            _selectedDestroyCost = 0;
        }

        private void LeftClick()
        {
            // Clear all prior selections 
            ClearSelection();

            // Make sure that we don't bring up the button if we click on a UI element. 
            if (SelectionIsDisabled()) return;
            
            _selected = GetBuildingOnClick();

            if (!_selected) return;
            
            // Make sure the building has not just been spawned (such as when it's just been built)
            if (_selected.HasNeverBeenSelected)
            {
                _selected.HasNeverBeenSelected = false;
                _selected.InitialiseBuildingSegments();
                ClearSelection();
                return;
            }

            // Find building position and reposition clearButton to overlay on top of it.  
            RepositionClearButton();

            // Get UI config information
            ClearButtonConfig config = GetClearButtonConfiguration();

            // Set button opacity (based on whether the player can afford to destroy a building)
            SetButtonOpacity(Manager.Wealth >= config.DestructionCost ? 255f : 166f);

            // Update UI
            UpdateUI(config.DestructionCost, config.BuildingName);
        }

        private void SetHighlightForSelectedElement()
        {
            if (_selected) SetHighlightColor(selectColor);
        }

        private void CheckForBuildingsAfterInterval()
        {
            _elapsed += Time.deltaTime;

            if (!(_elapsed >= raycastInterval)) return;
            
            Building buildingBeingHoveredOver = GetBuildingOnClick();

            if (IsAbleToHighlight())
            {
                if (buildingBeingHoveredOver)
                {
                    if (!buildingBeingHoveredOver.segmentsLoaded) buildingBeingHoveredOver.InitialiseBuildingSegments();
                    HighlightUnselected(buildingBeingHoveredOver);
                }
                else
                {
                    UnHighlightBuilding();
                }
            }

            _elapsed = 0f;
        }

        private void UnHighlightBuilding()
        {
            if (!_hoveringBuilding) return;
            _hoveringBuilding.selected = false;
            _hoveringBuilding = null;
        }

        private void HighlightUnselected(Building buildingBeingHoveredOver)
        {
            if (_hoveringBuilding && buildingBeingHoveredOver != _hoveringBuilding) _hoveringBuilding.selected = false;
            _hoveringBuilding = buildingBeingHoveredOver;
            SetHighlightColor(hoverColor);
        }

        private void SetHighlightColor(Color color)
        {
            _hoveringBuilding.selected = true;
            _outline.color.value = color;
        }

        private bool IsAbleToHighlight()
        {
            return !_selected && !SelectionIsDisabled();
        }

        private static bool SelectionIsDisabled()
        {
            return BuildingPlacement.Selected != -1 || EventSystem.current.IsPointerOverGameObject();
        }

        private ClearButtonConfig GetClearButtonConfiguration()
        {
            ClearButtonConfig config = new ClearButtonConfig();
            
            config.DestructionCost = CalculateBuildingClearCost();
            config.BuildingName = _selected.name;

            // If the selected element is terrain, apply the cost increase algorithm to the destruction cost.
            if (_selected.type == BuildingType.Terrain)
            {
                config.DestructionCost = CalculateTerrainClearCost();
                config.BuildingName = "Terrain";
            }

            return config;
        }

        private void SetButtonOpacity(float opacity)
        {
            SetButtonTextOpacity(opacity);
            
            SetButtonColorOpacity(opacity);
        }

        private void SetButtonTextOpacity(float opacity)
        {
            Color oldColor = _costText.color;
            _costText.color = _nameText.color = new Color(oldColor.r, oldColor.g, oldColor.b, opacity);
        }

        private void SetButtonColorOpacity(float opacity)
        {
            Color oldButtonColor = buttonImage.color;
            buttonImage.color = new Color(oldButtonColor.r, oldButtonColor.g, oldButtonColor.b, opacity / 255f);
        }

        private void UpdateUI(int destructionCost, string selectedName)
        {
            _costText.text = destructionCost.ToString();
            _nameText.text = selectedName;
            
            _selectedDestroyCost = destructionCost;
            _selectedPosition = _selected.transform.position;
        }

        private int CalculateBuildingClearCost()
        {
            return Mathf.FloorToInt(_selected.baseCost * BuildingRefundModifier);
        }
        
        private int CalculateTerrainClearCost()
        {
            var range = Enumerable.Range(1, _numberOfTerrainTilesDeleted);
            return (int) range.Select(i => 1.0f / Math.Pow(ScaleSteps, i)).Sum();
        }

        private void RepositionClearButton()
        {
            Vector3 buildingPosition = _selected.transform.position;
            _clearButton.position = _mainCamera.WorldToScreenPoint(buildingPosition) + (Vector3.up * yOffset);
            _clearButton.gameObject.SetActive(true);
        }

        private Building GetBuildingOnClick()
        {
            Ray ray = _mainCamera.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y, _mainCamera.nearClipPlane));
            Physics.Raycast(ray, out var hit, 200f, collisionMask);

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
            //Manager.Buildings.Remove(occupant); Being done on the occupant instead
            ClearSelection();
        }

        private void RightClick()
        {
            ClearSelection();
        }
    }
}

