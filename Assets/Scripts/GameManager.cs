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
    public GameObject adventurerPrefab;
    
    private static GameManager instance;
    public static GameManager Instance {
        get {
            if (!instance)
                instance = Resources.FindObjectsOfTypeAll<GameManager>().FirstOrDefault();
            return instance;
        }
    }

    [ReadOnly][SerializeField] private List<Adventurer> adventurers = new List<Adventurer>();

    [ReadOnly][SerializeField] private List<Building> buildings = new List<Building>();
    
    /*public List<Building> Buildings
    {
        get
        {
            return GameObject.FindGameObjectsWithTag("Building").Select(x => x.GetComponent<Building>()).ToList();
        }
    }*/
    
    [ReadOnly][SerializeField] private int availableAdventurers;
    public int AvailableAdventurers {
        get { return availableAdventurers = adventurers.Count(x => x.assignedQuest == null); }
    }
    
    [ReadOnly][SerializeField] private int accommodation;
    public int Accommodation
    {
        get { return accommodation = buildings.Sum(x => x.accommodation); }
    }

    [ReadOnly] [SerializeField] private int satisfaction;
    public int Satisfaction {
        get { return satisfaction = buildings.Where(x => x.operational).Sum(x => x.satisfaction); }
    }
    
    [ReadOnly] [SerializeField] private int effectiveness;
    public int Effectiveness {
        get { return effectiveness = buildings.Where(x => x.operational).Sum(x => x.effectiveness); }
    }

    [ReadOnly][SerializeField] private int spending;
    public int Spending {
        get
        { return spending = buildings.Where(x => x.operational).Sum(x => x.spending); }
    }

    [ReadOnly][SerializeField] private int defense;
    public int Defense {
        get { return defense = AvailableAdventurers * Effectiveness + buildings.Where(x => x.operational).Sum(x => x.defense); }
    }

    [ReadOnly][SerializeField] private int chaos;
    public int Chaos {
        get { return chaos = AvailableAdventurers / (Satisfaction+1); }
    }
    
    [ReadOnly][SerializeField] private int wealthPerTurn;
    public int WealthPerTurn {
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
    
    [Button("StartGame")]
    public void StartGame()
    {
        // Clear out all adventurers and buildings
        foreach (Transform child in GameObject.Find("Adventurers").transform) {
            GameObject.Destroy(child.gameObject);
        }
        foreach (Transform child in GameObject.Find("Buildings").transform) {
            GameObject.Destroy(child.gameObject);
        }
        adventurers = new List<Adventurer>();
        buildings = new List<Building>();
        
        CurrentWealth = 10;
        threat = 0;
        BuildGuildHall();
    }
    
    [Button("Next Turn")]
    public void NextTurn()
    {
        if (Accommodation > adventurers.Count())
        {
            adventurers.Add(Instantiate(adventurerPrefab, GameObject.Find("Adventurers").transform).GetComponent<Adventurer>());
        }
        Threat += buildings.Count;
        CurrentWealth = WealthPerTurn;
        UpdateUI();
    }

    public void Build(Building building)
    {
        CurrentWealth -= building.baseCost;
        buildings.Add(building);
        UpdateUI();
    }

    public void UpdateUI()
    {
        topBar.UpdateUI();
        wealthCounter.UpdateUI();
        //Todo: Updates all UI
        Debug.Log(
            "AvailableAdventurers: " + AvailableAdventurers +
            "\nAccommodation: " + Accommodation +
            "\nSatisfaction: " + Satisfaction +
            "\nEffectiveness: " + Effectiveness +
            "\nSpending: " + Spending +
            "\nDefense: " + Defense +
            "\nChaos: " + Chaos +
            "\nThread: " + Threat +
            "\nCurrent Wealth:" + CurrentWealth + "/" + wealthPerTurn);
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
    
    public TopBar topBar;
    public WealthCounter wealthCounter;

    [SerializeField] private GameObject guildHall;
    
    private void BuildGuildHall()
    {
        Instantiate(guildHall, GameObject.Find("Buildings").transform).GetComponent<Building>().Build();
    }
}
