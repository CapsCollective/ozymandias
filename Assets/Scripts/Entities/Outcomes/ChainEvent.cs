using UnityEngine;
using static Managers.GameManager;

namespace Entities.Outcomes
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
