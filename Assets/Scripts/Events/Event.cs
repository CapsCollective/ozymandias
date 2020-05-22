using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public enum EventType
{
    Flavour,
    Adventurers,
    Threat,
    Chaos,
    Endgame
}

[CreateAssetMenu(fileName = "Scenario")][System.Serializable]
public class Event : ScriptableObject
{
    public string ScenarioTitle;
    [TextArea] public string ScenarioText;
    public Sprite ScenarioBackground;

    public List<Choice> Choices = new List<Choice>();

    public EventType type;
    
    public float minChaos;
    public float minThreat;
    public float minWealth;

    public float maxChaos;
    public float maxThreat;
    public float maxWealth;

    public float weight;

    [Tooltip("These are the outcomes that are run without making a choice")]
    public List<Outcome> EventOutcomes = new List<Outcome>();
}
