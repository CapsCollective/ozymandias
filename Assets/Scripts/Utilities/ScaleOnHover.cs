using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Utilities
{
    public class ScaleOnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        // Serialised fields
        [SerializeField] private float scaleTarget = 1.2f;
        [SerializeField] private float duration = 0.3f;
        [SerializeField] private GameObject target;
        public bool interactable = true;

        private void Start()
        {
            if (!target)
            {
                target = gameObject;
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            // Ignore disabled buttons
            if (!interactable) return;

            // Start the on-hover animation
            target.transform.DOScale(scaleTarget, duration);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            // End the on-hover animation
            target.transform.DOScale(1.0f, duration);
        }
        
        public void OnEnable()
        {
            // End the on-hover animation if the target gets disabled
            if (target) target.transform.localScale = Vector3.one;
        }
    }
}
