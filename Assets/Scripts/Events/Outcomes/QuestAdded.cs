using System.Collections.Generic;
using Quests;
using Utilities;
using static Managers.GameManager;

namespace Events.Outcomes
{
    public class QuestAdded : Outcome
    {
        public Quest quest;

        protected override bool Execute()
        {
            if (Manager.Quests.IsActive(quest))
            {
                UnityEngine.Debug.LogWarning("Events: Quest already active - " + quest.name);
                return false;
            }
            Newspaper.OnNextClosed += () => Manager.Quests.Add(quest);
            return true;
        }
        
        private static readonly Dictionary<Location, string> LocationDescriptors = new Dictionary<Location, string>
        {
            { Location.Grid, "near your town" },
            { Location.Dock, "by the docks" },
            { Location.Forest, "by the forest path" },
            { Location.Mountains, "through the mountains" },
        };

        protected override string Description => (
            customDescription != "" ? customDescription :
            $"New {(quest.IsRadiant ? "enemy camp" : "quest")} added {LocationDescriptors[quest.location]}: {quest.Title}." +
            $"\n+1 {String.StatWithIcon(Stat.Threat)} per turn until cleared.".Conditional(quest.IsRadiant)
        ).StatusColor(quest.IsRadiant ? -1 : 0);
    }
}
