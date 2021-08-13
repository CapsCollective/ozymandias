using TMPro;
using UnityEngine;
using Event = Events.Event;

namespace UI
{
    public class NewspaperEvent : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI titleText, descriptionText, outcomeText;

        public void SetEvent(Event e, string outcome, bool upper = false)
        {
            titleText.text = upper ? e.headline.ToUpper() : e.headline;
            descriptionText.text = e.article;
            outcomeText.text = outcome;
            outcomeText.gameObject.SetActive(!outcome.Equals(""));
        }

        public void AddChoiceOutcome(string outcome)
        {
            outcomeText.text += outcome;
            outcomeText.gameObject.SetActive(!outcome.Equals(""));
        }
    }
}
