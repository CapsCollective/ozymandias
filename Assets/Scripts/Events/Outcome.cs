using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Outcome : ScriptableObject
{

    public string OutcomeName;
    public Event ChainEvent;
    public int ChainEventMaxTurnsAway = 3;

    private string[] dummyReplies =
    {
        "• Chaos has increased!",
        "• Chaos has increased!\n• Efficiency has increased!",
        "• Chaos has increased!\n• Efficiency has increased!\n• Something else has increased!"
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
