using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static GameManager;

public class SideBar : UiUpdater
{
    public RectTransform parent, mask, layout;
    
    public TextMeshProUGUI
        adventurers,
        spending,
        spendingModifier,
        effectivenessModifier,
        satisfactionModifier,
        overcrowdingModifier;

    public BarFill
        effectiveness,
        satisfaction,
        equipment,
        weaponry,
        magic,
        food,
        entertainment,
        luxury;

    // Update is called once per frame
    public override void UpdateUi()
    {
        bool overCapacity = Manager.AvailableAdventurers - Manager.Accommodation > 0;
        adventurers.text = "Adventurers: " + (overCapacity ? "<color=red>" : "") + Manager.AvailableAdventurers + (overCapacity ? "</color>" : "") + " / " + Manager.Accommodation;
        spending.text = "Spending: x" + (Manager.Spending / 100f).ToString("0.00");
        
        int mod = Manager.modifiers[Metric.Spending];
        spendingModifier.gameObject.SetActive(mod != 0);
        spendingModifier.text = (mod > 0 ? "<color=green>+" : "<color=red>") + mod + "%</color> from event modifiers"; 
        
        mod = Manager.modifiers[Metric.Effectiveness];
        effectivenessModifier.gameObject.SetActive(mod != 0);
        effectivenessModifier.text = (mod > 0 ? "<color=green>+" : "<color=red>") + mod + "%</color> from event modifiers";
        
        mod = Manager.modifiers[Metric.Satisfaction];
        satisfactionModifier.gameObject.SetActive(mod != 0);
        satisfactionModifier.text = (mod > 0 ? "<color=green>+" : "<color=red>") + mod + "%</color> from event modifiers";

        mod = Manager.OvercrowdingMod;
        overcrowdingModifier.gameObject.SetActive(mod != 0);
        overcrowdingModifier.text = "<color=red>"+ mod + "%</color> from overcrowding";

        effectiveness.SetBar(Manager.Effectiveness);
        satisfaction.SetBar(Manager.Satisfaction);
        equipment.SetBar(Manager.Equipment);
        weaponry.SetBar(Manager.Weaponry);
        magic.SetBar(Manager.Magic);
        food.SetBar(Manager.Food);
        entertainment.SetBar(Manager.Entertainment);
        luxury.SetBar(Manager.Luxury);

        LayoutRebuilder.ForceRebuildLayoutImmediate(layout);
        parent.sizeDelta = new Vector2(150, layout.rect.height);
        mask.sizeDelta = new Vector2(150, layout.rect.height);
    }
}
