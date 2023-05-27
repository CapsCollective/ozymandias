using UnityEngine;
using Utilities;
using static Managers.GameManager;
using String = Utilities.String;

namespace Events.Outcomes
{
    public class ThreatAdded : Outcome
    {
        public int baseAmount;
        
        private int Amount => Mathf.FloorToInt(Mathf.Sign(baseAmount) * (Mathf.Abs(baseAmount) + (Manager.Stats.TurnCounter / ThreatScaling)));

        protected override bool Execute()
        {
            Manager.Stats.BaseThreat += Amount;
            return true;
        }

        protected override string Description => $"{Amount.WithSign()} {String.StatWithIcon(Stat.Threat)} to the town.".StatusColor(-Amount);
    }
}
