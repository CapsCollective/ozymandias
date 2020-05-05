using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Scenario")][System.Serializable]
public class Event : ScriptableObject
{
    public string ScenarioTitle;
    [Multiline] public string ScenarioText;
    public Sprite ScenarioBackground;

    public List<Choice> Choices = new List<Choice>();

    public float minChaos;
    public float minThreat;
    public float minWealth;

    public float maxChaos;
    public float maxThreat;
    public float maxWealth;

    public float weight;

    public Outcome defaultOutcome;
}
