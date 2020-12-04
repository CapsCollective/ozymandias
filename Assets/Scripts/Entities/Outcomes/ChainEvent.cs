using UnityEngine;
using static GameManager;

[CreateAssetMenu(fileName = "Chain Event Outcome", menuName = "Outcomes/Chain Event")]
public class ChainEvent : Outcome
{
    public Event next;
    public bool toFront;
    public override bool Execute()
    {
        Manager.Events.Add(next, toFront);
        return true;
    }
}
