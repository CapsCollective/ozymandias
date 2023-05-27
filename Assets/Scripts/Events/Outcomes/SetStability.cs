using UnityEngine;
using Utilities;
using static Managers.GameManager;
using String = Utilities.String;

namespace Events.Outcomes
{
    public class SetStability : Outcome
    {
        public int amount;

        protected override bool Execute()
        {
            Manager.Stats.Stability = amount;
            return true;
        }

        protected override string Description => (
            customDescription != "" ? customDescription :
            $"Town Stability now at {amount}%"
        ).StatusColor(amount - 50);
    }
}
