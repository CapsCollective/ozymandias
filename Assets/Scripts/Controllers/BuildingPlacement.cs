using System.Collections.Generic;
using DG.Tweening;
using UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utilities;
using static GameManager;

namespace Controllers
{
    public class BuildingPlacement : MonoBehaviour
    {
        public const int Deselected = -1;
        public static int Selected = Deselected;
    
        #pragma warning disable 0649
        [SerializeField] private BuildingSelect[] cards;
        [SerializeField] private LayerMask layerMask;
        [SerializeField] private float tweenTime;
        [SerializeField] private Ease tweenEase;
        [SerializeField] private GameObject particle;
    
        private List<GameObject> remainingBuildings = new List<GameObject>();
        private RaycastHit hit;
        private Camera cam;
        private int rotation;
        private Cell[] highlighted = new Cell[0];
        private int previousSelected = Selected;
    
        private void Start()
        {
            cam = Camera.main;
        
            ClickManager.OnLeftClick += LeftClick;
            ClickManager.OnRightClick += RightClick;

            OnNextTurn += NewCards;
        
            remainingBuildings = Manager.BuildingCards.All;
            for (int i = 0; i < 3; i++) cards[i].buildingPrefab = remainingBuildings.PopRandom();
        }

        void Update()
        {
            // Clear previous highlights
            Manager.Map.Highlight(highlighted, Map.HighlightState.Inactive);
            highlighted = new Cell[0];

            if (previousSelected != Selected)
            {
                if (CursorSelect.Cursor.currentCursor != CursorSelect.CursorType.Destroy)
                {
                    var cursor = (Selected != Deselected)
                        ? CursorSelect.CursorType.Build
                        : CursorSelect.CursorType.Pointer;
                    CursorSelect.Cursor.Select(cursor);
                    previousSelected = Selected;
                }
            }

            if (Selected == Deselected || EventSystem.current.IsPointerOverGameObject()) return;

            Cell closest = Manager.Map.GetCellFromMouse();

            BuildingStructure building = cards[Selected].buildingPrefab.GetComponent<BuildingStructure>();

            highlighted = Manager.Map.GetCells(closest, building, rotation);

            Map.HighlightState state = Manager.Map.IsValid(highlighted) ? Map.HighlightState.Valid : Map.HighlightState.Invalid;
            Manager.Map.Highlight(highlighted, state);
        }

        private void LeftClick()
        {
            if (Selected == Deselected) return;
            Ray ray = cam.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y, cam.nearClipPlane));
            Physics.Raycast(ray, out hit, 200f, layerMask);

            if (!hit.collider || EventSystem.current.IsPointerOverGameObject()) return; // No placing through ui

            int i = Selected;
            GameObject buildingInstance = Instantiate(cards[i].buildingPrefab, GameObject.Find("Buildings").transform);
            if (!Manager.Map.CreateBuilding(buildingInstance, hit.point, rotation, true)) return;
        
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
            rotation++;
            rotation %= 4;
        }
    
        public void NewCards()
        {
            GetComponent<ToggleGroup>().SetAllTogglesOff();
            for (int i = 0; i < 3; i++) NewCardTween(i);
        }

        public void NewCardTween(int i)
        {
            RectTransform t = cards[i].GetComponent<RectTransform>();
            t.DOAnchorPosY(-100, tweenTime).SetEase(tweenEase).OnComplete(() =>
            {
                ChangeCard(i);
                t.DOAnchorPosY(0, 0.5f).SetEase(tweenEase);
            });
        }

        private void ChangeCard(int i)
        {
            if (remainingBuildings.Count == 0) remainingBuildings = Manager.BuildingCards.All;
            bool valid = false;

            // Confirm no duplicate buildings
            while (!valid)
            {
                valid = true;
                cards[i].buildingPrefab = remainingBuildings.PopRandom();
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
