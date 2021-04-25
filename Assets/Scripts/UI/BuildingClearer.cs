using System;
using System.Linq;
using Controllers;
using UnityEngine;
using Utilities;
using UnityEngine.UI;
using Entities;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.PostProcessing;
using static Managers.GameManager;

namespace UI
{
    public class BuildingClearer : MonoBehaviour
    { 
        [SerializeField] int yOffset;
        [SerializeField] private Image buttonImage;
        [SerializeField] private LayerMask collisionMask;
        [SerializeField] private PostProcessVolume v;

        private Transform _clearButton;
        private Building _selected;
        private int _selectedDestroyCost;
        private Camera _mainCamera;
        private int _numberOfTerrainTilesDeleted;
        private TextMeshProUGUI _nameText, _costText;
        private Vector3 _selectedPosition;
        private Vector3 _velocity = Vector3.zero;

        private OutlinePostProcess outline;

        private const float ScaleSteps = 1.025f;
        // Modifier for building refunds. Currently set to provide players with a 75% refund. 
        private const float BuildingRefundModifier = 0.25f;

        [Header("Hover Options")] 
        [SerializeField] private float raycastInterval;

        private float _elapsed = 0.0f;
        private Building _hoveringBuilding = null;

        // Struct for storing button configurations
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
    
            CameraMovement.OnCameraMove += Clear;
    
            _mainCamera = Camera.main;

            v.profile.TryGetSettings(out outline);

            Clear();
        }

        private void Update()
        {
            _elapsed += Time.deltaTime;

            if (_elapsed >= raycastInterval)
            {
                Building b = getBuildingOnClick();

                if (!_selected && !SelectionIsDisabled())
                {
                    if (b)
                    {
                        if (_hoveringBuilding && b != _hoveringBuilding) _hoveringBuilding.selected = false;
                        _hoveringBuilding = b;
                        _hoveringBuilding.selected = true;
                        outline.color.value = Color.white;
                    }
                    else
                    {
                        if (_hoveringBuilding)
                        {
                            _hoveringBuilding.selected = false;
                            _hoveringBuilding = null;
                            outline.color.value = Color.white;
                        }
                    }
                }

                _elapsed = 0f;
            }

            if (_selected)
            {
                _selected.selected = true;
                outline.color.value = Color.red;
            }
        }

            private void FixedUpdate()
        {
            if (_selected == null || !_clearButton.gameObject.activeSelf) return;
            
            // Smooth Damp the button to stay above the building.
            _clearButton.position = Vector3.SmoothDamp(
                _clearButton.position,
                _mainCamera.WorldToScreenPoint(_selectedPosition) + (Vector3.up * yOffset), 
                ref _velocity, 0.035f);
        }
    
        // Clears all selections
        void Clear()
        {
            _clearButton.gameObject.SetActive(false);
            if (_selected) _selected.selected = false;
            _selected = null;
            _selectedDestroyCost = 0;
        }

        private void LeftClick()
        {
            // Clear all prior selections 
            Clear();

            // Make sure that we don't bring up the button if we click on a UI element. 
            if (SelectionIsDisabled()) return;

            //Debug.Log("selected");
            _selected = getBuildingOnClick();

            if (!_selected) return;
            
            // Make sure the building has not just been spawned (such as when it's just been built)
            if (_selected.HasNeverBeenSelected)
            {
                _selected.HasNeverBeenSelected = false;
                Clear();
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

        private bool SelectionIsDisabled()
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
            // Change opacity of the text.
            SetButtonTextOpacity(opacity);

            // Change opacity of button
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

        private Building getBuildingOnClick()
        {
            Ray ray = _mainCamera.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y, _mainCamera.nearClipPlane));
            RaycastHit hit;
            Physics.Raycast(ray, out hit, 200f, collisionMask);

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

