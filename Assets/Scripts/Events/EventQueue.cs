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
    public List<StatChange> StatEffects = new List<StatChange>();

    public static Action<List<Event>> OnEventsProcessed;
    public static Action<StatChange> OnNewStatEffect;
    public static Action<StatChange> OnStatEffectComplete;

    public static string outcomeString;

    public void Awake()
    {
        OnNewStatEffect += HandleNewStatEffect;
        OnStatEffectComplete += RemoveStatEffect;
        GameManager.OnNewTurn += ProcessEvents;
        NewspaperController.OnOutcomeSelected += HandleChainEvent;
    }

    private void ProcessEvents()
    {
        outcomeString = "";

        CurrentEvents.Clear();

        for (int i = 0; i < StatEffects.Count; i++)
        {
            StatEffects[i].ProcessStatChange();
        }

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
                if (EventsQueue[i].eventOutcomes.Count > 0)
                {
                    foreach (Outcome o in EventsQueue[i].eventOutcomes)
                    {
                        execute = o.Execute();
                        if (execute)
                        {
                            CurrentEvents.Add(EventsQueue[i]);
                            EventsQueue.RemoveAt(i);
                            Debug.Log(o.GetOutcomeString());
                            outcomeString += $"\n" + o.GetOutcomeString();
                            execute = false;
                        }
                    }
                    Debug.Log(outcomeString);
                    break;
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

    private void HandleNewStatEffect(StatChange statEffect)
    {
        Debug.Log("Testing?");
        StatChange sc = Instantiate(statEffect) as StatChange;
        sc.ProcessStatChange();
        StatEffects.Add(sc);
    }

    private void RemoveStatEffect(StatChange statEffect)
    {
        StatEffects.Remove(statEffect);
    }

    public void HandleChainEvent(Outcome outcome)
    {
        //if(outcome.ChainEvent != null)
        //    EventsQueue.RandomInsert(outcome.ChainEvent, outcome.ChainEventMaxTurnsAway * 3);
    }
}
