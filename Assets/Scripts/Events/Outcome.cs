using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Outcome : ScriptableObject
{
    
    public string OutcomeName;
    public Event ChainEvent;

    // What happens when this is executed
    public virtual void Execute()
    {

    }
}
