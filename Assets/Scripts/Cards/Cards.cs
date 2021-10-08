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
using Random = UnityEngine.Random;
using static Managers.GameManager;

namespace Cards
{
    public class Cards : MonoBehaviour
    {
        public static Action<Card> OnCardSelected;
        public static Action<Blueprint> OnUnlock;
        public static Action OnDiscoverRuin;

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
        private ToggleGroup _toggleGroup;
        public  Card SelectedCard
        {
            get => _selectedCard;
            set
            {
                _prevCardIndex = _selectedCardIndex;
                _selectedCardIndex = hand.FindIndex(card => card == value);
                
                _selectedCard = value;
                if(_selectedCard) Manager.Map.Flood();
                else Manager.Map.Drain();
                
                Manager.Cursor.Current = _selectedCard ? CursorType.Build : CursorType.Pointer;
                OnCardSelected?.Invoke(_selectedCard);
            }
        }

        public bool PlacingBuilding { get; set; }
        
        #region Blueprint Lists
        public List<Blueprint> All => all; // All playable (excludes guild hall)
        public Blueprint GuildHall => guildHall;
        private List<Blueprint> Deck { get; set; } // Remaining cards in the deck
        private List<Blueprint> Unlocked { get; set; } // Unlocked across all playthroughs
        private List<Blueprint> Playable { get; set; } // Currently playable (both starter and unlocked/ discovered)
        private List<Blueprint> Discoverable { get; set; } // Cards discoverable in ruins
        public bool IsUnlocked(Blueprint blueprint) => Unlocked.Contains(blueprint);
        public bool IsDiscoverable(Blueprint blueprint) => Discoverable.Contains(blueprint);
        public Blueprint Find(BuildingType type) => type == BuildingType.GuildHall ? GuildHall : All.Find(blueprint => blueprint.type == type);
        #endregion
        
        private void Awake()
        {
            _cam = Camera.main;
            _toggleGroup = GetComponent<ToggleGroup>();

            Select.OnClear += structure =>
            {
                // Gets more likely to discover buildings as ruins get cleared until non remain
                if (Discoverable.Count == 0 || !structure.IsRuin ||
                    Random.Range(0, Manager.Structures.Ruins) > Discoverable.Count) return;
                
                Unlock(Discoverable.PopRandom(), true);
                Notification.OnNotification.Invoke($"Card rediscovered from ruins! ({Discoverable.Count} remaining)", notificationIcon);
            };
            State.OnNewGame += () =>
            {
                Playable = All.Where(b => b.starter).ToList();
                Deck = new List<Blueprint>(Playable); // Start fresh with a new shuffle
                Discoverable = Unlocked.RandomSelection(Mathf.Min(Manager.Upgrades.GetLevel(UpgradeType.Discoveries), Unlocked.Count));
                InitCards();
            };
            State.OnLoadingEnd += InitCards;
            State.OnEnterState += () => { if(Manager.State.NextTurn) NewCards(); };
        }

        private void Start()
        {
            Manager.Inputs.OnLeftClick.performed += PlaceBuilding;
            Manager.Inputs.OnRotateBuilding.performed += RotateBuilding;
            Manager.Inputs.OnSelectCards.performed += SelectCards;
            Manager.Inputs.OnDeselectCards.performed += DeselectCards;
            Manager.Inputs.OnNavigateCards.performed += NavigateCards;
            Manager.Inputs.OnSelectCardIndex.performed += SelectCardIndex;
            State.OnEnterState += () => SelectCard(-1);
        }
        
        #region Card Select
        public void PopCards() => hand.ForEach(card => card.Pop());
        public void DropCards() => hand.ForEach(card => card.Drop());
        private void InitCards() => hand.ForEach(card => card.Blueprint = NewCard());
        public void NewCards() => hand.ForEach(card => card.Replace());

        public Blueprint NewCard()
        {
            // Reshuffle cards back in, leaving out the ones already in the hand 
            if (Deck.Count == 0)
                Deck = Playable.Where(blueprint => !hand.Select(card => card.Blueprint).Contains(blueprint)).ToList();
            return Deck.PopRandom();
        }
        #endregion
        
        #region Input Management
        
        private void PlaceBuilding(InputAction.CallbackContext obj)
        {
            if (!SelectedCard || !_cellsValid || IsOverUi) return;
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
            if (!Manager.Cards.SelectedCard) return;
            _rotation = (_rotation + (int)Mathf.Sign(obj.ReadValue<float>()) + 4) % 4; // + 4 to offset negatives
        }
        
        private void SelectCards(InputAction.CallbackContext obj)
        {
            if (!Manager.State.InGame || _selectedCardIndex != -1) return;
            SelectCard(_prevCardIndex);
        }
        
        private void DeselectCards(InputAction.CallbackContext obj)
        {
            if (!Manager.State.InGame || _selectedCardIndex == -1) return;
            SelectCard(-1);
        }
        
        private void NavigateCards(InputAction.CallbackContext obj)
        {
            if (!Manager.State.InGame || _selectedCardIndex == -1) return;
            SelectCard((_selectedCardIndex + (int)obj.ReadValue<Vector2>().x + hand.Count) % hand.Count);
        }

        private void SelectCardIndex(InputAction.CallbackContext obj)
        {
            if (!Manager.State.InGame) return;

            int index = (int)obj.ReadValue<float>() - 1;
            SelectCard(_selectedCardIndex == index ? -1 : index);
        }
        
        private void SelectCard(int cardIndex)
        {
            if (cardIndex == _selectedCardIndex) return;
            
            if (cardIndex >= 0)
            {
                if (hand[cardIndex].IsReplacing) return;
                int originalIndex = cardIndex;
                do // Attempt to find a valid card
                {
                    if (hand[cardIndex].Toggle.interactable)
                    {
                        hand[cardIndex].Toggle.isOn = true;
                        hand[cardIndex].OnPointerEnter(null);
                        break;
                    }    
                    cardIndex = (cardIndex + 1) % hand.Count;
                } while (originalIndex != cardIndex);
            }
            else _toggleGroup.SetAllTogglesOff();
        }

        #endregion

        #region Building Placement
        private Cell ClosestCellToCursor
        {
            get
            {
                Ray ray = _cam.ScreenPointToRay(new Vector3(Manager.Inputs.MousePosition.x,
                    Manager.Inputs.MousePosition.y,
                    _cam.nearClipPlane));
                Physics.Raycast(ray, out RaycastHit hit, 200f, layerMask);
                return Manager.Map.GetClosestCell(hit.point);
            }
        }

        private void Update()
        {
            if (!Manager.Cards.SelectedCard || IsOverUi)
            {
                _hoveredCell = null;
                ClearCells(); 
                return;
            }
            
            Cell closest = ClosestCellToCursor;
            if (closest == null || !closest.Active || (_prevRotation == _rotation && _hoveredCell == closest && _prevCardIndex == _selectedCardIndex)) return;
            _hoveredCell = closest;
            _prevRotation = _rotation;
            
            // Wipe previously highlighted cells
            ClearCells();
            _selectedCells = Manager.Map.GetCells(Manager.Cards.SelectedCard.Blueprint.sections, closest.Id, _rotation);
            _cellsValid = Cell.IsValid(_selectedCells);
            Manager.Map.Highlight(_selectedCells, _cellsValid ? HighlightState.Valid : HighlightState.Invalid);
        }

        private void ClearCells()
        {
            Manager.Map.Highlight(_selectedCells, HighlightState.Inactive);
            _selectedCells.Clear();
        }
        
        #endregion
        
        public bool Unlock(Blueprint blueprint, bool isRuin = false)
        {
            if (Playable.Contains(blueprint)) return false;
            if (!Unlocked.Contains(blueprint)) Unlocked.Add(blueprint);
            Playable.Add(blueprint);
            Deck.Add(blueprint); // Add to deck so it shows up faster
            
            OnUnlock?.Invoke(blueprint);
            if (isRuin) OnDiscoverRuin?.Invoke();
            return true;
        }

        public void UnlockAll()
        {
            Deck = new List<Blueprint>(All);
            Playable = new List<Blueprint>(All);
            Discoverable = new List<Blueprint>();
        }

        public CardDetails Save()
        {
            return new CardDetails
            {
                deck = Deck.Select(x => x.type).ToList(),
                unlocked = Unlocked.Select(x => x.type).ToList(),
                playable = Playable.Select(x => x.type).ToList(),
                discoverable = Discoverable.Select(x => x.type).ToList()
            };
        }
        
        public void Load(CardDetails cards)
        {
            Deck = cards.deck?.Select(Find).ToList() ?? new List<Blueprint>();
            Unlocked = cards.unlocked?.Select(Find).ToList() ?? new List<Blueprint>();
            Playable = cards.playable?.Select(Find).ToList();
            if(Playable == null || Playable.Count == 0) Playable = All.Where(b => b.starter).ToList(); // Set for new game
            Discoverable = cards.discoverable?.Select(Find).ToList() ?? new List<Blueprint>();
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
