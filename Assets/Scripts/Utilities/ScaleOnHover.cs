using DG.Tweening;
using Managers;
using UnityEngine;
using UnityEngine.EventSystems;
using static Managers.GameManager;

namespace Utilities
{
    public class ScaleOnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        // Serialised fields
        [SerializeField] private float scaleTarget = 1.2f;
        [SerializeField] private float duration = 0.3f;
        [SerializeField] private GameObject target;
        private bool _interactable = true;

        private void Start()
        {
            if (!target)
            {
                target = gameObject;
            }

            State.OnNextTurnBegin += () => _interactable = false;
            State.OnNextTurnEnd += () => _interactable = true;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            // Ignore disabled buttons
            if (!_interactable) return;

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
