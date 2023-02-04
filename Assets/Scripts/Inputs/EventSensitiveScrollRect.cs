using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// https://www.alexandrow.org/blog/autoscroll-dropdowns-in-unity/

namespace Inputs
{
    public class EventSensitiveScrollRect : MonoBehaviour {
 
        private static readonly float SCROLL_MARGIN = 0.3f; // how much to "overshoot" when scrolling, relative to the selected item's height
        
        private ScrollRect sr;
 
        public void Awake() {
            sr = gameObject.GetComponent<ScrollRect>();
        }
 
        public void OnUpdateSelected(BaseEventData eventData) {
            // helper vars
            float contentHeight = sr.content.rect.height;
            float viewportHeight = sr.viewport.rect.height;
 
            // what bounds must be visible?
            float centerLine = eventData.selectedObject.transform.localPosition.y; // selected item's center
            float upperBound = centerLine + (eventData.selectedObject.GetComponent<RectTransform>().rect.height / 2f); // selected item's upper bound
            float lowerBound = centerLine - (eventData.selectedObject.GetComponent<RectTransform>().rect.height / 2f); // selected item's lower bound
 
            // what are the bounds of the currently visible area?
            float lowerVisible = (contentHeight - viewportHeight) * sr.normalizedPosition.y - contentHeight;
            float upperVisible = lowerVisible + viewportHeight;
 
            // is our item visible right now?
            float desiredLowerBound;
            if (upperBound > upperVisible) {
                // need to scroll up to upperBound
                desiredLowerBound = upperBound - viewportHeight + eventData.selectedObject.GetComponent<RectTransform>().rect.height * SCROLL_MARGIN;
            } else if (lowerBound < lowerVisible) {
                // need to scroll down to lowerBound
                desiredLowerBound = lowerBound - eventData.selectedObject.GetComponent<RectTransform>().rect.height * SCROLL_MARGIN;
            } else {
                // item already visible - all good
                return;
            }
 
            // normalize and set the desired viewport
            float normalizedDesired = (desiredLowerBound + contentHeight) / (contentHeight - viewportHeight);
            sr.normalizedPosition = new Vector2(0f, Mathf.Clamp01(normalizedDesired));
        }
 
    }
}
