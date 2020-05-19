using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;
using NaughtyAttributes;

public enum Metric
{
    Accomodation,
    Satisfaction,
    Effectiveness,
    Spending,
    Defense
}

public class GameManager : MonoBehaviour
{

    public static Action OnNewTurn;
    private static GameManager instance;

    public static GameManager Manager
    {
        get
        {
            if (!instance)
                instance = Resources.FindObjectsOfTypeAll<GameManager>().FirstOrDefault();
            return instance;
        }
    }

    [ReadOnly] [SerializeField] private List<Adventurer> adventurers = new List<Adventurer>();

    [ReadOnly] [SerializeField] public List<Building> buildings = new List<Building>();

    /*public List<Building> Buildings
    {
        get
        {
            return GameObject.FindGameObjectsWithTag("Building").Select(x => x.GetComponent<Building>()).ToList();
        }
    }*/

    [ReadOnly] [SerializeField] private int availableAdventurers;

    public int AvailableAdventurers
    {
        get { return availableAdventurers = adventurers.Count(x => x.assignedQuest == null); }
    }

    [ReadOnly] [SerializeField] private int accommodation;

    public int Accommodation
    {
        get { return accommodation = buildings.Where(x => x.operational).Sum(x => x.accommodation); }
    }

    [ReadOnly] [SerializeField] private int satisfaction;

    public int Satisfaction
    {
        get { return satisfaction = buildings.Where(x => x.operational).Sum(x => x.satisfaction); }
    }

    [ReadOnly] [SerializeField] private int effectiveness;

    public int Effectiveness
    {
        get { return effectiveness = buildings.Where(x => x.operational).Sum(x => x.effectiveness); }
    }

    [ReadOnly] [SerializeField] private int spending;

    public int Spending
    {
        get { return spending = buildings.Where(x => x.operational).Sum(x => x.spending); }
    }

    [ReadOnly] [SerializeField] private int defense;

    public int Defense
    {
        get
        {
            return defense = AvailableAdventurers * Effectiveness +
                             buildings.Where(x => x.operational).Sum(x => x.defense);
        }
    }

    [ReadOnly] [SerializeField] private int chaos;

    public int Chaos
    {
        get { return chaos = AvailableAdventurers / (Satisfaction + 1); }
    }

    [ReadOnly] [SerializeField] private int wealthPerTurn;

    public int WealthPerTurn
    {
        get { return wealthPerTurn = Spending * AvailableAdventurers; }
    }

    [SerializeField] private int threat;

    public int Threat
    {
        get { return threat; }
        private set { threat = value; }
    }

    [SerializeField] private int currentWealth;

    public int CurrentWealth
    {
        get { return currentWealth; }
        private set { currentWealth = value; }
    }

    public void AddAdventurer()
    {
        adventurers.Add(Instantiate(adventurerPrefab, GameObject.Find("Adventurers").transform).GetComponent<Adventurer>());
        
    }
    
    [Button("StartGame")]
    public void StartGame()
    {
        // Clear out all adventurers and buildings
        foreach (Transform child in GameObject.Find("Adventurers").transform) Destroy(child.gameObject);
        foreach (Transform child in GameObject.Find("Buildings").transform) Destroy(child.gameObject);
        adventurers = new List<Adventurer>();
        buildings = new List<Building>();

        // Start game with 5 Adventurers
        for (int i = 0; i < 5; i++) AddAdventurer();
        

        CurrentWealth = 50;
        threat = 1;
        BuildGuildHall();
        
        // Run the menu tutorial system dialogue
        // Commented out for now, players can trigger from the help button
        dialogueManager?.StartDialogue("menu_tutorial");
    }

    [Button("Next Turn")]
    public void NextTurn()
    {
        if (adventurers.Count() < Accommodation) AddAdventurer();
        Threat += buildings.Count;
        CurrentWealth = WealthPerTurn;

        UpdateUi();
        OnNewTurn?.Invoke();
    }

    public void Build(Building building)
    {
        CurrentWealth -= building.baseCost;
        buildings.Add(building);
        UpdateUi();
    }

    public UiUpdater[] uiUpdates;
    public void UpdateUi()
    {
        foreach (var uiUpdater in uiUpdates) uiUpdater.UpdateUi();        
    }

    private void Start()
    {
        StartGame();
    }

    public int GetMetric(Metric metric)
    {
        switch (metric)
        {
            case Metric.Accomodation: return Accommodation;
            case Metric.Satisfaction: return Satisfaction;
            case Metric.Effectiveness: return Effectiveness;
            case Metric.Spending: return Spending;
            case Metric.Defense: return Defense;
            default: return 0;
        }
    }

    [HorizontalLine()] 
    
    public GameObject adventurerPrefab;
    public DialogueManager dialogueManager;
    public Map map;
    public GameObject guildHall;

    private void BuildGuildHall()
    {
        //TODO: Should place in the center of your town
        //Need to call the click place manager for a virtual place if possible
        //Instantiate(guildHall, GameObject.Find("Buildings").transform).GetComponent<Building>().Build();
        
        //Build Guild Hall in the center of the map
        //map.Occupy(guildHall, map.transform.position);

        // This how we do it now ->
        BuildingPlacement.Building guildHallBuilding = Instantiate(guildHall).GetComponent<BuildingPlacement.Building>();

        Cell root = map.GetCell(map.transform.position);
        Cell[] cells = map.GetCells(root, guildHallBuilding);

        if (map.Validate(cells))
            map.Occupy(guildHallBuilding, cells);
        else
            Destroy(guildHallBuilding.gameObject);
        // <-
    }


    private List<string> shownTutorials = new List<string>();
    public void ShowTutorial(string tutorialName)
    {
        if (shownTutorials.Contains(tutorialName))
            return;
        dialogueManager.StartDialogue(tutorialName);
        shownTutorials.Add(tutorialName);
    }

}
