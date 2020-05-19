﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Outcome : ScriptableObject
{

    public string OutcomeName;
    public Event ChainEvent;
    public int ChainEventMaxTurnsAway = 3;

    private static string[] dummyReplies =
    {
        "Testing if the outcome is different!",
        "Chaos has increased!\nEfficiency has increased!",
        "Chaos has increased!\nEfficiency has increased!\nSomething else has increased!"
    };

    // What happens when this is executed
    public virtual bool Execute()
    {
        return false;
    }

    public virtual string GetOutcomeString()
    {
        return dummyReplies[Random.Range(0, 3)];
    }
}
