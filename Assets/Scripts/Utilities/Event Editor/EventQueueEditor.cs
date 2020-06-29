using Boo.Lang;
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EventQueue))]
public class EventQueueEditor : Editor
{
    private EventQueue e; 

    private void OnEnable()
    {
        e = (EventQueue)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
   
        if(GUILayout.Button("Add All Events"))
        {
            List<Event> allEvents = new List<Event>();
            var events = AssetDatabase.FindAssets($"t:{typeof(Event)}");
            foreach (var s in events)
            {

                var eventPath = AssetDatabase.GUIDToAssetPath(s);
                var eevent = AssetDatabase.LoadAssetAtPath<Event>(eventPath);

                if (e.filterEvents.Contains(eevent))
                    continue;

                allEvents.Add(eevent);
            }
            e.allEvents = allEvents.ToList();
        }
    }
}
