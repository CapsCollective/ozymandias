using UnityEngine;
using NaughtyAttributes;
using static Managers.GameManager;

[CreateAssetMenu(fileName = "New Quest Outcome", menuName = "Outcomes/New Quest")]
public class NewQuest : Outcome
{
    public Quest quest;

    [Button]
    public override bool Execute()
    {
        return Manager.Quests.Add(quest);
    }
    
    public override string Description
    {
        get
        {
            if (customDescription != "") return "<color=#007000ff>" + customDescription + "</color>";
            return "<color=#007000ff>New quest added: " + quest.title + "</color>";
        }
    }
}
