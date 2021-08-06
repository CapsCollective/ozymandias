using UnityEngine.EventSystems;
using UnityEngine.UI;
using static GameState.GameManager;

namespace Inputs
{
    public class ExtendedToggle : Toggle
    {
        public override void OnSubmit(BaseEventData eventData)
        {
            if (Manager.Inputs.UsingController)
            {
                return;
            }
            base.OnSubmit(eventData);
        }
    }
}
