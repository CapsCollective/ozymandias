using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;

[CreateAssetMenu]
public class GameOver : Outcome
{
    public override bool Execute()
    {
        Manager.GameOver();
        return true;
    }
}