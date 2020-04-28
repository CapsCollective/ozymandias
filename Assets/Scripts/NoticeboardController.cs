using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NoticeboardController : MonoBehaviour
{
    // Fields
    [SerializeField] private GameObject[] flyerList;
    private Dictionary<string, GameObject> flyerMappings = new Dictionary<string, GameObject>();

    public void UpdateDisplay()
    {
        // Fetch the currently active events
        var events = GetEvents();

        // Create a new mapping for previously posted events
        var newMappings = events.Where(ev => flyerMappings.ContainsKey(ev.ScenarioTitle))
            .ToDictionary(ev => ev.ScenarioTitle, ev => flyerMappings[ev.ScenarioTitle]);
        
        // Remove events that have already been mapped
        events = events.Where(e => !newMappings.Keys.Contains(e.ScenarioTitle)).ToArray();
        
        // Create a shuffled array of unused flyers
        var unusedFlyers = flyerList.Where(f => !flyerMappings.Values.Contains(f)).ToArray()
            .OrderBy(x => new System.Random().Next(1, 8)).ToArray();
        
        // Assign the remaining events to the unused flyers, setting their states and recording mappings
        for (var i = 0; i < unusedFlyers.Length; i++)
        {
            if (i < events.Length)
            {
                unusedFlyers[i].SetActive(true);
                unusedFlyers[i].GetComponent<FlyerManager>().SetEvent(events[i]);
                newMappings.Add(events[i].ScenarioTitle, unusedFlyers[i]);
            }
            else
                unusedFlyers[i].SetActive(false);
        }
        
        // Set the new flyer mappings
        flyerMappings = newMappings;
    }

    private Event[] GetEvents()
    {
        throw new System.NotImplementedException();
    }

}
