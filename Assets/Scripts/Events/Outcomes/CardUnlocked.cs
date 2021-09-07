using NaughtyAttributes;
using Structures;
using UnityEngine;
using static Managers.GameManager;

namespace Events.Outcomes
{
    [CreateAssetMenu(fileName = "Building Unlock", menuName = "Outcomes/Building Unlock")]
    public class CardUnlocked : Outcome
    {
        public Blueprint blueprint;

        [Button]
        public override bool Execute()
        {
            return blueprint && Manager.Cards.Unlock(blueprint);
        }

        public override string Description => "<color=#007000ff>Building Type Unlocked: " + blueprint.name + "!</color>";
    }
}
