using System.Collections.Generic;
using Events.Outcomes;
using UnityEngine;
using Utilities;

namespace Events
{
    public class Choice : ScriptableObject
    {
        public List<Outcome> outcomes = new List<Outcome>();
        public float costScale;
        public bool requiresItem, disableRepurchase;
        public Flag requiredItem;
    }
}
