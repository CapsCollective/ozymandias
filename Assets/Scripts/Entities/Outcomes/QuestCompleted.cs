using static Managers.GameManager;

namespace Entities.Outcomes
{
    public class QuestCompleted : Outcome
    {
        public Quest quest;
    
        public override bool Execute()
        {
            return Manager.Quests.Remove(quest);
        }

        public override string Description => "<color=#007000ff>Quest completed: " + quest.Title + ".\n" + 
                                              quest.adventurers + " Adventurer" + (quest.adventurers > 1 ? "s have" : " has") + " returned.</color>";
    }
}
