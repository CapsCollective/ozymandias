using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Utilities
{
    public class ExtendedDropdown : TMP_Dropdown
    {
        public bool isOpen;
        
        public override void OnSubmit(BaseEventData eventData)
        {
            base.OnSubmit(eventData);
            isOpen = true;
            UnityEngine.Debug.Log("Submit");
        }

        public override void OnCancel(BaseEventData eventData)
        {
            base.OnCancel(eventData);
            isOpen = false;
            UnityEngine.Debug.Log("Cancel");
        }
    }
}
