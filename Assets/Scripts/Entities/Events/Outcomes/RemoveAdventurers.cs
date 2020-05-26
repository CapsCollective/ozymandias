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
        int failed = 0;
        for (int i = 0; i < adventurerNames.Count; i++)
        {
            if (adventurerNames[i] != "") failed += Manager.RemoveAdventurer(adventurerNames[i], kill) ? 0 : 1;
            else failed += Manager.RemoveAdventurer(kill) ? 0 : 1;
        }
        return failed == 0;
    }
}