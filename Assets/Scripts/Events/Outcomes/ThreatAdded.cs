using UnityEngine;
using Utilities;
using static Managers.GameManager;

namespace Events.Outcomes
{
    public class ThreatAdded : Outcome
    {
        public int baseAmount;

        private int Amount => Mathf.RoundToInt(baseAmount * (1 + Manager.Stats.TurnCounter / ThreatScaling));

        protected override bool Execute()
        {
            Manager.Stats.BaseThreat += Amount;
            return true;
        }

        protected override string Description => $"+{Amount} {String.StatWithIcon(Stat.Threat)} to the town.".StatusColor(-1);
    }
}
