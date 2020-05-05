using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using System;

public class EventQueue : MonoBehaviour
{
    [ReorderableList]
    public List<Event> EventsQueue = new List<Event>();

    [ReorderableList]
    public List<Event> CurrentEvents = new List<Event>(3);

    public static Action<List<Event>> OnEventsProcessed;

    public void Awake()
    {
        GameManager.OnNewTurn += ProcessEvents;
    }

    private void ProcessEvents()
    {
        CurrentEvents.Clear();
        for (int e = 0; e < CurrentEvents.Capacity; e++)
        {
            for (int i = 0; i < EventsQueue.Count; i++)
            {
                bool execute = false;
                execute = EventsQueue[i].defaultOutcome.Execute();
                // Find a new event from the queue
                if (execute)
                {
                    CurrentEvents.Add(EventsQueue[i]);
                    EventsQueue.RemoveAt(i);
                    break;
                }
            }
            if (CurrentEvents.Count == 3)
                break;
        }
        OnEventsProcessed?.Invoke(CurrentEvents);
    }
}
