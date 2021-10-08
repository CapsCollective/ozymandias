﻿using System.Collections.Generic;
using NaughtyAttributes;
using Quests;
using Utilities;
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

        private static readonly Dictionary<Location, string> LocationDescriptors = new Dictionary<Location, string>
        {
            { Location.Grid, "near your town" },
            { Location.Dock, "by the docks" },
            { Location.Forest, "by the forest path" },
            { Location.Mountains, "through the mountains" },
        };

        protected override string Description => customDescription != "" ? customDescription : $"New quest added {LocationDescriptors[quest.location]}: {quest.Title}.";
    }
}
