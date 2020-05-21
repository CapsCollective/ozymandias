using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using System;
using UnityEngine.Analytics;

public class EventQueue : MonoBehaviour
{
    [SerializeField] private Outcome defaultOutcome;
    [ReorderableList]
    public List<Event> EventsQueue = new List<Event>();

    [ReorderableList]
    public List<Event> CurrentEvents = new List<Event>(3);
    
    [ReorderableList]
    public List<IPersistentEvent> PersistentEvents = new List<IPersistentEvent>();

    public static Action<List<Event>> OnEventsProcessed;
    public static Action<StatChange> OnNewPersistentEvent;
    //public static Action<StatChange> OnPersistentEventComplete;

    public static string outcomeString;

    public void Awake()
    {
        GameManager.OnNewTurn += ProcessEvents;
        NewspaperController.OnOutcomeSelected += HandleChainEvent;
    }

    private void ProcessEvents()
    {
        outcomeString = "";

        CurrentEvents.Clear();

        bool hasChoices = false;    

        for (int e = 0; e < CurrentEvents.Capacity; e++)
        {
            for (int i = 0; i < EventsQueue.Count; i++)
            {
                // If the event has choices
                if (EventsQueue[i].Choices.Count > 0)
                {
                    if (!hasChoices)
                    {
                        hasChoices = true;
                        CurrentEvents.Insert(0, EventsQueue[i]);
                        EventsQueue.RemoveAt(i);
                        break;
                    }
                    else
                        continue;
                }

                // If there is a default outcome
                // We execute that
                bool execute = false;
                if (EventsQueue[i].EventOutcomes.Count > 0)
                {
                    foreach (Outcome o in EventsQueue[i].EventOutcomes)
                    {
                        execute = o.Execute();
                        if (execute)
                        {
                            Debug.Log(o.GetOutcomeString());
                            outcomeString += o.GetOutcomeString() + "\n";
                            execute = false;
                        }
                        else
                            break;
                    }
                    CurrentEvents.Add(EventsQueue[i]);
                    EventsQueue.RemoveAt(i);
                    Debug.Log(outcomeString);
                    break;
                }

                CurrentEvents.Add(EventsQueue[i]);
                EventsQueue.RemoveAt(i);
                break;
            }
            if (CurrentEvents.Count == 3)
            {
                hasChoices = false;
                break;
            }
        }
        OnEventsProcessed?.Invoke(CurrentEvents);
    }

    public void HandleChainEvent(Outcome outcome)
    {
        //if(outcome.ChainEvent != null)
        //    EventsQueue.RandomInsert(outcome.ChainEvent, outcome.ChainEventMaxTurnsAway * 3);
    }
}
