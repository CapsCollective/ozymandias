#pragma warning disable 0649
using System;
using System.Collections.Generic;
using DG.Tweening;
using Managers;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;
using static Managers.GameManager;
using Random = UnityEngine.Random;

namespace Controllers
{
    public class Newspaper : MonoBehaviour
    {
        // Serialised Fields
        [SerializeField] private Text titleText;
        [SerializeField] private NewspaperEvent[] articleList;
        [SerializeField] private Image articleImage;
        [SerializeField] private Button[] choiceList;
        [SerializeField] private Button continueButton;
        [SerializeField] private Button gameOverButton;
        [SerializeField] private TextMeshProUGUI turnCounter;
        [SerializeField] private GameObject continueButtonContent;
        [SerializeField] private GameObject disableButtonContent;
        [SerializeField] private float animateInDuration = .5f;
        [SerializeField] private float animateOutDuration = .75f;
        private Event _choiceEvent;
        private string _newspaperTitle;
        private Canvas _canvas;

        private void Start()
        {
            _canvas = GetComponent<Canvas>();
            _newspaperTitle = GetNewspaperTitle();
            titleText.text = "{ " + _newspaperTitle + " }";
            GameManager.OnNewTurn += OnNewTurn;
            continueButton.onClick.AddListener(Close);
            Close();
        }

        private void OnNewTurn()
        {
            Open();
            if (Random.Range(0, 5) == 2) Jukebox.Instance.PlayMorning(); // 1/5 chance to play sound
            
            if (PlayerPrefs.GetInt("tutorial_video_events", 0) > 0) return;
            PlayerPrefs.SetInt("tutorial_video_events", 1);
            //TutorialPlayerController.Instance.PlayClip(1);
        }

        public void UpdateDisplay(List<Event> events, List<string> descriptions)
        {
            turnCounter.text = _newspaperTitle + ", Turn " + Manager.TurnCounter;
        
            _choiceEvent = events[0];
            if (_choiceEvent.choices.Count > 0) SetContinueButtonEnabled(false);
        
            // Set the image for the main article and a newspaper title
            if(events[0].image) articleImage.sprite = events[0].image;
            articleImage.gameObject.SetActive(events[0].image);

            // Assign the remaining events to the corresponding spots
            for (int i = 0; i < events.Count; i++)
                articleList[i].SetEvent(events[i], descriptions[i], i != events.Count - 1);

            // Set all event choices on button texts
            for (var i = 0; i < choiceList.Length; i++) SetChoiceActive(i, i < _choiceEvent.choices.Count);
        }

        private void SetChoiceActive(int choice, bool active)
        {
            choiceList[choice].gameObject.SetActive(active);
            if (active) choiceList[choice].GetComponentInChildren<TextMeshProUGUI>().text = _choiceEvent.choices[choice].name;
        }
    
        public void OnChoiceSelected(int choice)
        {
            articleList[0].AddChoiceOutcome (_choiceEvent.MakeChoice(choice));
            SetContinueButtonEnabled(true);
            for (int i = 0; i < choiceList.Length; i++) SetChoiceActive(i,false);
        
            Manager.UpdateUi();
            Save(); // Need to save again after a choice to lock in its outcomes
        }
    
        private static readonly string[] NewspaperTitles = {
            "The Wizarding Post", "The Adventurer's Economist", "The Daily Guild", "Dimensional Press",
            "The Conduit Chronicle", "The Questing Times"
        };

        private static string GetNewspaperTitle()
        {
            Random.InitState((int)DateTime.Now.Ticks);
            return NewspaperTitles[Random.Range(0, NewspaperTitles.Length)];
        }

        public void GameOver()
        {
            continueButton.gameObject.SetActive(false);
            gameOverButton.gameObject.SetActive(true);
        }

        public void QuitToMenu()
        {
            Manager.Settings.QuitToMenu();
        }

        private void SetContinueButtonEnabled(bool state)
        {
            continueButton.enabled = state;
            continueButtonContent.SetActive(state);
            disableButtonContent.SetActive(!state);
        }
        
        public void Open()
        {
            _canvas.enabled = true;
            Manager.EnterMenu();
            Jukebox.Instance.PlayScrunch();
            transform
                .DOLocalMove(Vector3.zero, animateInDuration)
                .OnStart(() => { _canvas.enabled = true; });
            transform.DOLocalRotate(Vector3.zero, animateInDuration);
        }
        
        public void Close()
        {
            Manager.ExitMenu();
            transform.DOLocalMove(new Vector3(1000, 500, 0), animateOutDuration);
            transform
                .DOLocalRotate(new Vector3(0, 0, -20), animateOutDuration)
                .OnComplete(() => { _canvas.enabled = false; });
        }
    }
}
