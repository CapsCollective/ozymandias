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
    [SerializeField] private Text[] fillerList;

    // Private Fields
    private Event[] currentEvents;

    public static Action<Outcome> OnOutcomeSelected;

    private void Awake()
    {
        GameManager.OnNewTurn += () =>
        {
            for (var i = 0; i < choiceList.Length; i++)
            {
                choiceList[i].GetComponentInChildren<Text>().text = "This is a choice you can select it by clicking on it";
                choiceList[i].interactable = false;
            }
        };

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
            var isAd = i == currentEvents.Length-1;
            articleList[i].GetComponent<EventDisplayManager>().SetEvent(currentEvents[i], !isAd);
        }

        // Set all event choices on button texts
        for (var i = 0; i < choiceList.Length; i++)
        {
            if (i < currentEvents[0].Choices.Count)
            {
                choiceList[i].gameObject.SetActive(true);
                choiceList[i].GetComponentInChildren<Text>().text = currentEvents[0].Choices[i].ChoiceTitle;
                choiceList[i].interactable = true;
                fillerList[i].text = "";
            }
            else
            {
                fillerList[i].text = "smaller event description text that contains more details on the quest. The particulars and flavour text are mostly contained within this section and allow the player to engage with the world on a narrative level. This is the smaller event description text that contains more details on the quest. The particulars and flavour text are mostly contained within this section and allow the player to engage with the world on a narrative level. This is the sm";
                choiceList[i].gameObject.SetActive(false);
            }
        }
    }
    
    public void OnChoiceSelected(int choice)
    {
        currentEvents[0].Choices[choice].PossibleOutcomes[0].Execute();
        Array.ForEach(choiceList, b => b.interactable = false);
        OnOutcomeSelected?.Invoke(currentEvents[0].Choices[choice].PossibleOutcomes[0]);
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
        e.ScenarioTitle = "LESSER POTIONS FOR LESSER HEROES!\nWhatever your strength, we've got you covered at PotionBarn!";
        e.ScenarioText = "Rude potion-sellers getting you down? Then come on down to where the potions aren't too hot or too cold for you, because they're just alright.";
        return e;
        // TODO randomly generate ads
    }
}
