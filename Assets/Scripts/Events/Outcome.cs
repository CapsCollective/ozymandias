using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Outcome : ScriptableObject
{
    
    public string OutcomeName;
    public Event ChainEvent;
    public int ChainEventMaxTurnsAway = 3;

    // What happens when this is executed
    public virtual bool Execute()
    {
        return false;
    }
}
