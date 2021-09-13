using Quests;

namespace Events.Outcomes
{
    public class QuestCompleted : Outcome
    {
        public Quest quest;

        private int _assigned;
        public override bool Execute()
        {
            _assigned = quest.AssignedCount;
            quest.Complete();
            return true;
        }

        public override string Description => "<color=#007000ff>Quest completed: " + quest.Title + ".\n" + 
                                              _assigned + " Adventurer" + (_assigned > 1 ? "s have" : " has") + " returned.</color>";
    }
}
