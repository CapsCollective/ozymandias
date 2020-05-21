using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;

[CreateAssetMenu]
public class RemoveAdventurers : Outcome
{
    public List<string> adventurerNames;
    
    public override bool Execute()
    {
        for (int i = 0; i < adventurerNames.Count; i++)
        {
            if (adventurerNames[i] != "") Manager.RemoveAdventurer(adventurerNames[i]);
            else Manager.RemoveAdventurer();
        }
        return true;
    }
}