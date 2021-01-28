using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    public class UIButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        // Private fields
        private Image _buttonImage;

        private void Start()
        {
            // Populate field values
            _buttonImage = GetComponent<Image>();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            // Start the on-hover animation
            _buttonImage.rectTransform.DOScale(1.2f, 0.3f);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            // End the on-hover animation
            _buttonImage.rectTransform.DOScale(1.0f, 0.3f);
        }
    }
}