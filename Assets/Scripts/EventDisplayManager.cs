using UnityEngine;
using UnityEngine.UI;

public class EventDisplayManager : MonoBehaviour
{
    #pragma warning disable 0649
    [SerializeField] private Text titleText;
    [SerializeField] private Text descriptionText;
    [SerializeField] private Text outcomeText;

    public void SetEvent(Event e, string outcome, bool upper = false)
    {
        titleText.text = upper ? e.headline.ToUpper() : e.headline;
        descriptionText.text = e.article;
        outcomeText.text = outcome;
    }

    public void AddChoiceOutcome(string outcome)
    {
        outcomeText.text += outcome;
    }
}
