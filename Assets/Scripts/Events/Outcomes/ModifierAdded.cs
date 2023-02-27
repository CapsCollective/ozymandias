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
            if (!Manager.Stats.Modifiers.ContainsKey(statToChange))
            {
                UnityEngine.Debug.LogWarning($"{statToChange} is not included in modifiers");
                return false;
            }
        
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
            $"{amount.WithSign()} {String.StatWithIcon(statToChange)}" +
            $" for {turns} turns {reason}.".Conditional(turns != -1)
        ).StatusColor(amount);
    }
}
