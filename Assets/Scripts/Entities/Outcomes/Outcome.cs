using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Outcome : ScriptableObject
{
    public string customDescription; //An override description for custom outcomes
    public virtual string Description => customDescription;

    // What happens when this is executed
    public virtual bool Execute()
    {
        return false;
    }
    
    public virtual bool Execute(bool fromChoice)
    {
        return false;
    }
    
    public static string Execute(List<Outcome> outcomes, bool fromChoice=false)
    {
        string description = "";
        
        foreach (Outcome outcome in outcomes)
        {
            bool res = outcome is StatChange ? outcome.Execute(fromChoice) : outcome.Execute();
            
            if (res && outcome.Description != "") description += "• " + outcome.Description + "\n";
        }
        return description.TrimEnd('\n');
    }
}
