using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Managers;
using Utilities;
using System.Collections;
using System.IO;
using NaughtyAttributes;
using static Managers.GameManager;
using static UI.GameHud.HudObject;
using System;

namespace UI
{
    public class GameHud : MonoBehaviour
    {
        [SerializeField] private float animateInDuration = 0.5f, animateOutDuration = 0.5f;
        [SerializeField] private RectTransform topBar, menuBar, rightButtons, cards;
        [SerializeField] private CanvasGroup menuBarGroup, rightGameGroup;

        public enum HudObject
        {
            TopBar,
            MenuBar,
            RightButtons,
            Cards,
        }

        private readonly struct HudObjectValues
        {
            public readonly RectTransform RectTransform;
            public readonly Vector2 ShowPos;
            public readonly Vector2 HidePos;

            public HudObjectValues(RectTransform rect, Vector2 show, Vector2 hide)
            {
                RectTransform = rect;
                ShowPos = show;
                HidePos = hide;
            }
        }

        private Dictionary<HudObject, HudObjectValues> _hudValuesMap;

        private void Start()
        {
            _hudValuesMap = new Dictionary<HudObject, HudObjectValues>
            {
                {TopBar, new HudObjectValues(topBar, Vector2.zero, new Vector2(0,200))},
                {HudObject.MenuBar, new HudObjectValues(menuBar, new Vector2(0,0), new Vector2(0, 150))},
                {RightButtons, new HudObjectValues(rightButtons, Vector2.zero, new Vector2(0,-230))},
                {HudObject.Cards, new HudObjectValues(cards, new Vector2(0,-155), new Vector2(0,-390))},
            };

            State.OnEnterState += OnNewState;

#if UNITY_EDITOR
            Manager.Inputs.OnScreenshot.performed += obj => { TakeScreenshot(); };
#endif
        }

        private void OnNewState(GameState state)
        {
            switch (state)
            {
                case GameState.InGame:
                    menuBarGroup.interactable = true;
                    rightGameGroup.interactable = true;
                    break;
                case GameState.InMenu:
                    menuBarGroup.interactable = false;
                    rightGameGroup.interactable = false;
                    break;
                case GameState.NextTurn:
                    menuBarGroup.interactable = false;
                    rightGameGroup.interactable = false;
                    break;
            }
        }

        public void Hide(bool animate = true)
        {
            Hide(new List<HudObject>(_hudValuesMap.Keys), animate);
        }

        public void Hide(List<HudObject> objects, bool animate = true)
        {
            foreach (HudObject obj in objects) Hide(obj, animate);
        }
        
        public void Hide(HudObject obj, bool animate = true)
        {
            HudObjectValues objValues = _hudValuesMap[obj];
            if (animate) objValues.RectTransform.DOAnchorPos(objValues.HidePos, animateOutDuration);
            else objValues.RectTransform.anchoredPosition = objValues.HidePos;
        }
        
        public void Show(bool animate = true)
        {
            Show(new List<HudObject>(_hudValuesMap.Keys), animate);
        }
        
        public void Show(List<HudObject> objects, bool animate = true)
        {
            foreach (HudObject obj in objects) _Show(obj, animate);
            UpdateUi();
        }

        public void Show(HudObject obj, bool animate = true)
        {
            // Wraps UI updating functionality
            _Show(obj, animate);
            UpdateUi();
        }

        private void _Show(HudObject obj, bool animate = true)
        {
            HudObjectValues objValues = _hudValuesMap[obj];
            if (animate) objValues.RectTransform.DOAnchorPos(objValues.ShowPos, animateInDuration);
            else objValues.RectTransform.anchoredPosition = objValues.ShowPos;
        }

        [Button("Take Screenshot")]
        private void TakeScreenshot()
        {
            // This unfortunately needs to run as a coroutine to make sure the UI is not visible
            StartCoroutine(_TakeScreenshot());
        }
        
        private IEnumerator _TakeScreenshot()
        {
            CanvasGroup gameCanvas = gameObject.GetComponent<CanvasGroup>();
            gameCanvas.alpha = 0;
            const string screenshotDir = "Screenshots";
            Directory.CreateDirectory(screenshotDir);
            var filename = $"{screenshotDir}/FTRM_{DateTime.Now:dd-MM-yyyy-hh-mm-ss}.png";
            ScreenCapture.CaptureScreenshot(filename);
            Debug.Log($"Saved screenshot capture to {filename}");
            yield return null;
            gameCanvas.alpha = 1;
        }
    }
}
