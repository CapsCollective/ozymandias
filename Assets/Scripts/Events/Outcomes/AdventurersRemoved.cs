using Utilities;
using static Managers.GameManager;

namespace Events.Outcomes
{
    public class AdventurersRemoved : Outcome
    {
        public int count;
        // To shreds, you say?
        public bool kill; // If they move to the graveyard or just disappear
        public Guild guild;
        public bool anyGuild;


        protected override bool Execute()
        {
            int removable = anyGuild ?
                Manager.Adventurers.Available :
                Manager.Adventurers.GetCount(guild, true);
            
            if (removable <= count) return false;

            for (int i = 0; i < count; i++)
            {
                if (anyGuild) Manager.Adventurers.Remove(kill);
                else Manager.Adventurers.Remove(kill, guild);
            }
            return true;
        }

        protected override string Description => (
            customDescription != "" ? customDescription :
            $"{count} " +
            $"{(anyGuild ? "Adventurer".Pluralise(count) : String.GuildWithIcon(guild, count))} " +
            $"{(count == 1 ? "has" : "have")} " +
            $"{(kill ? "been struck down" : "fled the town")}."
        ).StatusColor(-1);
    }
}
