using System.Collections.Generic;
using UnityEngine;
using static Managers.GameManager;

namespace Events.Outcomes
{
    [CreateAssetMenu(fileName = "Remove Adventurers Outcome", menuName = "Outcomes/Remove Adventurers")]
    public class AdventurersRemoved : Outcome
    {
        public int count;
        public List<string> adventurerNames;
        // To shreds, you say?
        public bool kill; // If they move to the graveyard or just disappear
        public override bool Execute()
        {
            if (Manager.Adventurers.Available <= count) return false;

            for (int i = 0; i < count; i++)
            {
                if (!Manager.Adventurers.Remove(kill)) return false;
            }
            return true;
        }
    
        public override string Description
        {
            get
            {
                if (customDescription != "") return "<color=#820000ff>" + customDescription + "</color>";
                return "<color=#820000ff>" +
                       adventurerNames.Count +
                       " adventurer" +
                       (adventurerNames.Count > 1 ? "s have " : " has ") +
                       "been lost</color>";
            }
        }
    }
}
