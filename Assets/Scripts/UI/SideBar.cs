using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static GameManager;

public class SideBar : UiUpdater
{
    public TextMeshProUGUI
        adventurers,
        spending,
        adventurersLarge,
        spendingLarge,
        spendingModifier,
        effectivenessModifier,
        satisfactionModifier,
        overcrowdingModifier;

    public BarFill
        effectiveness,
        satisfaction,
        effectivenessLarge,
        satisfactionLarge,
        equipment,
        weaponry,
        magic,
        food,
        entertainment,
        luxury;

    public Toggle toggleSize;
    public RectTransform toggleSizeIcon;
    public RectTransform smallPanel;
    public RectTransform largePanelMask;
    
    // Update is called once per frame
    public override void UpdateUi()
    {
        adventurers.text = Manager.AvailableAdventurers + " / " + Manager.Accommodation;
        spending.text = "x" + (Manager.Spending / 100f).ToString("0.00");
        adventurersLarge.text = "Adventurers " + adventurers.text;
        spendingLarge.text = "Spending " + spending.text;

        int mod = Manager.modifiers[Metric.Spending];
        spendingModifier.gameObject.SetActive(mod != 0);
        spendingModifier.text = (mod > 0 ? "+" : "") + mod + " from event modifiers"; 
        
        mod = Manager.modifiers[Metric.Effectiveness];
        effectivenessModifier.gameObject.SetActive(mod != 0);
        effectivenessModifier.text = (mod > 0 ? "+" : "") + mod + " from event modifiers"; 
        
        mod = Manager.modifiers[Metric.Satisfaction];
        satisfactionModifier.gameObject.SetActive(mod != 0);
        satisfactionModifier.text = (mod > 0 ? "+" : "") + mod + " from event modifiers";

        mod = Manager.OvercrowdingMod;
        overcrowdingModifier.gameObject.SetActive(mod != 0);
        overcrowdingModifier.text = mod + " from overcrowding";

        effectiveness.SetBar(Manager.Effectiveness);
        satisfaction.SetBar(Manager.Satisfaction);
        effectivenessLarge.SetBar(Manager.Effectiveness);
        satisfactionLarge.SetBar(Manager.Satisfaction);
        equipment.SetBar(Manager.Equipment);
        weaponry.SetBar(Manager.Weaponry);
        magic.SetBar(Manager.Magic);
        food.SetBar(Manager.Food);
        entertainment.SetBar(Manager.Entertainment);
        luxury.SetBar(Manager.Luxury);
    }

    public void ToggleSize()
    {
        /*
        toggleSizeIcon.Rotate(0,0,180);
        StartCoroutine(ToggleRoutine(toggleSize.isOn));
        */
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
