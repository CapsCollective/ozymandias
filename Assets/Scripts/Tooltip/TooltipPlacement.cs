using UnityEngine;
using UnityEngine.EventSystems;
using Utilities;
using static GameState.GameManager;

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
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _mouseOver = false;
            _mouseTimer = delay;
            Manager.Tooltip.Fade(0);
        }
    }
}
