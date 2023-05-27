using UnityEngine;
using Utilities;
using static Managers.GameManager;
using String = Utilities.String;

namespace Events.Outcomes
{
    public class WealthAdded : Outcome
    {
        public float turnsWorth;

        private int Amount => Mathf.RoundToInt(Manager.Stats.WealthPerTurn * turnsWorth);
        protected override bool Execute()
        {
            Manager.Stats.Wealth += Mathf.RoundToInt(Manager.Stats.WealthPerTurn * turnsWorth);
            return true;
        }

        protected override string Description => (
            customDescription != "" ? customDescription :
            $"{Amount.WithSign()} Wealth ({String.StatIcon(Stat.Spending)})"
        ).StatusColor(Amount);
    }
}
