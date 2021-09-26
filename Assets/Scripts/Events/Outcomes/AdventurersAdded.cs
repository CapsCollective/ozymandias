using System.Collections.Generic;
using Adventurers;
using Managers;
using UnityEngine;
using static Managers.GameManager;
using Random = UnityEngine.Random;

namespace Events.Outcomes
{
    [CreateAssetMenu(fileName = "New Adventurers Outcome", menuName = "Outcomes/New Adventurers")]
    public class AdventurersAdded : Outcome
    {
        public List<AdventurerDetails> adventurers;

        public override bool Execute()
        {
            foreach (AdventurerDetails details in adventurers)
            {
                if (details.name != null) Manager.Adventurers.Add(details);
                else Manager.Adventurers.Add();
            }

            return true;
        }

        private static string[] Descriptors = {
            "taken up residence.", "joined the fight!", "found a new home.", "started questing."
        };
    
        public override string Description
        {
            get
            {
                if (customDescription != "") return "<color=#007000ff>" + customDescription + "</color>";
                return "<color=#007000ff>" +
                       adventurers.Count + " adventurer" +
                       (adventurers.Count > 1 ? "s have " : " has ") +
                       Descriptors[Random.Range(0, Descriptors.Length)]+
                       "</color>";
            }
        }
    
    }
}
