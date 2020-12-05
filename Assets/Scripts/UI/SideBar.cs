#pragma warning disable 0649
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Managers.GameManager;

namespace UI
{
    public class SideBar : UiUpdater
    {
        [SerializeField] private RectTransform parent, mask, layout;
    
        [SerializeField] private TextMeshProUGUI
            spending,
            spendingModifier,
            effectivenessModifier,
            satisfactionModifier,
            overcrowdingModifier,
            lowThreatModifier;

        [SerializeField] private BarFill
            effectiveness,
            satisfaction,
            equipment,
            weaponry,
            magic,
            food,
            entertainment,
            luxury;

        // Update is called once per frame
        protected override void UpdateUi()
        {
            spending.text = "Spending: x" + (Manager.Spending / 100f).ToString("0.00");
        
            int mod = Manager.ModifiersTotal[Metric.Spending];
            spendingModifier.gameObject.SetActive(mod != 0);
            spendingModifier.text = (mod > 0 ? "<color=green>+" : "<color=red>") + mod + "%</color> from event modifiers"; 
        
            mod = Manager.ModifiersTotal[Metric.Effectiveness];
            effectivenessModifier.gameObject.SetActive(mod != 0);
            effectivenessModifier.text = (mod > 0 ? "<color=green>+" : "<color=red>") + mod + "%</color> from event modifiers";
        
            mod = Manager.ModifiersTotal[Metric.Satisfaction];
            satisfactionModifier.gameObject.SetActive(mod != 0);
            satisfactionModifier.text = (mod > 0 ? "<color=green>+" : "<color=red>") + mod + "%</color> from event modifiers";

            mod = Manager.OvercrowdingMod;
            overcrowdingModifier.gameObject.SetActive(mod != 0);
            overcrowdingModifier.text = "<color=red>"+ mod + "%</color> from overcrowding";
        
            mod = Manager.LowThreatMod;
            lowThreatModifier.gameObject.SetActive(mod != 0);
            lowThreatModifier.text = "<color=red>"+ mod + "%</color> from a lack of adventure";

            effectiveness.SetBar(Manager.Effectiveness);
            satisfaction.SetBar(Manager.Satisfaction);
            equipment.SetBar(Manager.Equipment);
            weaponry.SetBar(Manager.Weaponry);
            magic.SetBar(Manager.Magic);
            food.SetBar(Manager.Food);
            entertainment.SetBar(Manager.Entertainment);
            luxury.SetBar(Manager.Luxury);

            LayoutRebuilder.ForceRebuildLayoutImmediate(layout);
            Rect rect = layout.rect;
            parent.sizeDelta = new Vector2(150, rect.height);
            mask.sizeDelta = new Vector2(150, rect.height);
        }
    }
}
