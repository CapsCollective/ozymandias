using TMPro;
using UnityEngine;

namespace UI
{
    public class NewspaperEvent : MonoBehaviour
    {
        #pragma warning disable 0649
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI descriptionText;
        [SerializeField] private TextMeshProUGUI outcomeText;

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
