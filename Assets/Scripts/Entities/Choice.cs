using System.Collections.Generic;
using Entities.Outcomes;
using UnityEngine;

namespace Entities
{
    public class Choice : ScriptableObject
    {
        public List<Outcome> outcomes = new List<Outcome>();
    }
}
