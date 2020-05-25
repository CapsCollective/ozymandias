using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static GameManager;

public class NewspaperController : MonoBehaviour
{
    // Serialised Fields
    #pragma warning disable 0649
    [SerializeField] private Text newspaperTitle;
    [SerializeField] private EventDisplayManager[] articleList;
    [SerializeField] private Image articleImage;
    [SerializeField] private Button[] choiceList;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button gameOverButton;
    
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
        if(events[0].image) articleImage.sprite = events[0].image;
        articleImage.gameObject.SetActive(events[0].image);
        newspaperTitle.text = GetNewspaperTitle();

        // Assign the remaining events to the corresponding spots
        for (int i = 0; i < events.Count; i++)
            articleList[i].SetEvent(events[i], descriptions[i], i != events.Count - 1);

        // Set all event choices on button texts
        for (var i = 0; i < choiceList.Length; i++) SetChoiceActive(i, i < choiceEvent.choices.Count);
    }

    public void SetChoiceActive(int choice, bool active)
    {
        choiceList[choice].gameObject.SetActive(active);
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

    public void GameOver()
    {
        continueButton.gameObject.SetActive(false);
        gameOverButton.gameObject.SetActive(true);
    }

    public void QuestToMenu()
    {
        Manager.menuManager.QuitToMenu();
    }
    
    private void OnDestroy()
    {
        EventQueue.OnEventsProcessed -= UpdateDisplay;
    }
}
