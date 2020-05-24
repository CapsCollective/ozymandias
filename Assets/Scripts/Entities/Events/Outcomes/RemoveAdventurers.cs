using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;

[CreateAssetMenu]
public class RemoveAdventurers : Outcome
{
    public List<string> adventurerNames;
    // To shreds, you say?
    public bool kill; // If they move to the graveyard or just disappear
    public override bool Execute()
    {
        for (int i = 0; i < adventurerNames.Count; i++)
        {
            if (adventurerNames[i] != "") Manager.RemoveAdventurer(adventurerNames[i], kill);
            else Manager.RemoveAdventurer(kill);
        }
        return true;
    }
}