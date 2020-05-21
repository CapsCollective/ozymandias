﻿using System;
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
    Defense
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

    [ReadOnly] [SerializeField] private List<Adventurer> adventurers = new List<Adventurer>();

    [ReadOnly] [SerializeField] public List<BuildingStats> buildings = new List<BuildingStats>();

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
        get { return availableAdventurers = adventurers.Count(x => x.assignedQuest == null) + AdventurersMod; }
    }

    [ReadOnly] [SerializeField] private int accommodation;

    public int Accommodation
    {
        get { return accommodation = buildings.Where(x => x.operational).Sum(x => x.accommodation); }
    }

    [ReadOnly] [SerializeField] private int satisfaction;

    public int Satisfaction
    {
        get { return satisfaction = buildings.Where(x => x.operational).Sum(x => x.satisfaction) + SatisfactionMod; }
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
                             buildings.Where(x => x.operational).Sum(x => x.defense) + DefenseMod;
        }
    }

    [ReadOnly] [SerializeField] private int chaos;

    public int Chaos
    {
        get { return chaos = AvailableAdventurers / (Satisfaction + 1) + ChaosMod; }
    }

    [ReadOnly] [SerializeField] private int wealthPerTurn;

    public int WealthPerTurn
    {
        get { return wealthPerTurn = Spending * AvailableAdventurers; }
    }

    [SerializeField] private int threat;

    public int Threat
    {
        get { return threat + ThreatMod; }
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
            UpdateUi();
            return true;
        }
        return false;
    }
    

    [ReadOnly]
    public int AdventurersMod, ChaosMod, DefenseMod, ThreatMod, SatisfactionMod;

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
    
    public void RemoveAdventurer() //Removes a random adventurer, ensuring they aren't special
    {
        Adventurer toRemove = adventurers[Random.Range(0, adventurers.Count)];
        if (toRemove.isSpecial)
        {
            RemoveAdventurer(); // Try again!
        }
        else
        {
            adventurers.Remove(toRemove);
            toRemove.transform.parent = GameObject.Find("Graveyard").transform; //I REALLY hope we make use of this at some point
        }
    }
    public void RemoveAdventurer(string adventurerName) // Deletes an adventurer by name
    {
        Adventurer toRemove = GameObject.Find(adventurerName)?.GetComponent<Adventurer>();
        if (!toRemove) return;
        adventurers.Remove(toRemove);
        toRemove.transform.parent = GameObject.Find("Graveyard").transform;
    }
    
    
    [Button("StartGame")]
    public void StartGame()
    {
        // Clear out all adventurers and buildings
        foreach (Transform child in GameObject.Find("Adventurers").transform) Destroy(child.gameObject);
        foreach (Transform child in GameObject.Find("Buildings").transform) Destroy(child.gameObject);
        adventurers = new List<Adventurer>();
        buildings = new List<BuildingStats>();

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

        ChaosMod = 0;
        AdventurersMod = 0;
        DefenseMod = 0;
        SatisfactionMod = 0;

        OnNewTurn?.Invoke();
        UpdateUi();
    }

    public void Build(BuildingStats building)
    {
        CurrentWealth -= building.baseCost;
        buildings.Add(building);
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
