using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "New Adventurers Outcome", menuName = "Outcomes/New Adventurers")]
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

    private static string[] Descriptors = {
        "taken up residence.", "joined the fight!", "found a new home.", "started questing."
    };
    
    public override string Description
    {
        get
        {
            if (customDescription != "") return customDescription;
            Random.InitState((int)DateTime.Now.Ticks);
            return  adventurers.Count + " adventurer" +
                    (adventurers.Count > 1 ? "s have " : " has ") +
                    Descriptors[Random.Range(0, Descriptors.Length)];
        }
    }
    
}