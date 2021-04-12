using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    public class UIButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        // Serialised fields
        [SerializeField] private float scaleTarget = 1.2f;
        [SerializeField] private float duration = 0.3f;

        public void OnPointerEnter(PointerEventData eventData)
        {
            // Start the on-hover animation
            gameObject.transform.DOScale(scaleTarget, duration);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            // End the on-hover animation
            gameObject.transform.DOScale(1.0f, duration);
        }
    }
}