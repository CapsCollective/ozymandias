using System;
using System.Collections.Generic;
using System.Linq;
using Managers;
using Map;
using Structures;
using UI;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Utilities;
using static Managers.GameManager;
using Random = UnityEngine.Random;

namespace Cards
{
    public class Cards : MonoBehaviour
    {
        public static Action<Card> OnCardSelected;
        public static Action<Blueprint, bool> OnUnlock;
        public static Action OnBuildingRotate;

        [SerializeField] private List<Blueprint> all;
        [SerializeField] private Blueprint guildHall;
        [SerializeField] private LayerMask layerMask;
        [SerializeField] private List<Card> hand;
        [SerializeField] private Sprite notificationIcon;

        private Camera _cam;
        private int _prevRotation, _rotation;
        private List<Cell> _selectedCells = new List<Cell>();
        private bool _cellsValid;
        private Cell _hoveredCell; // Used to only trigger calculations on cell changing
        private int _prevCardIndex = 1, _selectedCardIndex = -1;
        private Card _selectedCard;
        private float _currentBadge = -1;
        private int _lastBadge = -1;

        private ToggleGroup _toggleGroup;
        public Card SelectedCard
        {
            get => _selectedCard;
            set
            {
                if(_selectedCard != null && _lastBadge >= 0)
                    _selectedCard.cardDisplay.Badges[_lastBadge].badge.OnPointerExit(null);

                if (_selectedCardIndex >= 0) _prevCardIndex = _selectedCardIndex;
                _selectedCardIndex = hand.FindIndex(card => card == value);

                _selectedCard = value;
                if (_selectedCard && _selectedCard.Interactable)
                {
                    Manager.Map.Flood();
                    Manager.Map.SetHighlightedPlacements(_selectedCard.Blueprint);
                }
                else
                {
                    Manager.Map.Drain();
                }
                
                Manager.Cursor.Current = _selectedCard && _selectedCard.Interactable ? CursorType.Build : CursorType.Pointer;
                OnCardSelected?.Invoke(_selectedCard);
                
                _lastBadge = -1;
                _currentBadge = -1;
            }
        }

        public bool PlacingBuilding { get; set; }
        
        #region Blueprint Lists
        public List<Blueprint> All => all; // All playable (excludes guild hall)
        public Blueprint GuildHall => guildHall;
        private List<Blueprint> Deck { get; set; } // Remaining cards in the deck
        private List<Blueprint> Unlocked { get; set; } // Unlocked across all playthroughs (not including starters)
        private List<Blueprint> Playable { get; set; } // Currently playable (both starter and unlocked/ discovered)
        private List<Blueprint> Starters => All.Where(b => b.starter).ToList();
        private List<Blueprint> Discoverable => Unlocked.Where(c => !Playable.Contains(c)).ToList();
        public bool IsPlayable(Blueprint blueprint) => Playable.Contains(blueprint);
        public bool IsUnlocked(Blueprint blueprint) => Unlocked.Contains(blueprint);
        public Blueprint Find(BuildingType type) => type == BuildingType.GuildHall ? GuildHall : All.Find(blueprint => blueprint.type == type);
        public int UnlockedCards => Unlocked.Count;
        public int DiscoveriesRemaining { get; set; } 
        
        #endregion
        
        private void Awake()
        {
            _cam = Camera.main;
            _toggleGroup = GetComponent<ToggleGroup>();

            Select.OnClear += structure =>
            {
                // Gets more likely to discover buildings as ruins get cleared until non remain
                if (!structure.IsRuin) return;
                if (Discoverable.Count <= 0)
                {
                    if (!Tutorial.Tutorial.Active && Random.Range(0,4) == 0) Manager.Notifications.Display(
                        "No unlocked cards to discover in ruins",
                        notificationIcon, 3,
                        () => Manager.Book.Open(Book.BookPage.Reports)
                    );
                    return;
                }
                if (DiscoveriesRemaining <= 0)
                {
                    if (Random.Range(0,4) == 0) Manager.Notifications.Display(
                        "No card discoveries remaining, upgrade to find more",
                        notificationIcon, 3,
                        () => Manager.Book.Open(Book.BookPage.Upgrades)
                    );
                    return;
                }
                Unlock(Discoverable.SelectRandom(), true);
                DiscoveriesRemaining--;
                Manager.Notifications.Display($"Card rediscovered from ruins! ({DiscoveriesRemaining} remaining)", notificationIcon, 3);
            };
            State.OnNewGame += () =>
            {
                Playable = Starters;
                Deck = new List<Blueprint>(Playable); // Start fresh with a new shuffle
                DiscoveriesRemaining = Manager.Upgrades.GetLevel(UpgradeType.Discoveries);
                InitCards();
            };
            State.OnLoadingEnd += InitCards;
            State.OnEnterState += (state) => { if(Manager.State.NextTurn) NewCards(); };
        }

        private void Start()
        {
            Manager.Inputs.LeftClick.performed += PlaceBuilding;
            Manager.Inputs.RotateBuilding.performed += RotateBuilding;
            Manager.Inputs.SelectCards.performed += SelectCards;
            Manager.Inputs.DeselectCards.performed += _ => DeselectCards();
            Manager.Inputs.NavigateCards.performed += NavigateCards;
            Manager.Inputs.SelectCardIndex.performed += SelectCardIndex;
            State.OnEnterState += _ =>
            {
                _toggleGroup.SetAllTogglesOff();
                if (Manager.State.InGame) PopCards();
                else DropCards();
            };
        }
        
        #region Card Select
        public void PopCards() => hand.ForEach(card => card.Pop());
        public void DropCards() => hand.ForEach(card => card.Drop());
        private void InitCards() => hand.ForEach(card => card.Blueprint = NewCard());
        public void NewCards() => hand.ForEach(card => card.Replace());

        public Blueprint NewCard()
        {
            // Reshuffle cards back in, leaving out the ones already in the hand
            while (true)
            {
                if (Deck.Count == 0) Deck = new List<Blueprint>(Playable);
                Blueprint card = Deck.PopRandom();
                // 1% for a free card per upgrade level
                card.Free = Random.Range(0, 100) < Manager.Upgrades.GetLevel(UpgradeType.FreeCard);
                if (!hand.Select(c => c.Blueprint).Contains(card)) return card;
            }
        }

        public void SetTutorialCards()
        {
            hand[0].Blueprint = Find(BuildingType.Herbalist);
            hand[0].Blueprint.Free = true;
            hand[1].Blueprint = Find(BuildingType.Tavern);
            hand[1].Blueprint.Free = true;
            hand[2].Blueprint = Find(BuildingType.Inn);
            hand[2].Blueprint.Free = true;
            UpdateUi();
        }
        
        #endregion
        
        #region Input Management
        
        private void PlaceBuilding(InputAction.CallbackContext obj)
        {
            if (!SelectedCard || !_cellsValid || IsOverUi || !SelectedCard.Interactable) return;
            PlacingBuilding = true;
            Blueprint blueprint = SelectedCard.Blueprint;
            SelectedCard.Replace();
            if (!Manager.Structures.AddBuilding(blueprint, _hoveredCell.Id, _rotation)) return;
            SelectedCard = null;
        }

        private void LateUpdate()
        {
            // Always set to false at end of frame to prevent select from immediately selecting placed buildings
            PlacingBuilding = false;
        }

        private void RotateBuilding(InputAction.CallbackContext obj)
        {
            if (!SelectedCard) return;
            OnBuildingRotate?.Invoke();
            _rotation = (_rotation + (int)Mathf.Sign(obj.ReadValue<float>()) + 4) % 4; // + 4 to offset negatives
        }
        
        private void SelectCards(InputAction.CallbackContext obj)
        {
            if (_selectedCardIndex != -1) return;
            SelectCard(_prevCardIndex);
        }
        
        public void DeselectCards()
        {
            _toggleGroup.SetAllTogglesOff();
            if (_selectedCardIndex == -1) return;
            SelectCard(-1);
        }
        
        private void NavigateCards(InputAction.CallbackContext obj)
        {
            if (_selectedCardIndex == -1) SelectCard(_prevCardIndex);
            else SelectCard((_selectedCardIndex + (int)obj.ReadValue<float>() + hand.Count) % hand.Count);
        }

        private void SelectCardIndex(InputAction.CallbackContext obj)
        {
            int index = (int)obj.ReadValue<float>() - 1;
            SelectCard(_selectedCardIndex == index ? -1 : index);
        }
        
        public void SelectCard(int cardIndex)
        {
            if (Manager.GameHud.PhotoModeEnabled || cardIndex == _selectedCardIndex || Tutorial.Tutorial.DisableSelect || !Manager.State.InGame || Manager.Tooltip.NavigationActive) return;

            if (cardIndex >= 0)
            {
                if (hand[cardIndex].IsReplacing) return;
                hand[cardIndex].AnimateSelected();
                hand[cardIndex].Toggle.isOn = true;
            }
            else
            {
                _toggleGroup.SetAllTogglesOff();
                ClearCells();
            }
        }

        #endregion

        #region Building Placement
        private Cell ClosestCellToCursor
        {
            get
            {
                var hit = Manager.Inputs.GetRaycast(_cam, 200f, layerMask);
                return Manager.Map.GetClosestCell(hit.point);
            }
        }

        private void Update()
        {
            if (Globals.RestartingGame) return;
            
            if (!SelectedCard)
            {
                ClearCells();
                _hoveredCell = null;
                return;
            }

            Cell closest = ClosestCellToCursor;
            // Return if target cell, rotation, or card selected hasn't changed
            if (closest == null || !closest.Active || (_prevRotation == _rotation && _hoveredCell == closest && _prevCardIndex == _selectedCardIndex)) return;
            
            // Wipe previously highlighted cells
            ClearCells();

            Manager.Map.HighlightPlacement();
            
            if (IsOverUi) return;
            
            _hoveredCell = closest;
            _prevRotation = _rotation;
            
            _selectedCells = Manager.Map.GetCells(SelectedCard.Blueprint.sections, closest.Id, _rotation);
            _cellsValid = Cell.IsValid(_selectedCells);
            Manager.Map.Highlight(_selectedCells, _cellsValid ? HighlightState.Valid : HighlightState.Invalid);

            // Badge Showing
            if (!Manager.Inputs.UsingController) return;
            _currentBadge += Time.deltaTime * 0.5f;
            if (!SelectedCard.cardDisplay.Badges[(int)_currentBadge].badge.gameObject.activeSelf) _currentBadge = 0;
            if (_currentBadge >= 0 && !SelectedCard.cardDisplay.Badges[(int)_currentBadge].badge.IsShowing)
            {
                if(_lastBadge >=0)
                    SelectedCard.cardDisplay.Badges[_lastBadge].badge.OnPointerExit(null);
                SelectedCard.cardDisplay.Badges[(int)_currentBadge].badge.OnPointerEnter(null);
                _lastBadge = (int)_currentBadge;
            }
        }

        private void ClearCells()
        {
            Manager.Map.ClearHighlight();
            _selectedCells.Clear();
        }
        
        #endregion
        
        public void Unlock(Blueprint blueprint, bool fromRuin = false)
        {
            if (Playable.Contains(blueprint)) return;
            if (!Unlocked.Contains(blueprint)) Unlocked.Add(blueprint);
            Playable.Add(blueprint);
            Deck.Add(blueprint); // Add to deck so it shows up faster
            
            OnUnlock?.Invoke(blueprint, fromRuin);
        }

        public void UnlockAll()
        {
            Deck = new List<Blueprint>(All);
            Playable = new List<Blueprint>(All);
        }

        public CardDetails Save()
        {
            return new CardDetails
            {
                deck = Deck.Select(x => x.type).ToList(),
                unlocked = Unlocked.Select(x => x.type).ToList(),
                playable = Playable.Select(x => x.type).ToList(),
                discoveriesRemaining = DiscoveriesRemaining
            };
        }
        
        public void Load(CardDetails cards)
        {
            Deck = cards.deck?.Select(Find).ToList() ?? new List<Blueprint>();
            Unlocked = cards.unlocked?.Select(Find).ToList() ?? new List<Blueprint>();
            Playable = cards.playable?.Select(Find).ToList() ?? new List<Blueprint>();
            DiscoveriesRemaining = cards.discoveriesRemaining;
            if (Playable == null || Playable.Count == 0) Playable = Starters; // Set for new game
        }

        #if UNITY_EDITOR
        public void DebugSetCard(Blueprint blueprint)
        {
            hand[0].Blueprint = blueprint;
            UpdateUi();
        }
        #endif
    }
}
