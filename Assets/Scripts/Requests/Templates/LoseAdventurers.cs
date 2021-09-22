using Adventurers;
using UnityEngine;
using Utilities;

namespace Requests.Templates
{
    public sealed class LoseAdventurers : Request
    {
        public Guild? targetGuild;
        public bool requireKill;
        public override string Description => $"Kill {Required} {(targetGuild != null ? targetGuild.ToString() : "adventurer")}s";
        protected override int RequiredScaled => 3;

        public override void Start()
        {
            Adventurers.Adventurers.OnAdventurerRemoved += CheckRemove;
        }
        
        public override void Complete()
        {
            Adventurers.Adventurers.OnAdventurerRemoved -= CheckRemove;
        }

        private void CheckRemove(Adventurer adventurer, bool kill)
        {
            if (adventurer.guild == targetGuild && (!requireKill || kill)) Completed++;
        }
    }
}
