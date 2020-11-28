using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;
using System.Threading.Tasks;
using Managers_and_Controllers;
using NaughtyAttributes;
using Newtonsoft.Json;
using UnityEngine.AddressableAssets;
using UnityEngine.Analytics;
using UnityEngine.UI;
using static AchievementManager;
using static QuestMapController;
using static BuildingManager;
using Random = UnityEngine.Random;

public enum Metric
{
    Accommodation,
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

    [Serializable]
    public class Modifier
    {
        public int amount;
        public int turnsLeft;
        public string reason;
    }
    
    [Serializable]
    public class SaveFile
    {
        [SerializeField] private int wealth, turnCounter, threatLevel, clearCount;
        [SerializeField] private List<string> buildings, adventurers, quests, unlockedBuildings;
        [SerializeField] private string modifiers;
        
        public string Save()
        {
            wealth = Manager.wealth;
            turnCounter = Manager.turnCounter;
            threatLevel = Manager.threatLevel;
            clearCount = Clear.ClearCount;
            buildings = new List<string>();
            
            foreach (var building in Manager.buildings)
                buildings.Add(building.Serialize());
            
            foreach (var terrain in Manager.terrain)
                buildings.Add(terrain.Serialize());
            
            adventurers = Manager.adventurers.Select(a => a.Serialize()).ToList();
            
            quests = QuestMap.Quests.Select(q => q.Serialize()).ToList();
            
            unlockedBuildings = BuildManager.unlockedBuildings.Select(x => x.name).ToList();
            
            modifiers = JsonConvert.SerializeObject(Manager.modifiers);
            
            return JsonUtility.ToJson(this);
        }

        public async Task Load()
        {
            Manager.wealth = wealth;
            Manager.turnCounter = turnCounter;
            Manager.threatLevel = threatLevel;
            Clear.ClearCount = clearCount;
            if (turnCounter == 0)
                Manager.StartGame();
            foreach (string adventurer in adventurers)
                Manager.AddAdventurer(adventurer);

            foreach (string building in buildings)
            {
                string[] details = building.Split(',');
                Vector3 worldPosition = new Vector3(float.Parse(details[1]), 0, float.Parse(details[2]));
                GameObject buildingInstance = await Addressables.InstantiateAsync(details[0], GameObject.Find("Buildings").transform).Task;
                Manager.map.CreateBuilding(buildingInstance, worldPosition, int.Parse(details[3]));
            }

            foreach (string quest in quests)
            {
                Quest.QuestDetails details = Quest.Deserialize(quest);
                Quest q = await Addressables.LoadAssetAsync<Quest>(details.name).Task;
                QuestMap.AddQuest(q);
                q.cost = details.cost;
                q.turnsLeft = details.turnsLeft;
                if (details.assigned.Count == 0) continue;
                q.assigned = details.assigned.Select(name => Manager.adventurers.Find(a => a.name == name)).ToList();
                q.assigned.ForEach(a => a.assignedQuest = q);
                q.ResumeQuest();
            }

            JsonConvert.PopulateObject(modifiers, Manager.modifiers);
            foreach (var metricPair in Manager.modifiers)
                Manager.modifiersTotal[metricPair.Key] = metricPair.Value.Sum(x => x.amount);

            unlockedBuildings.ForEach(async b => 
                BuildManager.unlockedBuildings.Add(await Addressables.LoadAssetAsync<GameObject>(b).Task));
            
            //TODO: Reshuffle buildings
            
            //TODO: Modifiers, Queued Events, One time events
        }
    }
    
    public void Save()
    {
        string saveJson = new SaveFile().Save();
        PlayerPrefs.SetString("save", saveJson);
        Debug.Log(saveJson);
    }

    private bool loading = false;
    public async void Load()
    {
        loading = true;
        //TODO: Do we want to save this to a file instead of playerprefs?
        string saveJson = PlayerPrefs.GetString("save", File.ReadAllText(Application.streamingAssetsPath + "/StartingLayout.json"));
        await JsonUtility.FromJson<SaveFile>(saveJson).Load();
        Debug.Log(saveJson);
        loading = false;
        UpdateUi();
    }

    public Dictionary<Metric, List<Modifier>> modifiers = new Dictionary<Metric, List<Modifier>>();
    public Dictionary<Metric, int> modifiersTotal = new Dictionary<Metric, int>();
    
    [ReadOnly] public List<Adventurer> adventurers = new List<Adventurer>();

    [ReadOnly] public List<BuildingStats> buildings = new List<BuildingStats>();
    [HideInInspector] public List<BuildingStats> terrain = new List<BuildingStats>(); // Hidden in inspector because it's big
    
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

    //[ReadOnly] [SerializeField] private int training;
    //public int Training => training = 0; // TODO: work out the specifics of this

    [ReadOnly] [SerializeField] private int effectiveness;
    public int Effectiveness => effectiveness = Mathf.Clamp(1 + Equipment/3 + Weaponry/3 + Magic/3  + LowThreatMod + modifiersTotal[Metric.Effectiveness], 0, 100);
    
    [HorizontalLine]
    
    [ReadOnly] [SerializeField] private int food;
    public int Food => food = Mathf.Clamp(100 * buildings.Where(x => x.operational).Sum(x => x.food) / AvailableAdventurers, 0, 100);

    [ReadOnly] [SerializeField] private int entertainment;
    public int Entertainment => entertainment = Mathf.Clamp(100 * buildings.Where(x => x.operational).Sum(x => x.entertainment) / AvailableAdventurers, 0, 100);
    
    [ReadOnly] [SerializeField] private int luxury;
    public int Luxury => luxury = Mathf.Clamp(100 * buildings.Where(x => x.operational).Sum(x => x.luxury) / AvailableAdventurers, 0, 100);

    public int OvercrowdingMod => Mathf.Min(0, (Accommodation - AvailableAdventurers) * 5); //lose 5% satisfaction per adventurer over capacity
    
    public int LowThreatMod => Mathf.Min(0, (ThreatLevel - 20) * 2); //lose up to 40% effectiveness from low threat
    
    [ReadOnly] [SerializeField] private int satisfaction;
    public int Satisfaction => satisfaction = Mathf.Clamp(1 + Food/3 + Entertainment/3 + Luxury/3 + OvercrowdingMod + modifiersTotal[Metric.Satisfaction], 0, 100);

    [HorizontalLine]
    
    [ReadOnly] [SerializeField] private int spending;
    public int Spending => spending = 100 + buildings.Where(x => x.operational).Sum(x => x.spending) + modifiersTotal[Metric.Spending];
    
    [ReadOnly] [SerializeField] private int chaos;
    public int Chaos => chaos = AvailableAdventurers * (100 - Satisfaction);

    [ReadOnly] [SerializeField] private int wealthPerTurn;
    public int WealthPerTurn => wealthPerTurn = Spending * AvailableAdventurers / 10; //10 gold per adventurer times spending

    [SerializeField] private int wealth;
    public int Wealth => wealth;
    
    [ReadOnly] [SerializeField] private int defense;
    public int Defense => defense =
        AvailableAdventurers * Effectiveness / 30 +
        buildings.Where(x => x.operational).Sum(x => x.defense) +
        modifiersTotal[Metric.Defense];
    
    [ReadOnly] [SerializeField] private int threat;
    public int Threat => threat = 12 + (3 * turnCounter) + modifiersTotal[Metric.Threat];

    public int ChangePerTurn => Threat - Defense; // How much the top bar shifts each turn
    
    [SerializeField] private int threatLevel;
    public int ThreatLevel => threatLevel; // Percentage of how far along the threat is.

    public bool Spend(int amount)
    {
        if (loading) return true; //Don't spend money when loading
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

    public void AddAdventurer(string s)
    {
        string[] details = s.Split(',');
        Adventurer adventurer = Instantiate(adventurerPrefab, GameObject.Find("Adventurers").transform)
            .GetComponent<Adventurer>();
        adventurer.name = details[0];
        adventurer.category = (AdventurerCategory)int.Parse(details[1]);
        adventurer.isSpecial = bool.Parse(details[2]);
        adventurer.turnJoined = int.Parse(details[3]);
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

        // Start game with 5 Adventurers
        for (int i = 0; i < 5; i++) AddAdventurer();

        threatLevel = 40;
        wealth = 50;
        
        eventQueue.AddEvent(openingEvent, true);
        
        // Run the tutorial video
        if (PlayerPrefs.GetInt("tutorial_video_basics", 0) == 0)
        {
            PlayerPrefs.SetInt("tutorial_video_basics", 1);
            TutorialPlayerController.Instance.PlayClip(0);
        }

        Analytics.EnableCustomEvent("New Turn", true);
        Analytics.enabled = true;
    }

    public int turnCounter = 0;
    [Button("Next Turn")]
    public void NextTurn()
    {
        OnNextTurn?.Invoke();
    }

    public void NewTurn()
    {
        placedThisTurn = 0;
        threatLevel += ChangePerTurn;
        if (threatLevel < 0) threatLevel = 0;
        wealth += wealthPerTurn;
        turnCounter++;

        if (ThreatLevel >= 100)
            foreach (var e in supportWithdrawnEvents) eventQueue.AddEvent(e, true);
        
        foreach (var stat in modifiers)
        {
            for (int i = stat.Value.Count-1; i >= 0; i--)
            {
                // -1 means infinite modifier
                if (modifiers[stat.Key][i].turnsLeft == -1 || --modifiers[stat.Key][i].turnsLeft > 0) continue;
                Debug.Log("Removing Stat: " + stat.Key);
                modifiersTotal[stat.Key] -= modifiers[stat.Key][i].amount;
                modifiers[stat.Key].RemoveAt(i);
            }
        }

        eventQueue.ProcessEvents();
        
        OnNewTurn?.Invoke();
        //if (turnCounter % 5 == 0)
        //{
        //    var ev = Analytics.CustomEvent("Turn Counter", new Dictionary<string, object>
        //    {
        //        { "turn_number", turnCounter }
        //    });
        //}

        Save();
        EnterMenu();
        UpdateUi();
    }

    private int placedThisTurn = 0;
    public void Build(BuildingStats building)
    {
        if (building.terrain) terrain.Add(building);
        else buildings.Add(building);

        if(!loading && ++placedThisTurn >= 5) Achievements.Unlock("I'm Saving Up!");
        if (buildings.Count >= 30 && Clear.ClearCount == 0) Achievements.Unlock("One With Nature");
        
        //var analyticEvent = Analytics.CustomEvent("Building Built", new Dictionary<string, object>
        //{
        //    {"building_type", building.name},
        //});
        if(!loading) UpdateUi();
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
        if (building.terrain) terrain.Remove(building);
        else buildings.Remove(building);
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
        // Set all mods to 0 at start
        modifiersTotal.Add(Metric.Defense, 0);
        modifiersTotal.Add(Metric.Threat, 0);
        modifiersTotal.Add(Metric.Spending, 0);
        modifiersTotal.Add(Metric.Effectiveness, 0);
        modifiersTotal.Add(Metric.Satisfaction, 0);

        modifiers.Add(Metric.Defense, new List<Modifier>());
        modifiers.Add(Metric.Threat, new List<Modifier>());
        modifiers.Add(Metric.Spending, new List<Modifier>());
        modifiers.Add(Metric.Effectiveness, new List<Modifier>());
        modifiers.Add(Metric.Satisfaction, new List<Modifier>());

        Load();
    }

    public int GetMetric(Metric metric)
    {
        switch (metric)
        {
            case Metric.Accommodation: return Accommodation;
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
        placementManager.GetComponent<ToggleGroup>().SetAllTogglesOff();
    }
    
    public void ExitMenu()
    {
        inMenu = false;
    }

    [HorizontalLine]
    
    public Map map;
    public EventQueue eventQueue;
    public NewspaperController newspaperController;
    public MenuManager menuManager;
    public PlacementManager placementManager;

    public GameObject adventurersContainer, buildingsContainer, graveyard;
    public GameObject adventurerPrefab;
    public GameObject guildHall;
    public Event openingEvent;
    public Event[] guildHallDestroyedEvents;
    public Event[] supportWithdrawnEvents;
    
    private void OnDestroy()
    {
        instance = null;
        OnNewTurn = null;
        OnNextTurn = null;
        OnUpdateUI = null;
    }
}
