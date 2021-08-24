using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Utilities
{
    public class ScaleOnButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        // Serialised fields
        [SerializeField] private float scaleTarget = 1.2f;
        [SerializeField] private float duration = 0.3f;
        [SerializeField] private GameObject target;

        private void Start()
        {
            if (!target)
            {
                target = gameObject;
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            // Start the on-hover animation
            target.transform.DOScale(scaleTarget, duration);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            // End the on-hover animation
            target.transform.DOScale(1.0f, duration);
        }
        
        public void OnDisable()
        {
            // TODO: This causes an error and needs to be looked at
            // End the on-hover animation if the target gets disabled
            //if (target) target.transform.DOScale(1.0f, duration);
        }
    }
}
