using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Inputs;
using Managers;
using Quests;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;
using Utilities;
using static Managers.GameManager;

namespace Buildings
{
    public class BuildingSelect : MonoBehaviour
    {
        [Serializable]
        private struct EffectBadge
        {
            public RectTransform transform;
            public CanvasGroup canvasGroup;
            public Image background, icon, chevron;
        }
        
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
        [SerializeField] private CanvasGroup buttonCanvasGroup;
        
        [SerializeField] private LayerMask collisionMask;
        
        [SerializeField] [ColorUsage(false, true)] private Color hoverColor;
        [SerializeField] [ColorUsage(false, true)] private Color selectColor;
        
        [SerializeField] private float raycastInterval;

        [SerializeField] private List<EffectBadge> badges;
        [SerializeField] private List<Sprite> chevronSizes;
        [SerializeField] private SerializedDictionary<Stat, Sprite> statIcons;

        private Canvas _canvas;
        private Camera _cam;
        private OutlinePostProcess _outline;
        private Building _hoveredBuilding, _selectedBuilding;
        private SelectionConfig _config;
        private float _timeSinceRaycast;

        public static Action<Building> OnClear;
        public static Action<Quest> OnQuestSelected;
        
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

                if (!_selectedBuilding)
                {
                    DOTween.Kill(buttonCanvasGroup);
                    buttonCanvasGroup.DOFade(0, 0.3f).OnComplete(() => _canvas.enabled = false);
                    HideEffects();
                    return;
                }

                _canvas.enabled = true;
                
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
                DOTween.Kill(buttonCanvasGroup);
                buttonCanvasGroup.alpha = 0;
                buttonCanvasGroup.DOFade(_config.IsRefund || Manager.Stats.Wealth >= _config.Cost ? 1f : 0.7f, 0.5f);
                buttonImage.sprite = _config.IsQuest ? questButtonBacking : buildingButtonBacking;
                
                // Set Cost text values
                if (_config.IsQuest) costText.text = "";
                else if (_config.Title == "Guild Hall") costText.text = "Cost: Everything";
                else costText.text = (_config.IsRefund ? "Refund: " : "Cost: ") + _config.Cost;

                badges.ForEach(badge => badge.canvasGroup.alpha = 0);
                // Fade in building effects
                if (_config.IsRefund) DisplayEffects();
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
            GetComponentInChildren<Button>().onClick.AddListener(SelectBuilding);

            ClickOnButtonDown.OnUIClick += DeselectBuilding;
            State.OnEnterState += () => HoveredBuilding = null;
        }

        private void DeleteBuildingInput(UnityEngine.InputSystem.InputAction.CallbackContext obj)
        {
            if (!Manager.State.InGame) return;
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
                return new SelectionConfig(_selectedBuilding.Quest.Title, SelectionType.Quest);
            if (SelectedBuilding.IsRuin)
                return new SelectionConfig("Ruin", SelectionType.Clear, SelectedBuilding.RuinsClearCost);
            if (SelectedBuilding.IsTerrain)
                return new SelectionConfig("Terrain", SelectionType.Clear, SelectedBuilding.TerrainClearCost);
            
            return new SelectionConfig(SelectedBuilding.name, SelectionType.Refund, SelectedBuilding.Refund); 
        }

        private void DisplayEffects()
        {
            var effects = SelectedBuilding.stats.OrderByDescending(x => x.Value).ToList();

            for (int i = 0; i < badges.Count; i++)
            {
                if (i >= effects.Count)
                {
                    // Hide the badge and chevron if there are no more effects to display
                    badges[i].canvasGroup.gameObject.SetActive(false);
                    continue;
                }
                
                badges[i].canvasGroup.gameObject.SetActive(true);
                badges[i].transform.localPosition = new Vector2(0, -50);
                int i1 = i; // copy to variable so it doesnt change during delay
                StartCoroutine(Algorithms.DelayCall(0.1f * i, () =>
                {
                    badges[i1].transform.DOLocalMove(Vector3.zero, 1f);
                    badges[i1].canvasGroup.DOFade(1, 1f);
                }));

                // Set the chevron values
                badges[i].chevron.color = effects[i].Value > 0 ? Colors.Green : Colors.Red;
                badges[i].chevron.transform.localRotation = 
                    Quaternion.Euler(effects[i].Value > 0 ? new Vector3(0, 0, 180) : Vector3.zero);
                badges[i].chevron.sprite = chevronSizes[Math.Abs(effects[i].Value)-1];
                // Set the badge values
                badges[i].background.color = Colors.StatColours[effects[i].Key];
                badges[i].icon.sprite = statIcons[effects[i].Key];
            }
        }
        
        private void HideEffects()
        {
            for (int i = 0; i < badges.Count; i++)
            {
                badges[i].transform.DOLocalMove(new Vector2(0, -50),0.3f);
                badges[i].canvasGroup.DOFade(0, 0.3f);
            }
        }
        
        private void RepositionButton()
        {
            Vector3 buildingPosition = SelectedBuilding.transform.position;
            transform.position = _cam.WorldToScreenPoint(buildingPosition) + (Vector3.up * yOffset);
            _canvas.enabled = true;
        }

        private void SelectBuilding()
        {
            // Switch on config type
            switch (_config.Type)
            {
                case SelectionType.Quest:
                    OnQuestSelected?.Invoke(SelectedBuilding.Quest);
                    break;
                case SelectionType.Refund:
                case SelectionType.Clear:
                {
                    bool affordable = Manager.Stats.Spend(
                        _config.Cost * (_config.IsRefund ? -1 : 1));
                    
                    if (!SelectedBuilding  || _config == null || 
                        SelectedBuilding.indestructible || !affordable) return;
            
                    if (SelectedBuilding.IsRuin) Building.RuinsClearCount += SelectedBuilding.SectionCount;
                    if (SelectedBuilding.IsTerrain) Building.TerrainClearCount += SelectedBuilding.SectionCount;
            
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

