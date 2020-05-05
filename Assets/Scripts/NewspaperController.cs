using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class NewspaperController : MonoBehaviour
{
    // Fields
    [SerializeField] private Text newspaperTitle;
    [SerializeField] private GameObject[] articleList;
    [SerializeField] private Image articleImage;
    [SerializeField] private Button[] choiceList;

    public void UpdateDisplay()
    {
        // Set the current newspaper title
        newspaperTitle.text = GetNewspaperTitle();
        
        // Fetch the currently active events
        var events = GetEvents();
        
        // Add the ad to the end of the array
        events = events.Append(GetNewspaperAd()).ToArray();

        // Set the image for the main article
        articleImage.sprite = events[0].ScenarioBackground;

        // Assign the remaining events to the unused flyers, setting their states and recording mappings
        for (var i = 0; i < articleList.Length; i++)
        {
            articleList[i].GetComponent<EventDisplayManager>().SetEvent(events[i]);
        }
        
        // Set all event choices on button texts
        for (var i = 0; i < choiceList.Length; i++)
        {
            choiceList[i].GetComponentInChildren<Text>().text = events[0].Choices[i].ChoiceText;
        }
    }

    private Event[] GetEvents()
    {
        throw new System.NotImplementedException();
    }

    private string GetNewspaperTitle()
    {
        return "The Wizarding Post";
    }
    
    private Event GetNewspaperAd()
    {
        var e = ScriptableObject.CreateInstance<Event>();
        e.ScenarioTitle = "Go buy this thing...";
        e.ScenarioText = "It's a really good thing to buy...";
        return e;
    }
}
