using System;
using System.Collections.Generic;
using DG.Tweening;
using Managers;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;
using static Managers.GameManager;
using Event = Entities.Event;
using Random = UnityEngine.Random;

namespace Controllers
{
    public class Newspaper : MonoBehaviour
    {
        // Public fields
        public static Action OnClosed;
        
        // Serialised Fields
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private NewspaperEvent[] articleList;
        [SerializeField] private Image articleImage;
        [SerializeField] private Button[] choiceList;
        [SerializeField] private Button continueButton;
        [SerializeField] private TextMeshProUGUI turnCounter;
        [SerializeField] private GameObject continueButtonContent;
        [SerializeField] private GameObject disableButtonContent;
        [SerializeField] private GameObject gameOverButtonContent;

        [SerializeField] private float animateInDuration = .5f;
        [SerializeField] private float animateOutDuration = .75f;
        private Event _choiceEvent;
        private string _newspaperTitle;
        private Canvas _canvas;
        private bool _choiceSelected;

        private enum ButtonState
        {
            Close,
            Choice,
            GameOver
        }
        
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
            if (Random.Range(0, 5) == 2) Manager.Jukebox.PlayMorning(); // 1/5 chance to play sound
            
            if (PlayerPrefs.GetInt("tutorial_video_events", 0) > 0) return;
            PlayerPrefs.SetInt("tutorial_video_events", 1);
            //TutorialPlayerController.Instance.PlayClip(1);
        }

        public void UpdateDisplay(List<Event> events, List<string> descriptions)
        {
            turnCounter.text = _newspaperTitle + ", Turn " + Manager.TurnCounter;
        
            _choiceEvent = events[0];
            if (_choiceEvent.choices.Count > 0) SetContinueButtonState(ButtonState.Choice);
            else SetContinueButtonState(Manager.IsGameOver ? ButtonState.GameOver : ButtonState.Close);
        
            // Set the image for the main article and a newspaper title
            if(events[0].image) articleImage.sprite = events[0].image;
            articleImage.gameObject.SetActive(events[0].image);

            // Assign the remaining events to the corresponding spots
            for (int i = 0; i < events.Count; i++)
                articleList[i].SetEvent(events[i], descriptions[i], i != events.Count - 1);

            // Set all event choices on button texts
            for (var i = 0; i < choiceList.Length; i++) SetChoiceActive(i, i < _choiceEvent.choices.Count);
            if (!_choiceSelected) UIEventController.SelectUI(continueButton.gameObject);
        }

        private void SetChoiceActive(int choice, bool active)
        {
            choiceList[choice].gameObject.SetActive(active);
            if (active) choiceList[choice].GetComponentInChildren<TextMeshProUGUI>().text = 
                _choiceEvent.choices[choice].name;

            if (choice == 0 && active)
            {
                UIEventController.SelectUI(choiceList[choice].gameObject);
                _choiceSelected = true;
            };
        }
    
        public void OnChoiceSelected(int choice)
        {
            //TODO: This needs to be more robust for the controller input
            if (!_canvas.enabled) return; // Ignore input if newspaper is closed
            
            articleList[0].AddChoiceOutcome (_choiceEvent.MakeChoice(choice));
            SetContinueButtonState(ButtonState.Close);
            for (int i = 0; i < choiceList.Length; i++) SetChoiceActive(i,false);
        
            Manager.UpdateUi();
            SaveFile.SaveState(); // Need to save again after a choice to lock in its outcomes
            UIEventController.SelectUI(continueButton.gameObject);
        }
    
        private static readonly string[] NewspaperTitles = {
            "The Wizarding Post", "The Adventurer's Economist", "The Daily Guild", "Dimensional Press",
            "The Conduit Chronicle", "The Questing Times"
        };

        private static string GetNewspaperTitle()
        {
            return NewspaperTitles[Random.Range(0, NewspaperTitles.Length)];
        }
        
        private void SetContinueButtonState(ButtonState state)
        {
            continueButton.enabled = state != ButtonState.Choice;
            continueButtonContent.SetActive(state == ButtonState.Close);
            disableButtonContent.SetActive(state == ButtonState.Choice);
            gameOverButtonContent.SetActive(state == ButtonState.GameOver);
        }
        
        public void Open()
        {
            _canvas.enabled = true;
            Manager.EnterMenu();
            Manager.Jukebox.PlayScrunch();
            transform
                .DOLocalMove(Vector3.zero, animateInDuration)
                .OnStart(() => { _canvas.enabled = true; });
            transform.DOLocalRotate(Vector3.zero, animateInDuration);
        }
        
        public void Close()
        {
            Manager.ExitMenu();
            transform.DOLocalMove(new Vector3(2000, 800, 0), animateOutDuration);
            transform
                .DOLocalRotate(new Vector3(0, 0, -20), animateOutDuration)
                .OnComplete(() =>
                {
                    OnClosed?.Invoke();
                    _canvas.enabled = false;
                });
            UIEventController.SelectUI(null);
        }
    }
}
