using System;
using System.Collections.Generic;
using System.Linq;
using Cards;
using DG.Tweening;
using Inputs;
using Managers;
using Quests;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Utilities;
using static Managers.GameManager;

namespace Structures
{
    public class Select : MonoBehaviour
    {
        [Serializable]
        private struct EffectBadge
        {
            public RectTransform transform;
            public CanvasGroup canvasGroup;
            public Image background, icon, chevron;
            public CardBadge badge;
        }
        
        [SerializeField] private int yOffset;
        [SerializeField] private Image buttonImage, maskImage;
        [SerializeField] private TextMeshProUGUI nameText, costText, questTitleText;
        [SerializeField] private Sprite buildingButtonBacking, questButtonBacking;
        [SerializeField] private CanvasGroup buttonCanvasGroup;
        
        [SerializeField] private LayerMask collisionMask;
        
        [SerializeField] [ColorUsage(false, true)] private Color hoverColor;
        [SerializeField] [ColorUsage(false, true)] private Color selectColor;
        
        [SerializeField] private float raycastInterval;

        [SerializeField] private List<EffectBadge> badges;
        [SerializeField] private EffectBadge bonusBadge;
        [SerializeField] private TextMeshProUGUI bonusText;
        [SerializeField] private List<Sprite> chevronSizes;
        [SerializeField] private SerializedDictionary<Stat, Sprite> statIcons;
        
        private OutlineRenderFeature _outline;
        private Canvas _canvas;
        private Camera _cam;
        private Structure _hoveredStructure, _selectedStructure;
        private float _timeSinceRaycast;
        private float _interactTimer;
        private bool _interactStarted;

        public static Action<Structure> OnClear;
        public static Action<Quest> OnQuestSelected;

        public static Select Instance { get; internal set; }

        private Structure HoveredStructure
        {
            set {
                if (_hoveredStructure == value) return;

                // Deselection should only happen when the un-hovered building
                // is not the same as the selected building
                if (_hoveredStructure && !SelectedStructure)
                {
                    _hoveredStructure.Selected = false;
                }
                _hoveredStructure = value;
                if (!_hoveredStructure) return;
                _hoveredStructure.Selected = true;
                SetHighlightColor(hoverColor);
            }
        }
        
        public Structure SelectedStructure
        {
            get => _selectedStructure;
            set
            {
                if (_selectedStructure) _selectedStructure.Selected = false;

                _selectedStructure = value;

                if (!_selectedStructure)
                {
                    DOTween.Kill(buttonCanvasGroup);
                    buttonCanvasGroup.DOFade(0, 0.3f).OnComplete(() => _canvas.enabled = false);
                    HideEffects();
                    return;
                }

                _canvas.enabled = true;
                
                _selectedStructure.Selected = true;
                SetHighlightColor(selectColor);
                
                // Find building position and reposition clearButton to overlay on top of it.  
                RepositionButton();
                
                // Set button image values
                DOTween.Kill(buttonCanvasGroup);
                buttonCanvasGroup.alpha = 0;
                buttonImage.sprite = SelectedStructure.IsQuest ? questButtonBacking : buildingButtonBacking;
                
                switch (SelectedStructure.StructureType)
                {
                    case StructureType.Quest:
                        nameText.text = "";
                        costText.text = "";
                        questTitleText.text = SelectedStructure.Quest ? SelectedStructure.Quest.Title : "No Quests Here";
                        buttonCanvasGroup.DOFade(SelectedStructure.Quest ? 1 : 0.7f, 0.5f);
                        DisableEffects();
                        break;
                    case StructureType.Building:
                        questTitleText.text = "";
                        nameText.text = SelectedStructure.name;
                        if (Tutorial.Tutorial.Active)
                        {
                            costText.text = "Disabled";
                            buttonCanvasGroup.DOFade(0.7f, 0.5f);
                        }
                        else if (SelectedStructure.Blueprint.type == BuildingType.GuildHall)
                        {
                            costText.text = "Destroy?";
                            buttonCanvasGroup.DOFade(1, 0.5f);
                        }
                        else
                        {
                            costText.text = $"Refund: {SelectedStructure.Blueprint.Refund}";
                            buttonCanvasGroup.DOFade(1, 0.5f);
                        }
                        DisplayEffects();
                        break;
                    default: // Terrain and Ruins
                        questTitleText.text = "";
                        nameText.text = SelectedStructure.IsRuin ? "Ruins" : "Forest";
                        int cost = SelectedStructure.ClearCost;
                        float alpha;
                        if (Tutorial.Tutorial.Active)
                        {
                            costText.text = SelectedStructure.IsRuin ? "Clear" : "Disabled";
                            alpha = SelectedStructure.IsRuin ? 1 : 0.7f;
                        }
                        else
                        {
                            costText.text = $"Cost: {SelectedStructure.ClearCost}";
                            alpha = Manager.Stats.Wealth >= cost ? 1 : 0.7f;
                        }
                        buttonCanvasGroup.DOFade(alpha, 0.5f);
                        DisableEffects();
                        break;
                }
            }
        }
        
        private void Deselect()
        {
            if(SelectedStructure) SelectedStructure = null;
        }

        private void Awake()
        {
            Instance = this;   
        }

        private void Start()
        {
            _canvas = GetComponent<Canvas>();
            _cam = Camera.main;

            Cards.Cards.OnCardSelected += _ => Deselect();
            Manager.Inputs.LeftClick.performed += _ => ToggleSelect();
            Manager.Inputs.RightClick.performed += _ => Deselect();
            
            Manager.Inputs.DemolishBuilding.performed += _ => { if(SelectedStructure && !SelectedStructure.IsQuest) Interact(); };
            Manager.Inputs.DemolishBuilding.canceled += _ => 
            {
                _interactTimer = 0;
                maskImage.fillAmount = 0;
            };
            Manager.Inputs.SelectQuest.performed += _ => { if(SelectedStructure && SelectedStructure.IsQuest) Interact(); };
            Manager.Inputs.SelectQuest.canceled += _ => 
            {
                _interactTimer = 0;
                maskImage.fillAmount = 0;
            };

            GameHud.OnTogglePhotoMode += _ => Deselect();
            
            GetComponentInChildren<Button>().onClick.AddListener(Interact);
            State.OnEnterState += (_) =>
            {
                SelectedStructure = null;
                HoveredStructure = null;
            };
            foreach(var rd in Manager.PlatformManager.Gameplay.GetPlatformAssets().RendererData.rendererFeatures)
            {
                if (rd is OutlineRenderFeature)
                {
                    _outline = rd as OutlineRenderFeature;
                    break;
                }
            }
        }

        private void Update()
        {
            if (Globals.RestartingGame) return;
            
            // Don't hover new buildings while a building or card is selected, the camera is moving, or in the UI
            if (SelectedStructure || CameraMovement.IsMoving || IsSelectionDisabled() || Tutorial.Tutorial.DisableSelect)
            {
                HoveredStructure = null;
                if (SelectedStructure && IsInteractable() &&
                    (!SelectedStructure.IsQuest && Manager.Inputs.DemolishBuilding.phase == InputActionPhase.Started ||
                    SelectedStructure.IsQuest && Manager.Inputs.SelectQuest.phase == InputActionPhase.Started))
                {
                    maskImage.fillAmount = Mathf.InverseLerp(0, 0.4f, _interactTimer += Time.deltaTime);
                }
                return;
            }

            // Time-slice the hovered building check
            _timeSinceRaycast += Time.deltaTime;
            if (!(_timeSinceRaycast >= raycastInterval)) return;
            _timeSinceRaycast = 0f;
            
            HoveredStructure = Hovered();
        }

        private void LateUpdate()
        {
            // Keep track of the button position above selected building
            if (!SelectedStructure) return;
            transform.position = _cam.WorldToScreenPoint(SelectedStructure.transform.position) + (Vector3.up * yOffset);
        }

        // Returns the building the cursor is hovering over if exists
        private Structure Hovered()
        {
            return Manager.PlatformManager.Gameplay.GetHoveredStructure(_cam, collisionMask);
        }
        
        private void ToggleSelect()
        {
            // Make sure that we don't bring up the button if we click on a UI element. 
            if (IsSelectionDisabled() || Tutorial.Tutorial.DisableSelect) return;
            
            if (SelectedStructure) // Deselect current building if already selected
            {
                Deselect();
                return;
            }

            SelectedStructure = Hovered();
        }

        private void SetHighlightColor(Color color)
        {
            _outline.settings.color = color;
        }

        private static bool IsSelectionDisabled()
        {
            return !Manager.State.InGame || Manager.Cards.SelectedCard || IsOverUi || Manager.Cards.PlacingBuilding;
        }

        private void DisplayEffects()
        {
            badges.ForEach(badge => badge.canvasGroup.alpha = 0);
            bonusBadge.canvasGroup.alpha = 0;
            bonusBadge.canvasGroup.gameObject.SetActive(SelectedStructure.Bonus.HasValue);
            var effects = SelectedStructure.Stats.OrderByDescending(x => x.Value).ToList();

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
                
                int scaledValue = effects[i].Value * Manager.Stats.StatMultiplier(effects[i].Key);
                badges[i].badge.Description =
                    $"{scaledValue.WithSign()} {effects[i].Key.ToString()}" +
                    " Satisfaction".Conditional((int)effects[i].Key < 5);
            }
            
            if (!SelectedStructure.Bonus.HasValue) return;
            bonusBadge.badge.Description = 
                $"+{Manager.Stats.StatMultiplier(SelectedStructure.Bonus.Value)} {SelectedStructure.Bonus.ToString()}" +
                " Satisfaction".Conditional((int)SelectedStructure.Bonus < 5);
            
            bonusBadge.transform.localPosition = new Vector2(0, -250);
            
            const float delay = 0.5f;
            bonusBadge.canvasGroup.DOFade(1, 1f).SetDelay(delay);
            bonusBadge.transform
                .DOLocalMove(new Vector2(0, -200), 1f)
                .SetDelay(delay)
                .OnStart(() => {
                    bonusBadge.icon.sprite = statIcons[SelectedStructure.Bonus.Value];
                    bonusBadge.background.color = Colors.StatColours[SelectedStructure.Bonus.Value];
                    bonusText.text = SelectedStructure.Blueprint.adjacencyConfig.Description;
                });
        }

        private void DisableEffects()
        {
            badges.ForEach(badge => badge.canvasGroup.gameObject.SetActive(false));
            bonusBadge.canvasGroup.gameObject.SetActive(false);
        }
        
        private void HideEffects()
        {
            for (int i = 0; i < badges.Count; i++)
            {
                badges[i].transform.DOLocalMove(new Vector2(0, -50),0.3f);
                badges[i].canvasGroup.DOFade(0, 0.3f);
            }
            bonusBadge.transform.DOLocalMove(new Vector2(0, -320), 0.3f);
            bonusBadge.canvasGroup.DOFade(0, 0.3f);
        }
        
        private void RepositionButton()
        {
            Vector3 buildingPosition = SelectedStructure.transform.position;
            transform.position = _cam.WorldToScreenPoint(buildingPosition) + (Vector3.up * yOffset);
            _canvas.enabled = true;
        }

        private int SelectedStructureCost()
        {
            var cost = 0;
            switch (SelectedStructure.StructureType)
            {
                case StructureType.Building:
                    cost = -SelectedStructure.Blueprint.Refund;
                    break;
                case StructureType.Terrain:
                case StructureType.Ruins:
                    if (Tutorial.Tutorial.Active) break;
                    cost = SelectedStructure.ClearCost;
                    break;
            }
            return cost;
        }

        private bool IsInteractable()
        {
            if (!SelectedStructure) return false;
            
            if (SelectedStructure.IsQuest) return SelectedStructure.Quest;
            
            // Guard against destroying anything besides ruins in the tutorial
            if (Tutorial.Tutorial.Active && !SelectedStructure.IsRuin) return false;
            return Manager.Stats.Wealth >= SelectedStructureCost();
        }

        private void Interact()
        {
            if (!IsInteractable()) return;
            
            if (SelectedStructure.IsQuest) OnQuestSelected?.Invoke(SelectedStructure.Quest);
            else
            {
                Manager.Stats.Spend(SelectedStructureCost());
                
                Structure structure = SelectedStructure; // Cache because invoke can deselect
                OnClear?.Invoke(SelectedStructure);
                Manager.Structures.Remove(structure);
            }
            Deselect();
            maskImage.fillAmount = 0;
            _interactTimer = 0;
        }
    }
}

