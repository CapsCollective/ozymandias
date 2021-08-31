using Adventurers;
using UnityEngine;

namespace Requests.Templates
{
    [CreateAssetMenu(fileName = "Attract Adventurers", menuName = "Requests/Attract Adventurers")]
    public sealed class AttractAdventurers : Request
    {
        public override string Description => $"Attract {Required} {guild}s";
        protected override int RequiredScaled => 3;

        public override void Start()
        {
            Adventurers.Adventurers.OnAdventurerJoin += CheckJoin;
        }
        
        public override void Complete()
        {
            Adventurers.Adventurers.OnAdventurerJoin -= CheckJoin;
        }

        private void CheckJoin(Adventurer adventurer)
        {
            if (adventurer.guild == guild) Completed++;
        }
    }
}
