using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ExtendedToggle : Toggle
{
    public override void OnSubmit(BaseEventData eventData)
    {
        if (InputManager.UsingController)
        {
            return;
        }
        base.OnSubmit(eventData);
    }
}
