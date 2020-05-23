using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Outcome : ScriptableObject
{
    public string customDescription; //An override description for custom outcomes
    public virtual string Description { get {
        return customDescription;
    } }

    // What happens when this is executed
    public virtual bool Execute()
    {
        return false;
    }
    
    public static string Execute(List<Outcome> outcomes)
    {
        string description = "";
        
        foreach (Outcome outcome in outcomes)
        {
            if (outcome.Execute() && outcome.Description != "") description += "• " + outcome.Description + "\n";
        }
        return description.TrimEnd('\n');
    }
}
