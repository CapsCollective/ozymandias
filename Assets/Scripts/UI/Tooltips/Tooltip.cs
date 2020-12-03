using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.Tooltips
{
    /*
 * At the moments works with the place script to handle hover actions (because at the moment, it requires a right click to open up a help
 * This doesn't translate that well when it's scaled to more than just building information (having to right click everything, even knowing to right click)
 * As a result, making the helper object fade in after a certain amount of time of the cursor being hovered over an object
 * In addition, making enumerators for this script so it adapts to the need of each ui object as opposed to having to make a new script each time
 */ 
    
    public class Tooltip : MonoBehaviour, IPointerExitHandler, IPointerEnterHandler
    {
        //[SerializeField] private GameObject[] helperPrefab = new GameObject[5];
        public GameObject tooltipPrefab;
        public Vector3 offset;

        public float delay = 0.2f;
        public float fadeDuration = 0.3f;

        //building[0], threat[1], quest[2], destroy[3], money[4], sidebar[5]
        //public UIType uiType;
        private GameObject tooltipInstance;
        private CanvasGroup tooltipCanvasGroup;
        private TooltipHelper helper;
        private bool mouseOver;
        private float mouseTimer = 0f;

        private void Start()
        {
            mouseTimer = delay;
            tooltipInstance = Instantiate(tooltipPrefab, transform, false);
            tooltipInstance.transform.localPosition = offset;
            tooltipCanvasGroup = tooltipInstance.GetComponent<CanvasGroup>();
            helper = tooltipInstance.GetComponent<TooltipHelper>();
            tooltipCanvasGroup.alpha = 0;
            tooltipCanvasGroup.interactable = false;
        }

        private void Update()
        {
            if (mouseTimer >= 0 && mouseOver)
                mouseTimer -= Time.deltaTime;

            if (mouseTimer < 0)
                tooltipCanvasGroup.DOFade(1, fadeDuration);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            mouseOver = true;
            helper?.UpdateTooltip();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            mouseOver = false;
            mouseTimer = delay;
            tooltipCanvasGroup.DOFade(0, fadeDuration);
        }
    }
}
