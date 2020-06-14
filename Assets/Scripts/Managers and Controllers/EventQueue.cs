using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using System;
using System.Linq;
using Random = UnityEngine.Random;
using EventType = Event.EventType;
using System.Runtime.InteropServices;

public class EventQueue : MonoBehaviour
{
    private const int MinQueueEvents = 2; // The minimum events in the queue to store
    private const int MinPoolEvents = 2; // The minumum events in the pool to 
    
    [ReorderableList]
    public LinkedList<Event> headliners = new LinkedList<Event>();
    public LinkedList<Event> others = new LinkedList<Event>();

    public Dictionary<EventType, LinkedList<Event>> eventPools = new Dictionary<EventType, LinkedList<Event>>(); //Events to randomly add to the queue
    
    [ReadOnly] public List<Event> current = new List<Event>(4);
    [ReadOnly] public List<string> outcomeDescriptions = new List<string>(4);
    
    public static Action<List<Event>, List<string>> OnEventsProcessed;
    
    public List<Event> allEvents;

    [Tooltip("Events in this array are ignored by the Add All button")]
    public Event[] filterEvents;
    
    public void Awake()
    {
        Random.InitState((int)DateTime.Now.Ticks);

        //TODO: Figure out best way to autoload events
        
        //allEvents = Resources.LoadAll<Event>("Events");
        //Init shuffled event pools pool
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
            if (others.Count < MinQueueEvents) {AddEvent(PickRandom()); continue;}
            current.Add(others.First.Value);
            others.RemoveFirst();
        }
        
        current.Add(PickRandom(EventType.Advert));

        foreach (Event e in current) outcomeDescriptions.Add(e.Execute());
        
        OnEventsProcessed?.Invoke(current, outcomeDescriptions);
    }

    public Event PickRandom()
    {
        EventType type;

        int adventurerW = GameManager.Manager.Satisfaction < 50 ? 13 : 33;
        if (GameManager.Manager.AvailableAdventurers < GameManager.Manager.Accommodation * 0.75f)
            adventurerW += 13;
        int chaosW = GameManager.Manager.Satisfaction < 50 ? 33 : 13;
        //int flavourW = 99 - chaosW - adventurerW; Don't think I need this
        int i = Random.Range(0, 100);


        if (i <= adventurerW)
            type = EventType.AdventurersJoin;
        else if (i <= adventurerW + chaosW)
            type = EventType.Chaos;
        else
            type = EventType.Flavour;

        // Uncomment these to see how the weights work.
        //Debug.Log($"{adventurerW} | {chaosW} | {flavourW}");
        //Debug.Log($"{i} | {type}");
        return PickRandom(type);
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
