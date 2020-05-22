using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static GameManager;

public class NewspaperController : MonoBehaviour
{
    private const string FillerText = "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.";
    
    // Serialised Fields
    #pragma warning disable 0649
    [SerializeField] private Text newspaperTitle;
    [SerializeField] private EventDisplayManager[] articleList;
    [SerializeField] private Image articleImage;
    [SerializeField] private Button[] choiceList;
    [SerializeField] private Text[] fillerList;
    [SerializeField] private Button continueButton;


    private Event choiceEvent;
    
    private void Awake()
    {
        EventQueue.OnEventsProcessed += UpdateDisplay;
    }

    public void UpdateDisplay(List<Event> events, List<string> descriptions)
    {
        choiceEvent = events[0];
        if (choiceEvent.choices.Count > 0) continueButton.enabled = false;
        
        // Set the image for the main article and a newspaper title
        if(events[0].image != null) articleImage.sprite = events[0].image;
        newspaperTitle.text = GetNewspaperTitle();

        // Assign the remaining events to the corresponding spots
        for (int i = 0; i < events.Count; i++)
        {
            articleList[i].SetEvent(events[i], descriptions[i], i != events.Count - 1);
        }

        // Set all event choices on button texts
        for (var i = 0; i < choiceList.Length; i++)
        {
            SetChoiceActive(i, i < choiceEvent.choices.Count);
        }
    }

    public void SetChoiceActive(int choice, bool active)
    {
        choiceList[choice].gameObject.SetActive(active);
        fillerList[choice].text = active ? "" : FillerText;
        if (active) choiceList[choice].GetComponentInChildren<Text>().text = choiceEvent.choices[choice].name;
    }
    
    public void OnChoiceSelected(int choice)
    {
        articleList[0].AddChoiceOutcome (choiceEvent.MakeChoice(choice));
        continueButton.enabled = true;
        for (var i = 0; i < choiceList.Length; i++) SetChoiceActive(i,false);
        
        Manager.UpdateUi();
    }
    
    private string GetNewspaperTitle()
    {
        return "{ " + "The Wizarding Post" + " }";
        // TODO randomly generate newspaper names
    }
    
    // private Event GetNewspaperAd()
    // {
    //     return adverts[0];
    //     // TODO Add more adverts and pick them at random
    //     var e = ScriptableObject.CreateInstance<Event>();
    //     e.ScenarioTitle = "LESSER POTIONS FOR LESSER HEROES!\nWhatever your strength, we've got you covered at PotionBarn!";
    //     e.ScenarioText = "Rude potion-sellers getting you down? Then come on down to where the potions aren't too hot or too cold for you, because they're just alright.";
    //     return e;
    //     // TODO randomly generate ads
    // }
}
