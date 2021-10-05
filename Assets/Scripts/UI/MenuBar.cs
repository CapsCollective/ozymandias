using System;
using DG.Tweening;
using Events;
using Managers;
using UnityEngine;
using UnityEngine.UI;
using static Managers.GameManager;

namespace UI
{
    internal struct MenuButton
    {
        public MenuButton(Button btn, Func<bool> displayPolicy)
        {
            Button = btn;
            ShouldDisplay = displayPolicy;
            StartPos = btn.transform.localPosition;
            OffsetPos = StartPos + new Vector3(-150, 0, 0);
            IsDisplaying = true;
        }

        public readonly Button Button;
        public bool IsDisplaying;
        public Vector3 StartPos { get; }
        public Vector3 OffsetPos { get; }
        public Func<bool> ShouldDisplay { get; }
    }
    
    public class MenuBar : MonoBehaviour
    {
        public float duration = 0.5f;
        public Button questButton;
        public Button newspaperButton;
        
        private MenuButton _questButton;
        private MenuButton _newspaperButton;

        private bool _newspaperClosed;

        private void Start()
        {
            // Setup buttons and their display policies
            _questButton = new MenuButton(questButton, () => Manager.Quests.Count > 0);
            _newspaperButton = new MenuButton(newspaperButton, () => _newspaperClosed);
            
            Newspaper.OnClosed += () =>
            {
                _newspaperClosed = true;
                UpdateDisplay();
            };
            
            State.OnGameEnd += () => _newspaperClosed = false;
            UpdateDisplay();
        }

        private void UpdateDisplay()
        {
            // Set display of quest button
            var questButtonChanged = false;
            var displayQuests = _questButton.ShouldDisplay();
            if (_questButton.IsDisplaying != displayQuests)
            {
                _questButton.Button.transform.DOLocalMove(
                    displayQuests ? _questButton.StartPos : _questButton.OffsetPos, duration);
                _questButton.IsDisplaying = displayQuests;
                questButtonChanged = true;
            }
            
            // Set display of newspaper button (updates if quest button is changing)
            var displayNewspaper = _newspaperButton.ShouldDisplay();
            if (_newspaperButton.IsDisplaying != displayNewspaper || questButtonChanged)
            {
                MenuButton button = displayQuests ? _newspaperButton : _questButton;
                _newspaperButton.Button.transform.DOLocalMove(
                    displayNewspaper ? button.StartPos : button.OffsetPos, duration);
                _newspaperButton.IsDisplaying = displayNewspaper;
            }
        }
    }
}