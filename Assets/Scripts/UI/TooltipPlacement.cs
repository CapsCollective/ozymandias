using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Managers;
using UnityEngine;
using UnityEngine.EventSystems;
using static Managers.GameManager;

namespace UI
{
    public class TooltipPlacement : MonoBehaviour, IPointerExitHandler, IPointerEnterHandler
    {
        [SerializeField] private Vector3 offset;
        [SerializeField] private float delay = 0.2f;
        [SerializeField] private TooltipType type;
        
        private bool _mouseOver;
        private float _mouseTimer = 0f;
        
        private void Start()
        {
            _mouseTimer = delay;
        }

        private void Update()
        {
            if (!_mouseOver) return;
            
            if (_mouseTimer >= 0)
                _mouseTimer -= Time.deltaTime;
            else
            {
                Manager.Tooltip.Fade(1);
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _mouseOver = true;
            var t = Manager.Tooltip.transform;
            t.SetParent(transform, false);
            t.localPosition = offset;
            Manager.Tooltip.UpdateTooltip(type);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _mouseOver = false;
            _mouseTimer = delay;
            Manager.Tooltip.Fade(0);
        }
    }
}
