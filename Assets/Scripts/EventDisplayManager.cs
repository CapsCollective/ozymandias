using UnityEngine;
using UnityEngine.UI;

public class EventDisplayManager : MonoBehaviour
{
    [SerializeField] private Text titleText;
    [SerializeField] private Text descriptionText;
    [SerializeField] private Text effectText;

    public void SetEvent(Event e, bool upper = false)
    {
        titleText.text = upper ? e.ScenarioTitle.ToUpper() : e.ScenarioTitle;
        descriptionText.text = e.ScenarioText;
        if (e.EventOutcomes.Count > 0)
            effectText.text = EventQueue.outcomeString ?? "";
    }
}
