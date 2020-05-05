using UnityEngine;
using UnityEngine.UI;

public class EventDisplayManager : MonoBehaviour
{
    [SerializeField] private Text titleText;
    [SerializeField] private Text descriptionText;

    public void SetEvent(Event e)
    {
        titleText.text = e.ScenarioTitle;
        descriptionText.text = e.ScenarioText;
    }
}
