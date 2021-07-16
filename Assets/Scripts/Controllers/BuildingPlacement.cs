using System;
using System.Collections.Generic;
using Entities;
using Managers;
using UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utilities;
using static Managers.GameManager;

namespace Controllers
{
    public class BuildingPlacement : MonoBehaviour
    {
        public static Action OnBuildingPlaced;

        public const int Deselected = -1;
        public static int Selected = Deselected;
    
        [SerializeField] private BuildingCard[] cards;
        [SerializeField] private LayerMask layerMask;
        [SerializeField] private GameObject particle;
        [SerializeField] private Transform container;
        [SerializeField] private GameObject testBuilding;
        private List<GameObject> _remainingBuildings = new List<GameObject>();
        private Camera _cam;
        private int _rotation;
        private Cell[] _highlighted = new Cell[0];
        private int _previousSelected = Selected;
        private ToggleGroup _toggleGroup;

        private void Start()
        {
            _cam = Camera.main;

            Click.OnLeftClick += LeftClick;
            Click.OnRightClick += RightClick;

            var canvasGroup = GetComponent<CanvasGroup>();
            OnNextTurn += () =>
            {
                NewCards();
                canvasGroup.interactable = false;
            };
            OnNewTurn += () =>
            {
                canvasGroup.interactable = true;
            };
        
            _remainingBuildings = Manager.BuildingCards.All;
            for (var i = 0; i < 3; i++) cards[i].buildingPrefab = _remainingBuildings.PopRandom();
            _toggleGroup = GetComponent<ToggleGroup>();
        }

        private void Update()
        {
#if UNITY_EDITOR
            //Random debug code
            //if (Input.GetKeyDown(KeyCode.F10))
            //{
            //    SetFirstCard(0);
            //}
#endif
            // Clear previous highlights
            Manager.Map.Highlight(_highlighted, Map.HighlightState.Inactive);
            _highlighted = new Cell[0];

            if (_previousSelected != Selected)
            {
                if (CursorSelect.Cursor.currentCursor != CursorSelect.CursorType.Destroy)
                {
                    var cursor = (Selected != Deselected)
                        ? CursorSelect.CursorType.Build
                        : CursorSelect.CursorType.Pointer;
                    CursorSelect.Cursor.Select(cursor);
                    _previousSelected = Selected;
                }
            }

            if (Selected == Deselected || EventSystem.current.IsPointerOverGameObject()) return;

            Cell closest = Manager.Map.GetClosestCellToCursor();
            if (closest == null || !closest.Active) return;
            
            Building building = cards[Selected].buildingPrefab.GetComponent<Building>();

            _highlighted = Manager.Map.GetCells(closest, building, _rotation).ToArray();

            Map.HighlightState state = Cell.IsValid(_highlighted) ? Map.HighlightState.Valid : Map.HighlightState.Invalid;
            Manager.Map.Highlight(_highlighted, state);

        }

        public bool ChangingCard(int cardNum) => cards[cardNum].isReplacing;

        public void ImitateHover(int cardNum)
        {

            if (cardNum >= 0)
            {
                if (cards[cardNum].isReplacing) return;
                cards[cardNum].SelectCard();
                if (cards[cardNum].toggle.interactable)
                {
                    cards[cardNum].toggle.isOn = true;
                }
            }

            for (int i = 0; i < cards.Length; i++)
            {
                if (i == cardNum) continue;

                cards[i].DeselectCard();
                if (cards[i].toggle.isOn) cards[i].toggle.isOn = false;
            }
        }

        public int NavigateCards(int newCardNum)
        {
            if (newCardNum > cards.Length - 1)
                newCardNum = 0;
            else if (newCardNum < 0)
                newCardNum = cards.Length - 1;

            return newCardNum;
        }

        private void LeftClick()
        {
            if (Selected == Deselected || EventSystem.current.IsPointerOverGameObject()) return;
            Click.PlacingBuilding = true;
            Ray ray = _cam.ScreenPointToRay(new Vector3(InputManager.MousePosition.x, InputManager.MousePosition.y, _cam.nearClipPlane));
            Physics.Raycast(ray, out RaycastHit hit, 200f, layerMask);

            if (!hit.collider || EventSystem.current.IsPointerOverGameObject()) return; // No placing through ui

            int i = Selected;
            GameObject buildingInstance = Instantiate(cards[i].buildingPrefab, container);
            
            if (!Manager.Map.CreateBuilding(buildingInstance,  Manager.Map.GetClosestCellToCursor().Id, _rotation, true))
            {
                Destroy(buildingInstance);
                return;
            }
            
            cards[i].SwitchCard(ChangeCard);
            cards[i].toggle.isOn = false;
            Selected = Deselected;
            
            Manager.UpdateUi();
            OnBuildingPlaced?.Invoke();
        }
    
        private void RightClick()
        {
            if (Selected == Deselected) return;
            _rotation++;
            _rotation %= 4;
        }
    
        public void NewCards()
        {
            _toggleGroup.SetAllTogglesOff();
            for (int i = 0; i < 3; i++) cards[i].SwitchCard(ChangeCard);
        }

        private void ChangeCard(int i)
        {
            if (_remainingBuildings.Count == 0) _remainingBuildings = Manager.BuildingCards.All;
            bool valid = false;

            // Confirm no duplicate buildings
            while (!valid)
            {
                valid = true;
                cards[i].buildingPrefab = _remainingBuildings.PopRandom();
                for (int j = 0; j < 3; j++)
                {
                    if (i == j) continue;
                    if (cards[j].buildingPrefab == cards[i].buildingPrefab) valid = false;
                }
            }

            Manager.UpdateUi();
        }

        private void SetFirstCard(int i)
        {
            if (_remainingBuildings.Count == 0) _remainingBuildings = Manager.BuildingCards.All;
            bool valid = false;

            // Confirm no duplicate buildings
            while (!valid)
            {
                valid = true;
                cards[i].buildingPrefab = testBuilding;
                for (int j = 0; j < 3; j++)
                {
                    if (i == j) continue;
                    if (cards[j].buildingPrefab == cards[i].buildingPrefab) valid = false;
                }
            }

            Manager.UpdateUi();
        }
    }
}
