using System.Collections.Generic;
using System.Collections;
using DG.Tweening;
using Managers_and_Controllers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static GameManager;

public class PlacementManager : MonoBehaviour
{
    public const int Deselected = -1;
    public static int Selected = Deselected;
    
    #pragma warning disable 0649
    [SerializeField] private BuildingSelect[] cards;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private float tweenTime;
    [SerializeField] private Ease tweenEase;
    [SerializeField] private GameObject particle;

    /////////////////Particles/////////////////////////
    //tracking instantiated object
    private GameObject trail;
    //directional information
    private Transform particleStart;
    private GameObject particleEnd;
    private Transform[] directions = new Transform[3];
    //////////////////////////////////////////////////
    private List<GameObject> remainingBuildings = new List<GameObject>();
    private Map map;
    private RaycastHit hit;
    private Camera cam;
    private int rotation;
    private Cell[] highlighted = new Cell[0];
    private int previousSelected = Selected;
    
    private void Awake()
    {
        cam = Camera.main;
        map = Manager.map;
        
        ClickManager.OnLeftClick += LeftClick;
        ClickManager.OnRightClick += RightClick;

        OnNextTurn += NewCards;
        
        remainingBuildings = new List<GameObject>(BuildingManager.BuildManager.AllBuildings);
        for (int i = 0; i < 3; i++) cards[i].buildingPrefab = remainingBuildings.PopRandom();
    }

    void Update()
    {
        // Clear previous highlights
        map.Highlight(highlighted, Map.HighlightState.Inactive);
        highlighted = new Cell[0];

        if (previousSelected != Selected)
        {
            if (CursorController.Instance.currentCursor != CursorController.CursorType.Destroy)
            {
                var cursor = (Selected != Deselected)
                    ? CursorController.CursorType.Build
                    : CursorController.CursorType.Pointer;
                CursorController.Instance.SwitchCursor(cursor);
                previousSelected = Selected;
            }
        }

        if (Selected == Deselected || EventSystem.current.IsPointerOverGameObject()) return;

        Cell closest = map.GetCellFromMouse();

        BuildingStructure building = cards[Selected].buildingPrefab.GetComponent<BuildingStructure>();

        highlighted = map.GetCells(closest, building, rotation);

        Map.HighlightState state = map.IsValid(highlighted) ? Map.HighlightState.Valid : Map.HighlightState.Invalid;
        map.Highlight(highlighted, state);
    }

    private void LeftClick()
    {
        if (Selected == Deselected) return;
        Ray ray = cam.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y, cam.nearClipPlane));
        Physics.Raycast(ray, out hit, 200f, layerMask);

        if (!hit.collider || EventSystem.current.IsPointerOverGameObject()) return; // No placing through ui
        int i = Selected;
        if (map.CreateBuilding(cards[i].buildingPrefab, hit.point, rotation))
        {
            Metric mainStat = cards[i].buildingPrefab.GetComponent<BuildingStats>().primaryStat;
            if (mainStat == Metric.Food || mainStat == Metric.Luxuries|| mainStat == Metric.Entertainment || mainStat == Metric.Equipment || 
                mainStat == Metric.Magic || mainStat == Metric.Weaponry || mainStat == Metric.Threat)
            {
                if (mainStat == Metric.Food)
                {
                    particleEnd = GameObject.Find("Food Bar");
                }
                if (mainStat == Metric.Luxuries)
                {
                    particleEnd = GameObject.Find("Luxury Bar");
                }
                if (mainStat == Metric.Entertainment)
                {
                    particleEnd = GameObject.Find("Entertainment Bar");
                }
                if (mainStat == Metric.Equipment)
                {
                    particleEnd = GameObject.Find("Equipment Bar");
                }
                if (mainStat == Metric.Magic)
                {
                    particleEnd = GameObject.Find("Magic Bar");
                }
                if (mainStat == Metric.Weaponry)
                {
                    particleEnd = GameObject.Find("Weaponry Bar");
                }
                if (mainStat == Metric.Defense)
                {
                    particleEnd = GameObject.Find("Threat Bar");
                }
                Vector3 particleStartVector = Input.mousePosition;
                GameObject mouseLocation = new GameObject();
                mouseLocation.transform.position = particleStartVector;
                particleStart = mouseLocation.transform;

                //Instantiate trail object
                trail = Instantiate(particle, transform.position, Quaternion.identity);
                trail.transform.SetParent(transform);
                //change particle color depending on location
                var particleMain = trail.GetComponent<ParticleSystem>().main;
                particleMain.startColor = particleEnd.transform.Find("Mask").Find("Fill").GetComponent<Image>().color;
                //create directions for the trail object [start, curve, end]
                directions[0] = particleStart;
                directions[2] = particleEnd.transform; 
                Vector3 curve = new Vector3();
                curve = particleStart.position + particleEnd.transform.position;
                GameObject placeholder = new GameObject();
                placeholder.transform.position = new Vector3(curve.x * 1 / 3, curve.y * 2 / 3, 0);
                directions[1] = placeholder.transform;

                //pass directions to trail object and begin coroutine to vary target shape
                trail.GetComponent<Trail>().waypointArray = directions;
                StartCoroutine(Scale(placeholder));
            }
            NewCardTween(i);
            cards[i].toggle.isOn = false;
            Selected = Deselected;
        }
        Manager.UpdateUi();
    }
    
    private void RightClick()
    {
        if (Selected == Deselected) return;
        rotation++;
    }

    public void NewCards()
    {
        GetComponent<ToggleGroup>().SetAllTogglesOff();
        for (int i = 0; i < 3; i++) NewCardTween(i);
    }

    public void NewCardTween(int i)
    {
        RectTransform transform = cards[i].GetComponent<RectTransform>();
        transform.DOAnchorPosY(-100, tweenTime).SetEase(tweenEase).OnComplete(() =>
        {
            ChangeCard(i);
            transform.DOAnchorPosY(0, 0.5f).SetEase(tweenEase);
        });
    }

    private void ChangeCard(int i)
    {
        if (remainingBuildings.Count == 0) remainingBuildings = new List<GameObject>(BuildingManager.BuildManager.AllBuildings);
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

    //scale the object the particles are flying to
    IEnumerator Scale(GameObject placeholder)
    {
        yield return new WaitForSeconds(0.7f);
        iTween.ScaleAdd(particleEnd, new Vector3(0.1f, 0.1f, 0.1f), 0.1f);
        yield return new WaitForSeconds(0.1f);
        iTween.ScaleAdd(particleEnd, new Vector3(-0.1f, -0.1f, -0.1f), 0.1f);
        yield return new WaitForSeconds(0.1f);
        iTween.ScaleAdd(particleEnd, new Vector3(0.1f, 0.1f, 0.1f), 0.1f);
        yield return new WaitForSeconds(0.05f);
        iTween.ScaleAdd(particleEnd, new Vector3(-0.1f, -0.1f, -0.1f), 0.1f);
        Destroy(placeholder);
    }
}