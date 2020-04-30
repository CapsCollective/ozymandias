using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Event Queue")]
public class EventQueue : ScriptableObject
{
    public List<Event> EventsQueue = new List<Event>();
}
