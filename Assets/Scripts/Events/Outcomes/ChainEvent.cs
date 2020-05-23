using System.Collections;
using System.Collections.Generic;
using System.Management.Instrumentation;
using UnityEngine;
using static GameManager;

[CreateAssetMenu]
public class ChainEvent : Outcome
{
    public Event next;
    public bool toFront;
    public override bool Execute()
    {
        Manager.eventQueue.AddEvent(next, toFront);
        return true;
    }
}
