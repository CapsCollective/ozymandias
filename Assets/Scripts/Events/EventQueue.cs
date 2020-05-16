using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using System;

public class EventQueue : MonoBehaviour
{
    [SerializeField] private Outcome defaultOutcome;
    [ReorderableList]
    public List<Event> EventsQueue = new List<Event>();

    [ReorderableList]
    public List<Event> CurrentEvents = new List<Event>(3);

    public static Action<List<Event>> OnEventsProcessed;

    public void Awake()
    {
        GameManager.OnNewTurn += ProcessEvents;
        NewspaperController.OnOutcomeSelected += HandleChainEvent;
    }

    private void ProcessEvents()
    {
        CurrentEvents.Clear();
        bool hasChoices = false;    

        for (int e = 0; e < CurrentEvents.Capacity; e++)
        {
            for (int i = 0; i < EventsQueue.Count; i++)
            {
                if (EventsQueue[i].defaultOutcome == null)
                    EventsQueue[i].defaultOutcome = defaultOutcome;

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
                if (EventsQueue[i].defaultOutcome != null)
                {
                    execute = EventsQueue[i].defaultOutcome.Execute();
                    if (execute)
                    {
                        CurrentEvents.Add(EventsQueue[i]);
                        EventsQueue.RemoveAt(i);
                        break;
                    }

                    if (EventsQueue[i].defaultOutcome.ChainEvent != null)
                        HandleChainEvent(EventsQueue[i].defaultOutcome);
                }
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
        EventsQueue.RandomInsert(outcome.ChainEvent, outcome.ChainEventMaxTurnsAway * 3);
    }
}
