using Managers;
using UnityEngine;
using Utilities;
using static Managers.GameManager;

namespace Events.Outcomes
{
    [CreateAssetMenu(fileName = "Stat Change Outcome", menuName = "Outcomes/Stat Change")]
    public class ModifierAdded : Outcome
    {

        public Stat statToChange;
        public int amount;
        public int turns;
        public string reason;
    
        public override bool Execute()
        {
            if (!Manager.Stats.Modifiers.ContainsKey(statToChange)) return false;
        
            Manager.Stats.Modifiers[statToChange].Add(new Stats.Modifier
            {
                amount = amount,
                turnsLeft = turns,
                reason = reason
            });
            Manager.Stats.ModifiersTotal[statToChange] += amount;
            return true;
        }
    
        public override string Description
        {
            get
            {
                string color = amount > 0 ? Colors.GreenText : Colors.RedText;
                if (customDescription != "") return $"<color={color}>{customDescription}</color>";
                
                string desc = "<color="+color+">" + statToChange + ((int)statToChange < 5 ? " Satisfaction" : "");
                
                if (amount > 0) desc += " has increased by " + amount;
                else desc += " has decreased by " + Mathf.Abs(amount);
            
                if (turns != -1) desc += " for " + turns + " turns " + reason + ".";
                return desc + "</color>";
            }
        }
    }
}
