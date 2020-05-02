using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Choice : ScriptableObject
{
    public string ChoiceTitle;
    public string ChoiceText;

    public List<Outcome> PossibleOutcomes = new List<Outcome>();
}
