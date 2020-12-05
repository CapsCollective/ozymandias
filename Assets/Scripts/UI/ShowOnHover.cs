#pragma warning disable 0649
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    public class ShowOnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private float minPos, maxPos;
        private RectTransform _rect;
        private float _target;
        private bool _moving;
    
        //TODO: Replace this with dot tween
        private void Awake()
        {
            _rect = GetComponent<RectTransform>();
        }
    
        public void OnPointerEnter(PointerEventData eventData)
        {
            _moving = true;
            _target = maxPos;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _moving = true;
            _target = minPos;
        }

        // Update is called once per frame
        private void Update()
        {
            if (!_moving) return;
            _rect.anchoredPosition = new Vector2(Mathf.Lerp(_rect.anchoredPosition.x, _target, 0.05f), _rect.anchoredPosition.y);
            if (Mathf.Abs(_rect.anchoredPosition.x - _target) < 0.1f) _moving = false;
        }
    }
}
