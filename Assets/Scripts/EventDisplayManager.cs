using UnityEngine;
using UnityEngine.UI;

public class EventDisplayManager : MonoBehaviour
{
    [SerializeField] private Text titleText;
    [SerializeField] private Text descriptionText;

    public void SetEvent(Event e, bool upper = false)
    {
        titleText.text = upper ? e.ScenarioTitle.ToUpper() : e.ScenarioTitle;
        descriptionText.text = e.ScenarioText;
    }
}
