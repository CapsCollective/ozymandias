using System.Collections.Generic;
using DG.Tweening;
using Entities;
using Managers;
using UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utilities;

namespace Controllers
{
    public class BuildingPlacement : MonoBehaviour
    {
        public const int Deselected = -1;
        public static int Selected = Deselected;
    
        [SerializeField] private BuildingCard[] cards;
        [SerializeField] private LayerMask layerMask;
        [SerializeField] private float tweenTime;
        [SerializeField] private Ease tweenEase;
        [SerializeField] private GameObject particle;
        [SerializeField] private Transform container;
        [SerializeField] private GameObject testBuilding;
        private List<GameObject> _remainingBuildings = new List<GameObject>();
        private Camera _cam;
        private int _rotation;
        private Cell[] _highlighted = new Cell[0];
        private int _previousSelected = Selected;

        private void Start()
        {
            _cam = Camera.main;

            Click.OnLeftClick += LeftClick;
            Click.OnRightClick += RightClick;

            var canvasGroup = GetComponent<CanvasGroup>();
            GameManager.OnNextTurn += () =>
            {
                NewCards();
                canvasGroup.interactable = false;
            };
            GameManager.OnNewTurn += () =>
            {
                canvasGroup.interactable = true;
            };
        
            _remainingBuildings = GameManager.Manager.BuildingCards.All;
            for (int i = 0; i < 3; i++) cards[i].buildingPrefab = _remainingBuildings.PopRandom();
        }

        private void Update()
        {
#if UNITY_EDITOR
            //Random debug code
            if (Input.GetKeyDown(KeyCode.F10))
            {
                SetFirstCard(0);
            }
#endif
            // Clear previous highlights
            GameManager.Manager.Map.Highlight(_highlighted, Map.HighlightState.Inactive);
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

            Cell closest = GameManager.Manager.Map.GetCellFromMouse();

            Building building = cards[Selected].buildingPrefab.GetComponent<Building>();

            _highlighted = GameManager.Manager.Map.GetCells(closest, building, _rotation);

            Map.HighlightState state = Map.IsValid(_highlighted) ? Map.HighlightState.Valid : Map.HighlightState.Invalid;
            GameManager.Manager.Map.Highlight(_highlighted, state);

        }

        private void LeftClick()
        {
            if (Selected == Deselected) return;
            
            Ray ray = _cam.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y, _cam.nearClipPlane));
            Physics.Raycast(ray, out RaycastHit hit, 200f, layerMask);

            if (!hit.collider || EventSystem.current.IsPointerOverGameObject()) return; // No placing through ui

            int i = Selected;
            GameObject buildingInstance = Instantiate(cards[i].buildingPrefab, container);
            buildingInstance.GetComponent<Building>().HasNeverBeenSelected = true;
            if (!GameManager.Manager.Map.CreateBuilding(
                buildingInstance, hit.point, _rotation, true)) return;
            
            // TODO get particles working again
            //Instantiate(particle, transform.parent).GetComponent<Trail>().SetTarget(cards[i].buildingPrefab.GetComponent<Building>().primaryStat);
    
            NewCardTween(i);
            cards[i].toggle.isOn = false;
            Selected = Deselected;
            
            BarFill.DelayBars = true;
            GameManager.Manager.UpdateUi();
            BarFill.DelayBars = false;
        }
    
        private void RightClick()
        {
            if (Selected == Deselected) return;
            _rotation++;
            _rotation %= 4;
        }
    
        public void NewCards()
        {
            GetComponent<ToggleGroup>().SetAllTogglesOff();
            for (int i = 0; i < 3; i++) NewCardTween(i);
        }

        private void NewCardTween(int i)
        {
            RectTransform t = cards[i].GetComponent<RectTransform>();
            cards[i].isReplacing = true;
            t.DOAnchorPosY(-100, tweenTime).SetEase(tweenEase).OnComplete(() =>
            {
                ChangeCard(i);
                cards[i].isReplacing = false;
                t.DOAnchorPosY(0, 0.5f).SetEase(tweenEase);
            });
        }

        private void ChangeCard(int i)
        {
            if (_remainingBuildings.Count == 0) _remainingBuildings = 
                GameManager.Manager.BuildingCards.All;
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

            GameManager.Manager.UpdateUi();
        }

        private void SetFirstCard(int i)
        {
            if (_remainingBuildings.Count == 0) _remainingBuildings = 
                GameManager.Manager.BuildingCards.All;
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

            GameManager.Manager.UpdateUi();
        }
    }
}
