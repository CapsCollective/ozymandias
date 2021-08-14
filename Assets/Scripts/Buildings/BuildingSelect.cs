using System;
using System.Linq;
using Inputs;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;
using Utilities;
using static GameState.GameManager;

namespace Buildings
{
    public class BuildingSelect : MonoBehaviour
    {
        public static int TerrainClearCount { get; set; }
        private const int TerrainBaseCost = 5;
        private const float TerrainCostScale = 1.025f;
        public static int RuinsClearCount { get; set; }
        private const int RuinsBaseCost = 20;
        private const float RuinsCostScale = 1.25f;
        
        private const float RefundPercentage = 0.50f;
        
        private enum SelectionType
        {
            Clear,
            Refund,
            Quest
        }
                
        private class SelectionConfig
        {
            public readonly string Title;
            public readonly int Cost;
            public readonly SelectionType Type;
            
            public SelectionConfig(string title, SelectionType type, int cost = 0)
            {
                Title = title;
                Type = type;
                Cost = cost;
            }
            
            public bool IsRefund => Type == SelectionType.Refund;
            
            public bool IsQuest => Type == SelectionType.Quest;
        }
        
        [SerializeField] private int yOffset;
        [SerializeField] private Image buttonImage;
        [SerializeField] private TextMeshProUGUI nameText, costText, questTitleText;
        [SerializeField] private Sprite buildingButtonBacking, questButtonBacking;
        
        [SerializeField] private LayerMask collisionMask;
        
        [SerializeField] [ColorUsage(false, true)] private Color hoverColor;
        [SerializeField] [ColorUsage(false, true)] private Color selectColor;
        
        [SerializeField] private float raycastInterval;

        private Canvas _canvas;
        private Camera _cam;
        private OutlinePostProcess _outline;
        private Building _hoveredBuilding, _selectedBuilding;
        private SelectionConfig _config;
        private float _timeSinceRaycast;

        public static Action<Building> OnClear;
        
        private Building HoveredBuilding
        {
            set {
                // Deselection should only happen when the un-hovered building
                // is not the same as the selected building
                if (_hoveredBuilding && !SelectedBuilding)
                {
                    _hoveredBuilding.Selected = false;
                }
                _hoveredBuilding = value;
                if (!_hoveredBuilding) return;
                _hoveredBuilding.Selected = true;
                SetHighlightColor(hoverColor);
            }
        }
        
        private Building SelectedBuilding
        {
            get => _selectedBuilding;
            set
            {
                if (_selectedBuilding) _selectedBuilding.Selected = false;

                _selectedBuilding = value;
                _canvas.enabled = _selectedBuilding;
                if (!_selectedBuilding) return;
                
                _selectedBuilding.Selected = true;
                SetHighlightColor(selectColor);
                
                // Find building position and reposition clearButton to overlay on top of it.  
                RepositionButton();

                // Get UI config information
                _config = GetButtonConfiguration();
                
                // Set title text values
                nameText.text = _config.IsQuest ? "" : _config.Title;
                questTitleText.text = _config.IsQuest ? _config.Title : "";
                
                // Set button image values
                SetButtonOpacity(_config.IsRefund || Manager.Wealth >= _config.Cost ? 255f : 166f);
                buttonImage.sprite = _config.IsQuest ? questButtonBacking : buildingButtonBacking;
                
                // Set Cost text values
                if (_config.IsQuest) costText.text = "";
                else if (_config.Title == "Guild Hall") costText.text = "Cost: Everything";
                else costText.text = (_config.IsRefund ? "Refund: " : "Cost: ") + _config.Cost;
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
            Manager.Inputs.IA_DeleteBuilding.performed += DeleteBuildingInput;
            Manager.Inputs.IA_DeleteBuilding.started += DeleteBuildingInput;
            Manager.Inputs.IA_DeleteBuilding.canceled += DeleteBuildingInput;

            ClickOnButtonDown.OnUIClick += DeselectBuilding;
            OnEnterMenu += () => HoveredBuilding = null;
        }

        private void DeleteBuildingInput(UnityEngine.InputSystem.InputAction.CallbackContext obj)
        {
            if (Manager.InMenu || Manager.TurnTransitioning) return;
            if(obj.performed) SelectBuilding();
        }

        private void Update()
        {
            // Don't hover new buildings while a building is selected, the camera is moving, or in the UI
            if (SelectedBuilding || CameraMovement.IsMoving || IsSelectionDisabled())
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
            // Keep track of the button position above selected building
            if (!SelectedBuilding) return;
            transform.position = _cam.WorldToScreenPoint(SelectedBuilding.transform.position) + (Vector3.up * yOffset);
        }

        // Returns the building the cursor is hovering over if exists
        private Building SelectHoveredBuilding()
        {
            Ray ray = _cam.ScreenPointToRay(
                new Vector3(Manager.Inputs.MousePosition.x, Manager.Inputs.MousePosition.y, _cam.nearClipPlane));
            Physics.Raycast(ray, out RaycastHit hit, 200f, collisionMask);

            return hit.collider ? hit.collider.GetComponentInParent<Building>() : null;
        }
        
        private void LeftClick()
        {
            if (Click.PlacingBuilding) return;

            // Make sure that we don't bring up the button if we click on a UI element. 
            if (IsSelectionDisabled()) return;
            
            if (SelectedBuilding) // Deselect current building if already selected
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
            return Place.Selected != Place.Deselected || EventSystem.current.IsPointerOverGameObject();
        }
        
        private SelectionConfig GetButtonConfiguration()
        {
            if (SelectedBuilding.IsQuest)
            {
                return new SelectionConfig(_selectedBuilding.Quest.Title, SelectionType.Quest);
            }
            else if (SelectedBuilding.IsRuin)
            {
                var cost = (int) 
                    Enumerable.Range(RuinsClearCount, SelectedBuilding.SectionCount)
                        .Sum(i => Mathf.Pow(RuinsCostScale, i)) * RuinsBaseCost;
                return new SelectionConfig("Ruin", SelectionType.Clear, cost);
            }
            else if (SelectedBuilding.IsTerrain)
            {
                var cost = (int)
                    Enumerable.Range(TerrainClearCount, SelectedBuilding.SectionCount)
                        .Sum(i => Mathf.Pow(TerrainCostScale, i)) * TerrainBaseCost;
                return new SelectionConfig("Terrain", SelectionType.Clear, cost);
            }
            else
            {
                return new SelectionConfig(SelectedBuilding.name, SelectionType.Refund, 
                    Mathf.FloorToInt(SelectedBuilding.ScaledCost * RefundPercentage));
            }
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

        private void RepositionButton()
        {
            Vector3 buildingPosition = SelectedBuilding.transform.position;
            transform.position = _cam.WorldToScreenPoint(buildingPosition) + (Vector3.up * yOffset);
            _canvas.enabled = true;
        }

        public void SelectBuilding()
        {
            // Switch on config type
            switch (_config.Type)
            {
                case SelectionType.Quest:
                    // TODO open quest flyer here
                    break;
                case SelectionType.Refund:
                case SelectionType.Clear:
                {
                    var affordable = Manager.Spend(
                        _config.Cost * (_config.IsRefund ? -1 : 1));
                    
                    if (!SelectedBuilding  || _config == null || 
                        SelectedBuilding.indestructible || !affordable) return;
            
                    if (SelectedBuilding.IsRuin) RuinsClearCount += SelectedBuilding.SectionCount;
                    if (SelectedBuilding.IsTerrain) TerrainClearCount += SelectedBuilding.SectionCount;
            
                    OnClear.Invoke(SelectedBuilding);
                    Manager.Buildings.Remove(SelectedBuilding);
                    DeselectBuilding();
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}

