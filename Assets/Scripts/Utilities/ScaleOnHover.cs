using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using Inputs;
using static Managers.GameManager;

namespace Utilities
{
    public class ScaleOnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
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
            Inputs.Inputs.OnControlChange += OnControlChange;
        }

        private void OnControlChange(UnityEngine.InputSystem.InputControlScheme scheme)
        {
            if (Manager.Inputs.UsingController)
                InputHelper.OnNewSelection += DoScaling;
            else
                InputHelper.OnNewSelection -= DoScaling;
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
        
        public void OnEnable()
        {
            // End the on-hover animation if the target gets disabled
            if (target) target.transform.localScale = Vector3.one;
        }

        public void DoScaling(GameObject go)
        {
            if(go == gameObject)
            {
                // Start the on-hover animation
                target.transform.DOScale(scaleTarget, duration);
            }
            else
            {
                // End the on-hover animation
                target.transform.DOScale(1.0f, duration);
            }
        }
    }
}
