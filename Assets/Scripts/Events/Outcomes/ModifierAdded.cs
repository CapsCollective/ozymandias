using Managers;
using UnityEngine;
using Utilities;
using static Managers.GameManager;

namespace Events.Outcomes
{
    public class ModifierAdded : Outcome
    {

        public Stat statToChange;
        public int amount;
        public int turns;
        public string reason;

        protected override bool Execute()
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

        protected override string Description => (
            customDescription != "" ? customDescription :
            $"{String.StatWithIcon(statToChange)} has {(amount > 0 ? "increased" : "decreased")} by {Mathf.Abs(amount)}" +
            $" for {turns} turns {reason}.".Conditional(turns != -1)
        ).StatusColor(amount);
    }
}
