using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;

[CreateAssetMenu(fileName = "Remove Adventurers Outcome", menuName = "Outcomes/Remove Adventurers")]
public class RemoveAdventurers : Outcome
{
    public List<string> adventurerNames;
    // To shreds, you say?
    public bool kill; // If they move to the graveyard or just disappear
    public override bool Execute()
    {
        if (Manager.AvailableAdventurers <= adventurerNames.Count) return false;
        
        for (int i = 0; i < adventurerNames.Count; i++)
        {
            if (adventurerNames[i] != "") {
                if (!Manager.RemoveAdventurer(adventurerNames[i], kill)) return false;
            }
            else if (!Manager.RemoveAdventurer(kill)) return false;
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
