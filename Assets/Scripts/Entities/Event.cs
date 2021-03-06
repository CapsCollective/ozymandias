﻿using System.Collections.Generic;
using Entities.Outcomes;
using NaughtyAttributes;
using UnityEngine;
using static Managers.GameManager;
using EventType = Utilities.EventType;

namespace Entities
{
    [CreateAssetMenu(fileName = "Event")][System.Serializable]
    public class Event : ScriptableObject
    {
        public string headline;
        [TextArea(3,8)] public string article;
        public Sprite image;
    
        public EventType type;
        public bool headliner; // If the event should be the main one, should be true for all events with choices

        [Tooltip("These are the outcomes that automatically run on an event")]
        public List<Outcome> outcomes = new List<Outcome>();
    
        [Tooltip("Up to 4 choices, with their own outcomes")]
        public List<Choice> choices = new List<Choice>();

        public bool oneTime;
    
        public string Execute() // Run the event's and return the outcome's description
        {
            return Outcome.Execute(outcomes);
        }

        public string MakeChoice(int choice)
        {
            return Outcome.Execute(choices[choice].outcomes, true);
        }

        [Button()] // Debug to test specific events
        public void AddToQueue()
        {
            Manager.EventQueue.Add(this, true);
        }
    
    }
}
