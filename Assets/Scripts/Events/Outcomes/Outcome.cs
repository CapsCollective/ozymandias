using System.Collections.Generic;
using UnityEngine;
using Utilities;

namespace Events.Outcomes
{
    public class Outcome : ScriptableObject
    {
        [TextArea]
        public string customDescription; //An override description for custom outcomes

        protected virtual string Description => customDescription;

        // What happens when this is executed
        protected virtual bool Execute()
        {
            return false;
        }
    
        public virtual bool Execute(bool fromChoice)
        {
            return false;
        }
    
        public static string Execute(List<Outcome> outcomes)
        {
            string description = "";
        
            foreach (Outcome outcome in outcomes)
            {
                bool res = outcome.Execute();
            
                if (res && outcome.Description != "") description += outcome.Description.ListItem();
            }
            return description.TrimStart('\n');
        }
    }
}
