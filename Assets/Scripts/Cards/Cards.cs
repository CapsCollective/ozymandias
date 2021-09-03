using System;
using System.Collections.Generic;
using System.Linq;
using Inputs;
using Managers;
using Map;
using Structures;
using UnityEngine;
using UnityEngine.EventSystems;
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
        
        #region Blueprint Lists
        [SerializeField] private List<Blueprint> all;
        public List<Blueprint> All => all; // All playable (excludes guild hall)
        
        [SerializeField] private Blueprint guildHall;
        public Blueprint GuildHall => guildHall;
        
        [SerializeField] private List<Card> hand; 
        private List<Blueprint> Deck { get; set; } // Remaining cards in the deck
        private List<Blueprint> Unlocked { get; set; } // Unlocked across all playthroughs
        private List<Blueprint> Playable { get; set; } // Currently playable (both starter and unlocked/ discovered)
        private List<Blueprint> Discoverable { get; set; } // Cards discoverable in ruins
        public bool IsDiscoverableOrPlayable(Blueprint blueprint) => Discoverable.Contains(blueprint) || Playable.Contains(blueprint);
        public bool IsUnlocked(Blueprint blueprint) => Unlocked.Contains(blueprint);
        public Blueprint Find(BuildingType type) => type == BuildingType.GuildHall ? GuildHall : All.Find(blueprint => blueprint.type == type);
        
        #endregion
        
        private void Awake()
        {
            _cam = Camera.main;

            Select.OnClear += structure =>
            {
                // Gets more likely to discover buildings as ruins get cleared until non remain
                if (Discoverable.Count != 0 && structure.IsRuin && Random.Range(0, Manager.Structures.Ruins) <= Discoverable.Count)
                    Unlock(Discoverable.PopRandom(), true);
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
            Click.OnLeftClick += LeftClick;
            Click.OnRightClick += RightClick;
            // Manager.Inputs.IA_SelectCards.performed += SelectCards;
            // Manager.Inputs.IA_UINavigate.performed += Navigate;
            // Manager.Inputs.IA_UICancel.performed += UICancel;
            // Manager.Inputs.IA_DeselectCards.performed += UICancel;
            // Manager.Inputs.IA_RotateBuilding.performed += RotateBuilding;
        }
        
        
        #region Card Select
        private Card _selectedCard;
        public  Card SelectedCard
        {
            get => _selectedCard;
            set
            {
                _selectedCard = value;
                if(_selectedCard) Manager.Map.Flood();
                else Manager.Map.Drain();
                
                Manager.Cursor.Current = _selectedCard ? CursorType.Build : CursorType.Pointer;
                OnCardSelected?.Invoke(_selectedCard);
            }
        }
        
        private void InitCards()
        {
            hand.ForEach(card => card.Blueprint = NewCard());
        }
        
        private void NewCards()
        {
            hand.ForEach(card => card.Replace());
        }

        public Blueprint NewCard()
        {
            // Reshuffle cards back in, leaving out the ones already in the hand 
            if (Deck.Count == 0)
                Deck = Playable.Where(blueprint => !hand.Select(card => card.Blueprint).Contains(blueprint)).ToList();
            return Deck.PopRandom();
        }
        #endregion
        
        #region Building Placement
        [SerializeField] private LayerMask layerMask;
        private Camera _cam;
        private int _prevRotation, _rotation;
        private List<Cell> _selectedCells = new List<Cell>();
        private bool _cellsValid;
        private Cell _hoveredCell; // Used to only trigger calculations on cell changing
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
            if (!Manager.Cards.SelectedCard || EventSystem.current.IsPointerOverGameObject())
            {
                _hoveredCell = null;
                ClearCells(); 
                return;
            }
            
            Cell closest = ClosestCellToCursor;
            if (closest == null || !closest.Active || (_prevRotation == _rotation && _hoveredCell == closest)) return;
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
        
        private void LeftClick()
        {
            if (!SelectedCard || !_cellsValid || EventSystem.current.IsPointerOverGameObject()) return;
            Click.PlacingBuilding = true;
            Blueprint blueprint = SelectedCard.Blueprint;
            SelectedCard.Replace(); // Check again as card can be deselected
            if (!Manager.Structures.AddBuilding(blueprint, _hoveredCell.Id, _rotation)) return;
            SelectedCard = null;
        }

        private void RightClick()
        {
            if (!SelectedCard) return;
            _rotation++;
            _rotation %= 4;
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
    }
}
