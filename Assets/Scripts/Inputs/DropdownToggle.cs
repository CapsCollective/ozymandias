using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Inputs
{
    public class DropdownToggle : Toggle
    {
        public override void OnSelect(BaseEventData eventData)
        {
            base.OnSelect(eventData);
            GetComponentInParent<EventSensitiveScrollRect>().OnUpdateSelected(eventData);
        }
    }
}
