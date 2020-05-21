using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Outcome : ScriptableObject
{

    public string OutcomeName;
    public string OutcomeFlavourText;

    private static string[] dummyReplies =
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
        return OutcomeFlavourText;
    }
}
