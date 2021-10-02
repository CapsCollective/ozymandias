﻿using Managers;
using UnityEngine;
using Utilities;
using static Managers.GameManager;

namespace Requests.Templates
{
    public sealed class KeepUpset : Request
    {
        public Guild targetGuild;
        public override string Description => $"Keep {targetGuild}s Upset for {Required} Turns";
        protected override int RequiredScaled => 3;

        public override void Start()
        {
            State.OnNextTurnEnd += CheckUpset;
        }
        
        public override void Complete()
        {
            State.OnNextTurnEnd -= CheckUpset;
        }

        private void CheckUpset()
        {
            Debug.Log(Manager.Stats.GetSatisfaction(targetGuild));
            if (Manager.Stats.GetSatisfaction(targetGuild) <= -5) Completed++;
            else Completed = 0;
        }
    }
}