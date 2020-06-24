using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Managers_and_Controllers;
using NaughtyAttributes;
using UnityEngine.Analytics;
using UnityEngine.UI;
using static AchievementManager;
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
    Ruins,
    GuildHall,
    //Equipment
    GeneralStore, //Better name?
    Tailor,
    Apothecary,
    //Weaponry
    Blacksmith,
    Leatherworks,
    Armoury, //Doubles as Defense
    //Magic
    Alchemists,
    Enchanters,
    Jewellers, //Doubles as Luxury
    //Training: Class specific expensive buildings (quest unlocks?)
    Arena, //Doubles as Entertainment
    HuntingLodge, //Doubles as Food
    PerformanceHall, //Doubles as Entertainment
    Monastery, //Doubles as Accommodation
    Library, //Doubles as Magic
    //Food
    Farm,
    Bakery,
    Brewery,
    //Entertainment
    Tavern,
    Plaza,
    Bathhouse,
    //Luxury
    Herbalist,
    Cartographers,
    //Accommodation
    Inn,
    House,
    Barracks, // Doubles as housing
    //Defense
    GuardOutpost,
    //Misc
    Graveyard,
    Lake
}

public class GameManager : MonoBehaviour
{
    public static Action OnNewTurn;
    public static Action OnNextTurn;
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
    
    [ReadOnly] public List<Adventurer> adventurers = new List<Adventurer>();

    [ReadOnly] public List<BuildingStats> buildings = new List<BuildingStats>();
    
    [ReadOnly] [SerializeField] private int totalAdventurers;
    public int TotalAdventurers => totalAdventurers = adventurers.Count;
    
    [ReadOnly] [SerializeField] private int availableAdventurers;
    public int AvailableAdventurers => availableAdventurers = adventurers.Count(x => !x.assignedQuest);
    
    [ReadOnly] [SerializeField] private int removableAdventurers;
    public int RemovableAdventurers => removableAdventurers = adventurers.Count(x => !x.assignedQuest && !x.isSpecial);
    
    [ReadOnly] [SerializeField] private int accommodation;
    public int Accommodation => accommodation = buildings.Where(x => x.operational).Sum(x => x.accommodation);

    [HorizontalLine]
    
    [ReadOnly] [SerializeField] private int weaponry;
    public int Weaponry => weaponry = Mathf.Clamp(100 * buildings.Where(x => x.operational).Sum(x => x.weaponry) / AvailableAdventurers, 0, 100);
    
    [ReadOnly] [SerializeField] private int magic;
    public int Magic => magic = Mathf.Clamp(100 * buildings.Where(x => x.operational).Sum(x => x.magic) / AvailableAdventurers, 0, 100);
    
    [ReadOnly] [SerializeField] private int equipment;
    public int Equipment => equipment = Mathf.Clamp(100 * buildings.Where(x => x.operational).Sum(x => x.equipment) / AvailableAdventurers,0, 100);

    [ReadOnly] [SerializeField] private int training;
    public int Training => training = 0; // TODO: work out the specifics of this

    [ReadOnly] [SerializeField] private int effectiveness;
    public int Effectiveness => effectiveness = Mathf.Clamp(1 + Equipment/3 + Weaponry/3 + Magic/3 + modifiers[Metric.Effectiveness], 0, 100);
    
    [HorizontalLine]
    
    [ReadOnly] [SerializeField] private int food;
    public int Food => food = Mathf.Clamp(100 * buildings.Where(x => x.operational).Sum(x => x.food) / AvailableAdventurers, 0, 100);

    [ReadOnly] [SerializeField] private int entertainment;
    public int Entertainment => entertainment = Mathf.Clamp(100 * buildings.Where(x => x.operational).Sum(x => x.entertainment) / AvailableAdventurers, 0, 100);
    
    [ReadOnly] [SerializeField] private int luxury;
    public int Luxury => luxury = Mathf.Clamp(100 * buildings.Where(x => x.operational).Sum(x => x.luxury) / AvailableAdventurers, 0, 100);

    public int OvercrowdingMod => Mathf.Min(0, (Accommodation - AvailableAdventurers) * 5); //lose 5% satisfaction per adventurer over capacity
        
    [ReadOnly] [SerializeField] private int satisfaction;
    public int Satisfaction => satisfaction = Mathf.Clamp(0, 1+ Food/3 + Entertainment/3 + Luxury/3 + OvercrowdingMod + modifiers[Metric.Satisfaction], 100);

    [HorizontalLine]
    
    [ReadOnly] [SerializeField] private int spending;
    public int Spending => spending = 100 + buildings.Where(x => x.operational).Sum(x => x.spending) + modifiers[Metric.Spending];
    
    [ReadOnly] [SerializeField] private int chaos;
    public int Chaos => chaos = AvailableAdventurers * (100 - Satisfaction);

    [ReadOnly] [SerializeField] private int wealthPerTurn;
    public int WealthPerTurn => wealthPerTurn = Spending * AvailableAdventurers / 10; //10 gold per adventurer times spending

    [SerializeField] private int wealth;
    public int Wealth => wealth;
    
    [ReadOnly] [SerializeField] private int defense;
    public int Defense => defense = 
        AvailableAdventurers * Effectiveness / 30 + 
        buildings.Where(x => x.operational).Sum(x => x.defense) + modifiers[Metric.Defense];
    
    [ReadOnly] [SerializeField] private int threat;
    public int Threat => threat = 12 + (3 * turnCounter) + modifiers[Metric.Threat];

    public int ChangePerTurn => Threat - Defense; // How much the top bar shifts each turn
    
    [SerializeField] private int threatLevel;
    public int ThreatLevel => threatLevel; // Percentage of how far along the threat is.

    public bool Spend(int amount)
    {
        if (wealth < amount) return false;
        wealth -= amount;
        return true;
    }

    public Adventurer AssignAdventurer(Quest q)
    {
        List<Adventurer> removable = adventurers.Where(x => !(x.assignedQuest || x.isSpecial)).ToList();
        if (removable.Count == 0) return null;

        int randomIndex = Random.Range(0, removable.Count);
        removable[randomIndex].assignedQuest = q;
        return removable[randomIndex];
    }
    
    public void AddAdventurer()
    {
        adventurers.Add(Instantiate(adventurerPrefab, GameObject.Find("Adventurers").transform).GetComponent<Adventurer>());
        Achievements.SetCitySize(TotalAdventurers);
    }
    
    public void AddAdventurer(AdventurerDetails adventurerDetails)
    {
        Adventurer adventurer = Instantiate(adventurerPrefab, GameObject.Find("Adventurers").transform)
            .GetComponent<Adventurer>();
        adventurer.name = adventurerDetails.name;
        adventurer.category = adventurerDetails.category;
        adventurer.isSpecial = adventurerDetails.isSpecial;
        adventurers.Add(adventurer);
        Achievements.SetCitySize(TotalAdventurers);
    }

    public bool RemoveAdventurer(bool kill) //Removes a random adventurer, ensuring they aren't special
    {
        List<Adventurer> removable = adventurers.Where(x => !(x.assignedQuest || x.isSpecial)).ToList();
        if (removable.Count == 0) return false;
        int randomIndex = Random.Range(0, removable.Count);
        Adventurer toRemove = removable[randomIndex];
            
        adventurers.Remove(toRemove);
        if (kill) toRemove.transform.parent = graveyard.transform; //I REALLY hope we make use of this at some point
        else Destroy(toRemove);
        Achievements.SetCitySize(TotalAdventurers);
        return true;
    }

    public bool RemoveAdventurer(string adventurerName, bool kill) // Deletes an adventurer by name
    {
        Adventurer toRemove = GameObject.Find(adventurerName)?.GetComponent<Adventurer>();
        if (!toRemove) return false;
        adventurers.Remove(toRemove);
        if (kill) toRemove.transform.parent = graveyard.transform;
        else Destroy(toRemove);
        Achievements.SetCitySize(AvailableAdventurers);
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
        modifiers.Add(Metric.Defense, 0);
        modifiers.Add(Metric.Threat, 0);
        modifiers.Add(Metric.Spending, 0);
        modifiers.Add(Metric.Effectiveness, 0);
        modifiers.Add(Metric.Satisfaction, 0);

        // Start game with 5 Adventurers
        for (int i = 0; i < 5; i++) AddAdventurer();

        threatLevel = 30;
        wealth = 50;
        BuildGuildHall();
        
        eventQueue.AddEvent(openingEvent, true);
        
        // Run the tutorial video
        TutorialPlayerController.Instance.PlayClip(0);
        Analytics.EnableCustomEvent("New Turn", true);
        Analytics.enabled = true;
    }

    public int turnCounter = 0;
    [Button("Next Turn")]
    public void NextTurn()
    {
        OnNextTurn?.Invoke();
        //NewTurn();
    }

    public void NewTurn()
    {
        placedThisTurn = 0;
        threatLevel += ChangePerTurn;
        if (threatLevel < 0) threatLevel = 0;
        wealth += wealthPerTurn;
        turnCounter++;

        modifiers[Metric.Defense] = 0;
        modifiers[Metric.Threat] = 0;
        modifiers[Metric.Spending] = 0;
        modifiers[Metric.Effectiveness] = 0;
        modifiers[Metric.Satisfaction] = 0;
        
        if (ThreatLevel >= 100)
            foreach (var e in supportWithdrawnEvents) eventQueue.AddEvent(e, true);
        eventQueue.ProcessEvents();
        
        OnNewTurn?.Invoke();
        if (turnCounter % 5 == 0)
        {
            var ev = Analytics.CustomEvent("Turn Counter", new Dictionary<string, object>
            {
                { "turn_number", turnCounter }
            });
        }
        EnterMenu();
        UpdateUi();
    }

    private int placedThisTurn = 0;
    public void Build(BuildingStats building)
    {
        buildings.Add(building);

        if(++placedThisTurn >= 5) Achievements.Unlock("I'm Saving Up!");
        if (buildings.Count >= 30 && Clear.ClearCount == 0) Achievements.Unlock("One With Nature");
        
        var analyticEvent = Analytics.CustomEvent("Building Built", new Dictionary<string, object>
        {
            {"building_type", building.name },
        });
        Debug.Log(analyticEvent);
        UpdateUi();
    }

    public void Demolish(BuildingStats building)
    {
        if (building.type == BuildingType.GuildHall)
        {
            //TODO: Add an 'are you sure?' dialogue
            Achievements.Unlock("Now Why Would You Do That?");
            foreach (var e in guildHallDestroyedEvents) eventQueue.AddEvent(e, true);
            NextTurn();
        }
        
        map.Clear(building.GetComponent<BuildingStructure>());
        if (!building.terrain) buildings.Remove(building);
        //Destroy(building.gameObject);
        UpdateUi();
    }
    
    public void UpdateUi()
    {
        if (AvailableAdventurers >= 20 && Effectiveness == 100) Achievements.Unlock("Top of Their Game");
        if (AvailableAdventurers >= 20 && Satisfaction == 100) Achievements.Unlock("A Jolly Good Show");
        
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

    public bool inMenu = false;
    public void EnterMenu()
    {
        inMenu = true;
        placementController.GetComponent<ToggleGroup>().SetAllTogglesOff();
    }
    
    public void ExitMenu()
    {
        inMenu = false;
    }

    [HorizontalLine()] 
    
    public Map map;
    public EventQueue eventQueue;
    public NewspaperController newspaperController;
    public MenuManager menuManager;
    public PlacementController placementController;

    public GameObject adventurersContainer, buildingsContainer, graveyard;
    public GameObject adventurerPrefab;
    public GameObject guildHall;
    public Event openingEvent;
    public Event[] guildHallDestroyedEvents;
    public Event[] supportWithdrawnEvents;

    
    private void BuildGuildHall()
    {
        //Build Guild Hall in the center of the map
        map.CreateBuilding(guildHall, map.transform.position, animate: true);
    }

    private void OnDestroy()
    {
        instance = null;
        OnNewTurn = null;
        OnNextTurn = null;
        OnUpdateUI = null;
    }
}
