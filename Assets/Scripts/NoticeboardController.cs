using UnityEngine;

public class NoticeboardController : MonoBehaviour
{
    [SerializeField] private GameObject[] flyerList;

    private void Start()
    {
        Display();
    }

    public void Display()
    {
        var events = GetDummyEvents();
        for (var i = 0; i < flyerList.Length; i++)
        {
            
            if (i < events.Length)
            {
                var fm = flyerList[i].GetComponent<FlyerManager>();
                fm.SetTitle(events[i].ScenarioTitle);
                fm.SetDescription(events[i].ScenarioText);
            }
            else
                flyerList[i].SetActive(false);
        }
    }

    private static Event[] GetDummyEvents()
    {
        var event1 = ScriptableObject.CreateInstance<Event>();
        event1.ScenarioTitle = "Real Fake Event 1";
        event1.ScenarioText = "This event is brought to you by Scriptable Object 1";
        var event2 = ScriptableObject.CreateInstance<Event>();
        event2.ScenarioTitle = "Real Fake Event 2";
        event2.ScenarioText = "This event is brought to you by Scriptable Object 2";
        return new [] {event1, event2};
    }
}
