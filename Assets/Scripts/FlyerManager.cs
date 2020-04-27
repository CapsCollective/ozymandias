using UnityEngine;
using UnityEngine.UI;

public class FlyerManager : MonoBehaviour
{
    [SerializeField] private Text titleText;
    [SerializeField] private Text descriptionText;

    private Event @event;

    public void SetEvent(Event e)
    {
        @event = e;
        titleText.text = @event.ScenarioTitle;
        descriptionText.text = @event.ScenarioText;
    }
    
    public Event GetEvent()
    {
        return @event;
    }
}
