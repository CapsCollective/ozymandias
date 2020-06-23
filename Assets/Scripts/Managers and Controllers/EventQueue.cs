using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using System;
using System.Linq;
using Random = UnityEngine.Random;
using EventType = Event.EventType;
using static GameManager;

public class EventQueue : MonoBehaviour
{
    private const int MinQueueEvents = 3; // The minimum events in the queue to store
    private const int MinPoolEvents = 2; // The minumum events in the pool to 
    
    [ReorderableList]
    public LinkedList<Event> headliners = new LinkedList<Event>();
    public LinkedList<Event> others = new LinkedList<Event>();

    public Dictionary<EventType, LinkedList<Event>> eventPools = new Dictionary<EventType, LinkedList<Event>>(); //Events to randomly add to the queue
    
    [ReadOnly] public List<Event> current = new List<Event>(4);
    [ReadOnly] public List<string> outcomeDescriptions = new List<string>(4);
    
    public static Action<List<Event>, List<string>> OnEventsProcessed;
    
    public List<Event> allEvents;

    private int nextBuildingUnlock = 10;

    [Tooltip("Events in this array are ignored by the Add All button")]
    public Event[] filterEvents;
    
    public void Awake()
    {
        Random.InitState((int)DateTime.Now.Ticks);

        foreach (EventType type in Enum.GetValues(typeof(EventType))) eventPools.Add(type, Shuffle(type));
    }

    public void ProcessEvents()
    {
        current.Clear();
        outcomeDescriptions.Clear();
        
        if (headliners.Count > 0)
        {
            current.Add(headliners.First.Value);
            headliners.RemoveFirst();
        }

        while (current.Count < 3)
        {
            if (others.Count < MinQueueEvents) { AddRandomSelection(); continue; }
            current.Add(others.First.Value);
            others.RemoveFirst();
        }
        
        current.Add(PickRandom(EventType.Advert));

        foreach (Event e in current) outcomeDescriptions.Add(e.Execute());
        
        OnEventsProcessed?.Invoke(current, outcomeDescriptions);
    }

    public void AddRandomSelection()
    {
        List<Event> eventPool = new List<Event>();
        
        for (int j = 0; j < 3; j++) eventPool.Add(PickRandom(EventType.Flavour)); //Baseline of 3 flavour events
        
        
        if (Manager.TotalAdventurers >= nextBuildingUnlock) {
            eventPool.Add(PickRandom(EventType.Blueprint)); // Spawn every 10 adventurers
            nextBuildingUnlock += 10;
        }

        // Keeps adventurer count roughly at a fair level
        if (Manager.TotalAdventurers < 7 + Manager.turnCounter && Manager.Satisfaction > 70) eventPool.Add(PickRandom(EventType.AdventurersJoin));
        // Catchup if falling behind
        if (Manager.TotalAdventurers < 4 + Manager.turnCounter && Manager.Satisfaction > 50) eventPool.Add(PickRandom(EventType.AdventurersJoin));
        if (Manager.TotalAdventurers < Manager.turnCounter) eventPool.Add(PickRandom(EventType.AdventurersJoin));
        // More if high satisfaction
        if (Manager.Satisfaction > 80) eventPool.Add(PickRandom(EventType.AdventurersJoin));
        
        if (Manager.turnCounter >= 5)
        {
            // Fill up to 5 quests if unfilled
            if(Random.Range(0,5) > QuestMapController.QuestMap.ActiveQuests) eventPool.Add(PickRandom(EventType.Radiant));
            
            // 30% flat chance to spawn chaos
            if (Random.Range(0,100) < 30) eventPool.Add(PickRandom(EventType.Chaos));
            //Variable rate for < 50
            if (Manager.ThreatLevel - Random.Range(0,50) < 0) eventPool.Add(PickRandom(EventType.Chaos));
            
            // Start spawning threat events at 50, and gets more likely the higher it gets
            if (Random.Range(50,100) - Manager.ThreatLevel < 0) eventPool.Add(PickRandom(EventType.Threat));
            
            // Fixed 10% spawn rate for challenge
            if (Random.Range(0,100) < 10) eventPool.Add(PickRandom(EventType.AdventurersLeave));
            // Variable rate for < 50 and 30, should cause a mass exodus
            if (Manager.Satisfaction - Random.Range(10,50) < 0) eventPool.Add(PickRandom(EventType.AdventurersLeave));
            if (Manager.Satisfaction - Random.Range(10,30) < 0) eventPool.Add(PickRandom(EventType.AdventurersLeave));
        }

        while (eventPool.Count > 0) AddEvent(eventPool.PopRandom()); // Add events in random order
    }

    public Event PickRandom(EventType type)
    {
        while (true) // Repeats until valid event is found
        {
            if (eventPools[type].Count == 0) eventPools[type] = Shuffle(type);
            
            Event e = eventPools[type].First.Value;
            eventPools[type].RemoveFirst();
            if (ValidEvent(e))
            {
                if (e.oneTime) allEvents.Remove(e);
                return e;
            }
        }
    }

    public void AddEvent(Event e, bool toFront = false)
    {
        if (e.headliner || e.choices.Count > 0)
        {
            if (toFront) headliners.AddFirst(e);
            else headliners.AddLast(e);
        }
        else
        {
            if (toFront) others.AddFirst(e);
            else others.AddLast(e);
        }
    }

    public LinkedList<Event> Shuffle(EventType type) // Returns a shuffled list of all events of a certain type
    {
        List<Event> events = allEvents.Where(x => x.type == type).ToList();
        LinkedList<Event> shuffled = new LinkedList<Event>();
        while (events.Count != 0)
        {
            shuffled.AddLast(events.PopRandom());
        }
        return shuffled;
    }

    public bool ValidEvent(Event e)
    {
        //TODO: Implement
        return true;
    }

    private void OnDestroy()
    {
        OnEventsProcessed = null;
    }
}

public static class MyExtensions
{
    public static T PopRandom<T>(this List<T> list)
    {
        int i = Random.Range(0, list.Count);
        T value = list[i];
        list.RemoveAt(i);
        return value;
    }
}
