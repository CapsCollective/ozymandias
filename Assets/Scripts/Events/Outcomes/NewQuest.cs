using NaughtyAttributes;
using Quests;
using UnityEngine;
using static GameState.GameManager;

namespace Events.Outcomes
{
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
                return "<color=#007000ff>New quest added: " + quest.Title + "</color>";
            }
        }
    }
}
