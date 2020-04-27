using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NoticeboardController : MonoBehaviour
{
    [SerializeField] private GameObject[] flyerList;

    private Dictionary<string, GameObject> flyerMappings = new Dictionary<string, GameObject>();

    private void Update()
    {
        if (Input.GetKey("q"))
        {
            Update(GetDummyEvents1());
        }
        if (Input.GetKey("w"))
        {
            Update(GetDummyEvents2());
        }
        if (Input.GetKey("e"))
        {
            Update(GetDummyEvents3());
        }
        if (Input.GetKey("r"))
        {
            Update(GetDummyEvents4());
        }
        if (Input.GetKey("t"))
        {
            Update(GetDummyEvents5());
        }
    }

    public void Update(Event[] eventlist)
    {
        var events = eventlist;
        var newMap = new Dictionary<string, GameObject>();
        
        foreach (var ev in events)
        {
            if (flyerMappings.ContainsKey(ev.ScenarioTitle))
            {
                print("found a copy");
                newMap.Add(ev.ScenarioTitle, flyerMappings[ev.ScenarioTitle]);
            }
        }
        flyerMappings = newMap;
        
        events = events.Where(e => !flyerMappings.Keys.Contains(e.ScenarioTitle)).ToArray();
        var availableFlyers = flyerList.Where(f => !flyerMappings.Values.Contains(f)).ToArray();
        for (var i = 0; i < availableFlyers.Length; i++)
        {
            if (i < events.Length)
            {
                availableFlyers[i].SetActive(true);
                availableFlyers[i].GetComponent<FlyerManager>().SetEvent(events[i]);
                flyerMappings.Add(events[i].ScenarioTitle, availableFlyers[i]);
            }
            else
                availableFlyers[i].SetActive(false);
        }
    }

    private static Event[] GetDummyEvents1()
    {
        var event1 = ScriptableObject.CreateInstance<Event>();
        event1.ScenarioTitle = "Real Fake Event 1";
        event1.ScenarioText = "This event is brought to you by Scriptable Object 1";
        var event2 = ScriptableObject.CreateInstance<Event>();
        event2.ScenarioTitle = "Real Fake Event 2";
        event2.ScenarioText = "This event is brought to you by Scriptable Object 2";
        return new [] {event1, event2};
    }
    
    private static Event[] GetDummyEvents2()
    {
        var event2 = ScriptableObject.CreateInstance<Event>();
        event2.ScenarioTitle = "Real Fake Event 2";
        event2.ScenarioText = "This event is brought to you by Scriptable Object 2";
        var event3 = ScriptableObject.CreateInstance<Event>();
        event3.ScenarioTitle = "Real Fake Event 3";
        event3.ScenarioText = "This event is brought to you by Scriptable Object 3";
        return new [] {event2, event3};
    }
    
    private static Event[] GetDummyEvents3()
    {
        var event1 = ScriptableObject.CreateInstance<Event>();
        event1.ScenarioTitle = "Real Fake Event 1";
        event1.ScenarioText = "This event is brought to you by Scriptable Object 1";
        var event3 = ScriptableObject.CreateInstance<Event>();
        event3.ScenarioTitle = "Real Fake Event 3";
        event3.ScenarioText = "This event is brought to you by Scriptable Object 3";
        var event4 = ScriptableObject.CreateInstance<Event>();
        event4.ScenarioTitle = "Real Fake Event 4";
        event4.ScenarioText = "This event is brought to you by Scriptable Object 4";
        var event5 = ScriptableObject.CreateInstance<Event>();
        event5.ScenarioTitle = "Real Fake Event 5";
        event5.ScenarioText = "This event is brought to you by Scriptable Object 5";
        return new [] {event4, event3, event1, event5};
    }
    
    private static Event[] GetDummyEvents4()
    {
        var event4 = ScriptableObject.CreateInstance<Event>();
        event4.ScenarioTitle = "Real Fake Event 4";
        event4.ScenarioText = "This event is brought to you by Scriptable Object 4";
        return new [] {event4};
    }
    
    private static Event[] GetDummyEvents5()
    {
        return new Event[] {};
    }
}
