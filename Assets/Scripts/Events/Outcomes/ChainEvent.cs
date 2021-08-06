using UnityEngine;
using static GameState.GameManager;

namespace Events.Outcomes
{
    [CreateAssetMenu(fileName = "Chain Event Outcome", menuName = "Outcomes/Chain Event")]
    public class ChainEvent : Outcome
    {
        public Event next;
        public bool toFront;
        public override bool Execute()
        {
            Manager.EventQueue.Add(next, toFront);
            return true;
        }
    }
}
