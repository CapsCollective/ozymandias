using Utilities;
using static Managers.GameManager;

namespace Events.Outcomes
{
    public class AdventurersRemoved : Outcome
    {
        public int count;
        // To shreds, you say?
        public bool kill; // If they move to the graveyard or just disappear

        protected override bool Execute()
        {
            if (Manager.Adventurers.Available <= count) return false;

            for (int i = 0; i < count; i++)
            {
                if (!Manager.Adventurers.Remove(kill)) return false;
            }
            return true;
        }

        protected override string Description => (
            customDescription != "" ? customDescription :
            $"{count} {"Adventurer".Pluralise(count)} {(count == 1 ? "has" : "have")} {(kill ? "been struck down" : "fled the town")}."
        ).StatusColor(-1);
    }
}
