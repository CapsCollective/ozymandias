using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    public class CardBadge : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public string Description { get; set; }
        [SerializeField] private RectTransform tooltip;
        [SerializeField] private CanvasGroup tooltipCanvas;
        [SerializeField] private TextMeshProUGUI tooltipText;

        public void OnPointerEnter(PointerEventData eventData)
        {
            tooltip.position = transform.position + new Vector3(0, 50);
            tooltipText.text = Description;
            Fade(1);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Fade(0);
        }

        private void Fade(float opacity)
        {
            tooltipCanvas.DOFade(opacity, 0.5f);
        }
    }
}
