using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;

[CreateAssetMenu(fileName = "Game Over Outcome", menuName = "Outcomes/Game Over")]
public class GameOver : Outcome
{
    public override bool Execute()
    {
        Manager.GameOver();
        return true;
    }
}