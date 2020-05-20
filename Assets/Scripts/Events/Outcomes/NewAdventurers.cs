using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;

[CreateAssetMenu]
public class NewAdventurers : Outcome
{
    public int count;

    public override bool Execute()
    {
        for (int i = 0; i < count; i++)
        {
            Manager.AddAdventurer();
        }
        return true;
    }
}