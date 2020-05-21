using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;

[CreateAssetMenu]
public class NewAdventurers : Outcome
{
    public List<AdventurerDetails> adventurers;
    
    public override bool Execute()
    {
        for (int i = 0; i < adventurers.Count; i++)
        {
            if (adventurers[i] != null) Manager.AddAdventurer(adventurers[i]);
            else Manager.AddAdventurer();
        }
        return true;
    }
}