using TMPro;
using UnityEngine.EventSystems;

namespace Inputs
{
    public class ExtendedDropdown : TMP_Dropdown
    {
        public bool isOpen;
        
        public override void OnSubmit(BaseEventData eventData)
        {
            base.OnSubmit(eventData);
            isOpen = true;
            GetComponentInChildren<EventSensitiveScrollRect>().OnUpdateSelected(eventData);
        }

        public override void OnDeselect(BaseEventData eventData)
        {
            base.OnDeselect(eventData);
            isOpen = false;
        }
        
        public override void OnCancel(BaseEventData eventData)
        {
            base.OnCancel(eventData);
            isOpen = false;
        }
    }
}
