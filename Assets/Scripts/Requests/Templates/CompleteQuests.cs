using Quests;
using Utilities;

namespace Requests.Templates
{
    public sealed class CompleteQuests : Request
    {
        public override string Description => $"Complete {Required} {String.Pluralise("Quest", Required)}";
        protected override int RequiredScaled => Tokens;

        public override void Start()
        {
            Quests.Quests.OnQuestCompleted += CheckCompleted;
        }
        
        public override void Complete()
        {
            Quests.Quests.OnQuestCompleted -= CheckCompleted;
        }

        private void CheckCompleted(Quest quest)
        {
            Completed++;
        }
    }
}
