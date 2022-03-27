using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using Utilities;
using static Managers.GameManager;

namespace Tooltip
{
    public class TooltipPlacement : MonoBehaviour, IPointerExitHandler, IPointerEnterHandler
    {
        [SerializeField] private Vector3 position;
        [SerializeField] private Vector2 pivot;
        [SerializeField] private float delay = 0.2f;
        [SerializeField] private TooltipType type;
        
        private bool _mouseOver;
        private float _mouseTimer;

        [Serializable]
        public struct NavigationDirections
        {
            public TooltipPlacement up, down, left, right;
        }
        public NavigationDirections navigationDirections;
        
        private void Start()
        {
            _mouseTimer = delay;
        }

        private void Update()
        {
            if (!_mouseOver) return;
            
            if (_mouseTimer >= 0 && !Manager.Tooltip.IsVisible()) _mouseTimer -= Time.deltaTime;
            else Manager.Tooltip.Fade(1);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _mouseOver = true;
            RectTransform t = Manager.Tooltip.GetComponent<RectTransform>();
            t.SetParent(transform.parent, false);
            t.pivot = pivot;
            t.anchoredPosition = position;
            Manager.Tooltip.UpdateTooltip(type);
            
            if (Manager.Tooltip.NavigationActive && type != TooltipType.Stability) transform.DOScale(1.1f, 0.3f);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _mouseOver = false;
            _mouseTimer = delay;
            Manager.Tooltip.Fade(0);
            transform.DOScale(1.0f, 0.3f);
        }
    }
}
