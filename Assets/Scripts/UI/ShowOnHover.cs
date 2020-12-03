using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    public class ShowOnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public float minPos;
        public float maxPos;
        private RectTransform rect;
        private float target;
        public bool moving;
    
        //TODO: Replace this with dot tween
        void Awake()
        {
            rect = GetComponent<RectTransform>();
        }
    
        public void OnPointerEnter(PointerEventData eventData)
        {
            moving = true;
            target = maxPos;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            moving = true;
            target = minPos;
        }

        // Update is called once per frame
        void Update()
        {
            if (!moving) return;
            rect.anchoredPosition = new Vector2(Mathf.Lerp(rect.anchoredPosition.x, target, 0.05f), rect.anchoredPosition.y);
            if (Mathf.Abs(rect.anchoredPosition.x - target) < 0.1f) moving = false;
        }
    }
}
