using UnityEngine;
using static Managers.GameManager;

namespace Events.Outcomes
{
    [CreateAssetMenu(fileName = "Threat Added Outcome", menuName = "Outcomes/Threat Added")]
    public class ThreatAdded : Outcome
    {
        public int amount;
    
        public override bool Execute()
        {
            Manager.Stats.BaseThreat += amount;
            return true;
        }
    
        public override string Description => "<color=#820000ff>" + amount + " threat to the town</color>";
    }
}
