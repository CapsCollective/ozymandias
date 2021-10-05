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

        protected override string Description => $"{Colors.RedText}+{Amount} threat to the town.{Colors.EndText}";
    }
}
