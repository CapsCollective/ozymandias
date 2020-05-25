using System;
using System.Collections.Generic;
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
    Threat, // Mod changes per turn, not total
    Chaos,
    // Effectiveness Subcategories
    Weaponry,
    Magic,
    Equipment,
    Training,
    // Satisfaction Subcategories
    Food,
    Entertainment,
    Luxuries,
}

public enum BuildingType
{
    Terrain,
    Tavern,
    Blacksmith,
    GuildHall,
    Inn,
    GeneralStore,
    House
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
                instance = FindObjectsOfType<GameManager>()[0];
            return instance;
        }
    }

    public Dictionary<Metric, int> modifiers = new Dictionary<Metric, int>();
    
    [ReadOnly] [SerializeField] private List<Adventurer> adventurers = new List<Adventurer>();

    [ReadOnly] [SerializeField] public List<BuildingStats> buildings = new List<BuildingStats>();
    
    [ReadOnly] [SerializeField] private int availableAdventurers;
    public int AvailableAdventurers
    {
        get { return availableAdventurers = adventurers.Count(x => !x.assignedQuest); }
    }

    [ReadOnly] [SerializeField] private int accommodation;
    public int Accommodation
    {
        get { return accommodation = buildings.Where(x => x.operational).Sum(x => x.accommodation); }
    }

    [HorizontalLine]
    
    [ReadOnly] [SerializeField] private int weaponry;
    public int Weaponry
    {
        get { return weaponry = buildings.Where(x => x.operational).Sum(x => x.weaponry); }
    }
    
    [ReadOnly] [SerializeField] private int magic;
    public int Magic
    {
        get { return magic = buildings.Where(x => x.operational).Sum(x => x.magic); }
    }
    
    [ReadOnly] [SerializeField] private int equipment;
    public int Equipment
    {
        get { return equipment = buildings.Where(x => x.operational).Sum(x => x.equipment); }
    }

    [ReadOnly] [SerializeField] private int training;
    public int Training
    {
        get { return training = 0; } // TODO: work out the specifics of this
    }
    
    [ReadOnly] [SerializeField] private int effectiveness;
    public int Effectiveness
    {
        get
        {
            return effectiveness = Mathf.Clamp(0, 
                Mathf.Clamp(25 * Weaponry / AvailableAdventurers, 0, 25) +
                Mathf.Clamp(25 * Magic / AvailableAdventurers, 0, 25) +
                Mathf.Clamp(25 * Equipment / AvailableAdventurers, 0, 25) +
                Mathf.Clamp(25 * Training / AvailableAdventurers, 0, 25) +
                modifiers[Metric.Effectiveness], 100);
        }
    }
    
    [HorizontalLine]
    
    [ReadOnly] [SerializeField] private int food;
    public int Food
    {
        get { return food = buildings.Where(x => x.operational).Sum(x => x.food); }
    }
    
    [ReadOnly] [SerializeField] private int entertainment;
    public int Entertainment
    {
        get { return entertainment = buildings.Where(x => x.operational).Sum(x => x.entertainment); }
    }
    
    [ReadOnly] [SerializeField] private int luxury;
    public int Luxury
    {
        get { return effectiveness = buildings.Where(x => x.operational).Sum(x => x.luxury); }
    }
    
    [ReadOnly] [SerializeField] private int satisfaction;
    public int Satisfaction
    {
        get
        {
            return satisfaction = Mathf.Clamp(0, 
                Mathf.Clamp(40 * Food / AvailableAdventurers, 0, 40) +
                Mathf.Clamp(40 * Entertainment / AvailableAdventurers, 0, 40) +
                Mathf.Clamp(20 * Luxury / AvailableAdventurers, 0, 20) +
                Mathf.Min(0, Accommodation - AvailableAdventurers) + //lose 1% satisfaction per adventurer over capacity
                modifiers[Metric.Satisfaction], 100);
        }
    }

    [HorizontalLine]
    
    [ReadOnly] [SerializeField] private int spending;
    public int Spending
    {
        get { return spending = 100 + buildings.Where(x => x.operational).Sum(x => x.spending) + modifiers[Metric.Spending]; }
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
        get { return chaos = AvailableAdventurers * (100 - Satisfaction); }
    }

    [ReadOnly] [SerializeField] private int wealthPerTurn;
    public int WealthPerTurn
    {
        get { return wealthPerTurn = Spending * AvailableAdventurers / 10; } //10 gold per adventurer times spending
    }
    
    [SerializeField] private int wealth;
    public int Wealth
    {
        get { return wealth; }
        private set { wealth = value; }
    }    
    
    [ReadOnly] [SerializeField] private int threatPerTurn;
    public int ThreatPerTurn
    {
        get { return buildings.Count + modifiers[Metric.Threat]; } //10 gold per adventurer times spending
    }
    
    [SerializeField] private int threat;
    public int Threat
    {
        get { return threat; }
        private set { threat = value; }
    }

    public bool Spend(int amount)
    {
        if (wealth >= amount)
        {
            wealth -= amount;
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

    public bool RemoveAdventurer(bool kill) //Removes a random adventurer, ensuring they aren't special
    {
        int randomIndex = Random.Range(0, adventurers.Count);
        for (int i = 0; i < adventurers.Count; i++)
        {
            Adventurer toRemove = adventurers[(i + randomIndex) % adventurers.Count];
            if (toRemove.isSpecial || toRemove.assignedQuest) continue;
            
            adventurers.Remove(toRemove);
            if (kill) toRemove.transform.parent = graveyard.transform; //I REALLY hope we make use of this at some point
            return true;
        }

        return false;
    }

    public bool RemoveAdventurer(string adventurerName, bool kill) // Deletes an adventurer by name
    {
        Adventurer toRemove = GameObject.Find(adventurerName)?.GetComponent<Adventurer>();
        if (!toRemove) return false;
        adventurers.Remove(toRemove);
        if (kill) toRemove.transform.parent = graveyard.transform;
        return true;
    }
    
    [Button("StartGame")]
    public void StartGame()
    {
        // Clear out all adventurers and buildings
        foreach (Transform child in adventurersContainer.transform) Destroy(child.gameObject);
        foreach (Transform child in buildingsContainer.transform) Destroy(child.gameObject);
        adventurers = new List<Adventurer>();
        buildings = new List<BuildingStats>();
        // Set all mods to 0 at start
        foreach (Metric mod in Enum.GetValues(typeof(Metric))) modifiers.Add(mod, 0);

        // Start game with 5 Adventurers
        for (int i = 0; i < 5; i++) AddAdventurer();

        wealth = 50;
        threat = 1;
        BuildGuildHall();
        
        eventQueue.AddEvent(openingEvent, true);
        
        // Run the menu tutorial system dialogue
        dialogueManager.StartDialogue("menu_tutorial");
    }

    [Button("Next Turn")]
    public void NextTurn()
    {
        Threat += buildings.Count + modifiers[Metric.Threat];
        Wealth += WealthPerTurn;

        foreach (Metric mod in Enum.GetValues(typeof(Metric))) modifiers[mod] = 0;

        eventQueue.ProcessEvents();

        OnNewTurn?.Invoke();
        
        UpdateUi();
    }

    public void Build(BuildingStats building)
    {
        buildings.Add(building);
        UpdateUi();
    }

    public void Demolish(BuildingStats building)
    {
        if (building.type == BuildingType.GuildHall)
        {
            //TODO: Add an 'are you sure?' dialogue
            foreach (var e in guildHallDestroyedEvents) eventQueue.AddEvent(e, true);
            NextTurn();
        }
        
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

    public void GameOver()
    {
        newspaperController.GameOver();            
    }

    [HorizontalLine()] 
    
    public Map map;
    public EventQueue eventQueue;
    public DialogueManager dialogueManager;
    public NewspaperController newspaperController;
    public MenuManager menuManager;


    public GameObject adventurersContainer, buildingsContainer, graveyard;
    public GameObject adventurerPrefab;
    public GameObject guildHall;
    public Event openingEvent;
    public Event[] guildHallDestroyedEvents;
    
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
