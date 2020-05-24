using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Choice : ScriptableObject
{
    public string description;

    public List<Outcome> outcomes = new List<Outcome>();
}
