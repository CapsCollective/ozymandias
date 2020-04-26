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

}
