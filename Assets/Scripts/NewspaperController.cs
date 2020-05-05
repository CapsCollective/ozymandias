using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class NewspaperController : MonoBehaviour
{
    // Serialised Fields
    [SerializeField] private Text newspaperTitle;
    [SerializeField] private GameObject[] articleList;
    [SerializeField] private Image articleImage;
    [SerializeField] private Button[] choiceList;

    // Private Fields
    private Event[] currentEvents;

    private void Awake()
    {
        EventQueue.OnEventsProcessed += (list) =>
        {
            currentEvents = list.ToArray();
            UpdateDisplay();
        };
    }

    public void UpdateDisplay()
    {
        // Fetch the currently active events and add the advertisement
        //currentEvents = GetEvents();
        currentEvents = currentEvents.Append(GetNewspaperAd()).ToArray();

        // Set the image for the main article and a newspaper title
        if(currentEvents[0].ScenarioBackground != null)
            articleImage.sprite = currentEvents[0].ScenarioBackground;
        newspaperTitle.text = GetNewspaperTitle();

        // Assign the remaining events to the unused flyers, setting their states and recording mappings
        for (var i = 0; i < currentEvents.Length; i++)
        {
            articleList[i].GetComponent<EventDisplayManager>().SetEvent(currentEvents[i]);
        }

        // Set all event choices on button texts
        for (var i = 0; i < choiceList.Length; i++)
        {
            if (currentEvents[0].Choices.Count > 0)
            {
                choiceList[i].GetComponentInChildren<Text>().text = currentEvents[0].Choices[i].ChoiceText;
                choiceList[i].interactable = true;
            }
        }
    }
    
    public void OnChoiceSelected(int choice)
    {
        Array.ForEach(choiceList, b => b.interactable = false);
        // TODO call the game logic with the selected choice with the following:
        // currentEvents[0].Choices[choice]
    }

    private Event[] GetEvents()
    {
        throw new System.NotImplementedException();
    }

    private string GetNewspaperTitle()
    {
        return "{ " + "The Wizarding Post" + " }";
        // TODO randomly generate newspaper names
    }
    
    private Event GetNewspaperAd()
    {
        var e = ScriptableObject.CreateInstance<Event>();
        e.ScenarioTitle = "Go buy this thing...";
        e.ScenarioText = "It's a really good thing to buy...";
        return e;
        // TODO randomly generate ads
    }
}
