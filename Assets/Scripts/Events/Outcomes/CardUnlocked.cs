using NaughtyAttributes;
using Structures;
using Utilities;
using static Managers.GameManager;

namespace Events.Outcomes
{
    public class CardUnlocked : Outcome
    {
        public Blueprint blueprint;

        [Button]
        protected override bool Execute()
        {
            return blueprint && Manager.Cards.Unlock(blueprint);
        }

        protected override string Description => $"{Colors.GreenText}" +
                                                 $"{String.Pluralise(blueprint.name)}" +
                                                 $" have been unlocked.{Colors.EndText}";
    }
}
