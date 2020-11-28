using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static GameManager;
using Random = UnityEngine.Random;

namespace Managers_and_Controllers
{
    public class NewspaperController : MonoBehaviour
    {
        // Serialised Fields
        #pragma warning disable 0649
        [SerializeField] private Text titleText;
        [SerializeField] private EventDisplayManager[] articleList;
        [SerializeField] private Image articleImage;
        [SerializeField] private Button[] choiceList;
        [SerializeField] private Button continueButton;
        [SerializeField] private Button gameOverButton;
        [SerializeField] private TextMeshProUGUI turnCounter;
        [SerializeField] private GameObject continueButtonContent;
        [SerializeField] private GameObject disableButtonContent;
        private Event choiceEvent;
        private string newspaperTitle;

        private void Awake()
        {
            newspaperTitle = GetNewspaperTitle();
            titleText.text = "{ " + newspaperTitle + " }";
            EventQueue.OnEventsProcessed += UpdateDisplay;
            GameManager.OnNewTurn += OnNewTurn;
        }

        private void OnNewTurn()
        {
            OnOpened();
            if (PlayerPrefs.GetInt("tutorial_video_events", 0) > 0) return;
            PlayerPrefs.SetInt("tutorial_video_events", 1);
            TutorialPlayerController.Instance.PlayClip(1);
        }
        
        public void OnOpened()
        {
            JukeboxController.Instance.PlayScrunch();
            if (Random.Range(0, 5) != 2) return;
            JukeboxController.Instance.PlayMorning();
        }

        public void UpdateDisplay(List<Event> events, List<string> descriptions)
        {
            turnCounter.text = newspaperTitle + ", Turn " + Manager.turnCounter;
        
            choiceEvent = events[0];
            if (choiceEvent.choices.Count > 0) SetContinueButtonEnabled(false);
        
            // Set the image for the main article and a newspaper title
            if(events[0].image) articleImage.sprite = events[0].image;
            articleImage.gameObject.SetActive(events[0].image);

            // Assign the remaining events to the corresponding spots
            for (int i = 0; i < events.Count; i++)
                articleList[i].SetEvent(events[i], descriptions[i], i != events.Count - 1);

            // Set all event choices on button texts
            for (var i = 0; i < choiceList.Length; i++) SetChoiceActive(i, i < choiceEvent.choices.Count);
        }

        public void SetChoiceActive(int choice, bool active)
        {
            choiceList[choice].gameObject.SetActive(active);
            if (active) choiceList[choice].GetComponentInChildren<TextMeshProUGUI>().text = choiceEvent.choices[choice].name;
        }
    
        public void OnChoiceSelected(int choice)
        {
            articleList[0].AddChoiceOutcome (choiceEvent.MakeChoice(choice));
            SetContinueButtonEnabled(true);
            for (var i = 0; i < choiceList.Length; i++) SetChoiceActive(i,false);
        
            Manager.UpdateUi();
            Manager.Save(); // Need to save again after a choice to lock in its outcomes
        }
    
        private static string[] NewspaperTitles = {
            "The Wizarding Post", "The Adventurer's Economist", "The Daily Guild", "Dimensional Press",
            "The Conduit Chronicle", "The Questing Times"
        };

        private string GetNewspaperTitle()
        {
            Random.InitState((int)DateTime.Now.Ticks);
            return NewspaperTitles[Random.Range(0, NewspaperTitles.Length)];
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

        private void SetContinueButtonEnabled(bool state)
        {
            continueButton.enabled = state;
            continueButtonContent.SetActive(state);
            disableButtonContent.SetActive(!state);
        }
    }
}
