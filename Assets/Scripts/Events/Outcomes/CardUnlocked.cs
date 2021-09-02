using NaughtyAttributes;
using UnityEngine;
using static Managers.GameManager;

namespace Events.Outcomes
{
    [CreateAssetMenu(fileName = "Building Unlock", menuName = "Outcomes/Building Unlock")]
    public class CardUnlocked : Outcome
    {
        public GameObject building;

        [Button]
        public override bool Execute()
        {
            return building && Manager.Cards.Unlock(building);
        }

        public override string Description => "<color=#007000ff>Building Type Unlocked: " + building.name + "!</color>";
    }
}
