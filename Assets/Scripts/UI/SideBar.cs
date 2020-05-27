using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static GameManager;

public class SideBar : UiUpdater
{
    public TextMeshProUGUI adventurers, spending;

    public BarFill effectiveness, satisfaction;

    public Toggle toggleSize;
    public RectTransform toggleSizeIcon;
    public RectTransform smallPanel;
    public RectTransform largePanelMask;
    
    // Update is called once per frame
    public override void UpdateUi()
    {
        adventurers.text = Manager.AvailableAdventurers + " / " + Manager.Accommodation;
        spending.text = "x" + (Manager.Spending / 100f).ToString("0.00");
        
        satisfaction.SetBar(Manager.Satisfaction);
        effectiveness.SetBar(Manager.Effectiveness);
    }

    public void ToggleSize()
    {
        toggleSizeIcon.Rotate(0,0,180);
        StartCoroutine(ToggleRoutine(toggleSize.isOn));
    }

    public IEnumerator ToggleRoutine(bool dir)
    {
        toggleSize.enabled = false;
        for (int i = 0; i <= 30; i++)
        {
            float time = dir ? i / 30f : (30 - i)/30f; // invert for close

            smallPanel.anchoredPosition = new Vector2(Mathf.Lerp(-35, 30, time), smallPanel.localPosition.y);
            largePanelMask.sizeDelta = new Vector2(Mathf.Lerp(0, 135, time), Mathf.Lerp(160, 300, time));
            yield return null;
        }
        toggleSize.enabled = true;
        yield return null;
    }
}
