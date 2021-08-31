using System;
using Events;
using UnityEngine;
using Utilities;
using Event = Events.Event;

namespace Requests.Templates
{
    public abstract class Request : ScriptableObject
    {
        public Guild guild;
        public Event completedEvent;
        
        [NonSerialized] public int Completed, Required;
        public bool IsCompleted => Completed >= Required;
        public abstract string Description { get; }
        protected abstract int RequiredScaled { get; }

        public void Init()
        {
            Completed = 0;
            Required = RequiredScaled;
        }
        
        public abstract void Start();
        public abstract void Complete();

        public virtual void Configure(EventCreator.RequestConfig config) {}
    }
}
