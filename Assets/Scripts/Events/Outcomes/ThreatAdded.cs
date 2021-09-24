using UnityEngine;
using static Managers.GameManager;

namespace Events.Outcomes
{
    [CreateAssetMenu(fileName = "Threat Added Outcome", menuName = "Outcomes/Threat Added")]
    public class ThreatAdded : Outcome
    {
        public int baseAmount;

        private int Amount => Mathf.RoundToInt(baseAmount * (1 + Manager.Stats.TurnCounter / ThreatScaling));
    
        public override bool Execute()
        {
            Manager.Stats.BaseThreat += Amount;
            return true;
        }
    
        public override string Description => "<color=#820000ff>" + Amount + " threat to the town</color>";
    }
}
