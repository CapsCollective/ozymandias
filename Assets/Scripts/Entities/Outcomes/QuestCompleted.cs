using System.Collections.Generic;
using static QuestMapController;

public class QuestCompleted : Outcome
{
    public Quest quest;
    
    public override bool Execute()
    {
        foreach (var adventurer in quest.assigned) adventurer.assignedQuest = null;
        quest.assigned = new List<Adventurer>();
        
        return QuestMap.RemoveQuest(quest);
    }

    public override string Description => "<color=#007000ff>Quest completed: " + quest.title + ".\n" + 
    quest.adventurers + " Adventurer" + (quest.adventurers > 1 ? "s have" : " has") + " returned.</color>";
}