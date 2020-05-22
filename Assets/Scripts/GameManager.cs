using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;
using NaughtyAttributes;
using Random = UnityEngine.Random;

public enum Metric
{
    Accomodation,
    Satisfaction,
    Effectiveness,
    Spending,
    Defense,
    Threat, //per turn
    Chaos,
}

public enum BuildingType
{
    GuildHall,
    Inn,
    Blacksmith,
    ItemShop,
    Bar
}

public class GameManager : MonoBehaviour
{
    public static Action OnNewTurn;
    public static Action OnUpdateUI;
    
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


    public Dictionary<Metric, int> modifiers = new Dictionary<Metric, int>();
    
    [ReadOnly] [SerializeField] private List<Adventurer> adventurers = new List<Adventurer>();

    [ReadOnly] [SerializeField] public List<BuildingStats> buildings = new List<BuildingStats>();
    
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
        get { return satisfaction = buildings.Where(x => x.operational).Sum(x => x.satisfaction) + modifiers[Metric.Satisfaction]; }
    }

    [ReadOnly] [SerializeField] private int effectiveness;
    public int Effectiveness
    {
        get { return effectiveness = buildings.Where(x => x.operational).Sum(x => x.effectiveness) + modifiers[Metric.Effectiveness]; }
    }

    [ReadOnly] [SerializeField] private int spending;
    public int Spending
    {
        get { return spending = buildings.Where(x => x.operational).Sum(x => x.spending) + modifiers[Metric.Spending]; }
    }

    [ReadOnly] [SerializeField] private int defense;
    public int Defense
    {
        get
        {
            return defense = AvailableAdventurers * Effectiveness +
                             buildings.Where(x => x.operational).Sum(x => x.defense) +
                             modifiers[Metric.Defense];
        }
    }

    [ReadOnly] [SerializeField] private int chaos;
    public int Chaos
    {
        get { return chaos = AvailableAdventurers / (Satisfaction + 1) + modifiers[Metric.Chaos]; }
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

    public bool Spend(int amount)
    {
        if (currentWealth >= amount)
        {
            currentWealth -= amount;
            return true;
        }
        return false;
    }
    
    public void AddAdventurer()
    {
        adventurers.Add(Instantiate(adventurerPrefab, GameObject.Find("Adventurers").transform).GetComponent<Adventurer>());
    }
    public void AddAdventurer(AdventurerDetails adventurerDetails)
    {
        Adventurer adventurer = Instantiate(adventurerPrefab, GameObject.Find("Adventurers").transform)
            .GetComponent<Adventurer>();
        adventurer.name = adventurerDetails.name;
        adventurer.category = adventurerDetails.category;
        adventurer.isSpecial = adventurerDetails.isSpecial;
        adventurers.Add(adventurer);
    }
    
    public void RemoveAdventurer(bool kill) //Removes a random adventurer, ensuring they aren't special
    {
        Adventurer toRemove = adventurers[Random.Range(0, adventurers.Count)];
        if (toRemove.isSpecial)
        {
            RemoveAdventurer(kill); // Try again!
        }
        else
        {
            adventurers.Remove(toRemove);
            if (kill) toRemove.transform.parent = GameObject.Find("Graveyard").transform; //I REALLY hope we make use of this at some point
        }
    }
    public void RemoveAdventurer(string adventurerName, bool kill) // Deletes an adventurer by name
    {
        Adventurer toRemove = GameObject.Find(adventurerName)?.GetComponent<Adventurer>();
        if (!toRemove) return;
        adventurers.Remove(toRemove);
        if (kill) toRemove.transform.parent = GameObject.Find("Graveyard").transform;
    }
    
    [Button("StartGame")]
    public void StartGame()
    {
        // Clear out all adventurers and buildings
        foreach (Transform child in GameObject.Find("Adventurers").transform) Destroy(child.gameObject);
        foreach (Transform child in GameObject.Find("Buildings").transform) Destroy(child.gameObject);
        adventurers = new List<Adventurer>();
        buildings = new List<BuildingStats>();
        // Set all mods to 0 at start
        foreach (Metric mod in Enum.GetValues(typeof(Metric))) modifiers.Add(mod, 0);

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
        //if (adventurers.Count() < Accommodation) AddAdventurer();
        Threat += buildings.Count + modifiers[Metric.Threat];
        CurrentWealth = WealthPerTurn;

        foreach (Metric mod in Enum.GetValues(typeof(Metric))) modifiers[mod] = 0;

        OnNewTurn?.Invoke();
        UpdateUi();
    }

    public void Build(BuildingStats building)
    {
        //CurrentWealth -= building.baseCost;
        buildings.Add(building);
        UpdateUi();
    }

    public void Demolish(BuildingStats building)
    {
        map.Clear(building.GetComponent<BuildingStructure>());
        if (!building.terrain) buildings.Remove(building);
        Destroy(building.gameObject);
        UpdateUi();
    }
    
    public void UpdateUi()
    {
        OnUpdateUI?.Invoke();
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

    public int BuildingCount(BuildingType type)
    {
        return buildings.Count(x => x.type == type);
    }
    

    [HorizontalLine()] 
    
    public GameObject adventurerPrefab;
    public DialogueManager dialogueManager;
    public Map map;
    public GameObject guildHall;

    private void BuildGuildHall()
    {
        //Build Guild Hall in the center of the map
        map.CreateBuilding(guildHall, map.transform.position);
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
