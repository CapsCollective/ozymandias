using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using static Managers.GameManager;
using static UI.GameHud.HudObject;

namespace UI
{
    public class GameHud : MonoBehaviour
    {
        [SerializeField] private float animateInDuration = 0.5f, animateOutDuration = 0.5f;
        [SerializeField] private RectTransform topBar, leftButtons, rightButtons, cards;

        public enum HudObject
        {
            TopBar,
            LeftButtons,
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
                {LeftButtons, new HudObjectValues(leftButtons, new Vector2(10,0), new Vector2(-150,0))},
                {RightButtons, new HudObjectValues(rightButtons, Vector2.zero, new Vector2(0,-230))},
                {HudObject.Cards, new HudObjectValues(cards, new Vector2(0,-155), new Vector2(0,-390))},
            };
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
    }
}