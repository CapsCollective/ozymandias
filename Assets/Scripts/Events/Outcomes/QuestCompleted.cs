using Quests;
using Utilities;

namespace Events.Outcomes
{
    public class QuestCompleted : Outcome
    {
        public Quest quest;

        private int _assigned, _threat;

        protected override bool Execute()
        {
            if (!quest.IsActive) return false;
            
            _assigned = quest.AssignedCount;
            if (quest.Structure) _threat = quest.Structure.SectionCount;
            quest.Complete();
            return true;
        }
        
        protected override string Description => 
            $"{Colors.GreenText}Quest completed: {quest.Title}." +
            $"\nThreat reduced by {_threat} and ".Conditional(quest.IsRadiant) +
            $"{_assigned} {"Adventurer".Pluralise(_assigned)} " +
            $"{(_assigned == 1 ? "has" : "have")} " +
            $"returned.{Colors.EndText}";
    }
}
