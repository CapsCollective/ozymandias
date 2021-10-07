using NaughtyAttributes;
using Quests;
using UnityEngine;
using static Managers.GameManager;

namespace Events.Outcomes
{
    public class QuestAdded : Outcome
    {
        public Quest quest;

        [Button]
        protected override bool Execute()
        {
            return Manager.Quests.Add(quest);
        }

        protected override string Description => customDescription != "" ? customDescription : $"New quest added: {quest.Title}.";
    }
}
