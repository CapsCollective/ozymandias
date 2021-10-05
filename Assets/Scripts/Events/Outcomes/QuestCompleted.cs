using Quests;
using Utilities;

namespace Events.Outcomes
{
    public class QuestCompleted : Outcome
    {
        public Quest quest;

        private int _assigned;

        protected override bool Execute()
        {
            _assigned = quest.AssignedCount;
            quest.Complete();
            return true;
        }
        
        protected override string Description => 
            $"{Colors.GreenText}Quest completed: {quest.Title}. " +
            $"{_assigned} Adventurer{(_assigned > 1 ? "s have" : " has")} returned.{Colors.EndText}";
    }
}
