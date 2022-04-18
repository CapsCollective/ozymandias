using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Cards
{
    public class CardBadge : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public bool IsShowing = false;
        public string Description { get; set; }
        [SerializeField] private RectTransform tooltip;
        [SerializeField] private CanvasGroup tooltipCanvas;
        [SerializeField] private TextMeshProUGUI tooltipText;

        public void OnPointerEnter(PointerEventData eventData)
        {
            IsShowing = true;
            tooltip.position = transform.position;
            tooltipText.text = Description;
            Fade(1);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            IsShowing = false;
            Fade(0);
        }

        private void Fade(float opacity)
        {
            tooltipCanvas.DOFade(opacity, 0.5f);
        }
    }
}
