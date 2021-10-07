using System;
using UnityEngine;
using Utilities;
using Event = Events.Event;

namespace Requests.Templates
{
    public abstract class Request : ScriptableObject
    {
        public Guild guild;
        public Event completedEvent;
        
        [NonSerialized] public int Completed, Required, Tokens;
        public bool IsCompleted => Completed >= Required;
        public abstract string Description { get; }
        protected abstract int RequiredScaled { get; }

        public void Init()
        {
            Tokens = 1; // TODO: Scale reward amount
            Completed = 0;
            Required = RequiredScaled;
        }
        
        public abstract void Start();
        public abstract void Complete();
        
        private void OnDisable()
        {
            Complete(); // Clear whatever setup the object has on clear
        }
    }
}
