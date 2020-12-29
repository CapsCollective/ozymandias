#pragma warning disable 0649
using System.Collections.Generic;
using DG.Tweening;
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
        public const int Deselected = -1;
        public static int Selected = Deselected;
    
        [SerializeField] private BuildingSelect[] cards;
        [SerializeField] private LayerMask layerMask;
        [SerializeField] private float tweenTime;
        [SerializeField] private Ease tweenEase;
        [SerializeField] private GameObject particle;
    
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

            OnNextTurn += NewCards;
        
            _remainingBuildings = Manager.BuildingCards.All;
            for (int i = 0; i < 3; i++) cards[i].buildingPrefab = _remainingBuildings.PopRandom();
        }

        private void Update()
        {
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

            Cell closest = Manager.Map.GetCellFromMouse();

            BuildingStructure building = cards[Selected].buildingPrefab.GetComponent<BuildingStructure>();

            _highlighted = Manager.Map.GetCells(closest, building, _rotation);

            Map.HighlightState state = Manager.Map.IsValid(_highlighted) ? Map.HighlightState.Valid : Map.HighlightState.Invalid;
            Manager.Map.Highlight(_highlighted, state);
        }

        private void LeftClick()
        {
            if (Selected == Deselected) return;
            Ray ray = _cam.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y, _cam.nearClipPlane));
            Physics.Raycast(ray, out RaycastHit hit, 200f, layerMask);

            if (!hit.collider || EventSystem.current.IsPointerOverGameObject()) return; // No placing through ui

            int i = Selected;
            GameObject buildingInstance = Instantiate(cards[i].buildingPrefab, GameObject.Find("Buildings").transform);
            if (!Manager.Map.CreateBuilding(buildingInstance, hit.point, _rotation, true)) return;
        
            Instantiate(particle, transform.parent).GetComponent<Trail>().SetTarget(cards[i].buildingPrefab.GetComponent<BuildingStats>().primaryStat);
        
            NewCardTween(i);
            cards[i].toggle.isOn = false;
            Selected = Deselected;

            BarFill.DelayBars = true;
            Manager.UpdateUi();
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
    }
}
