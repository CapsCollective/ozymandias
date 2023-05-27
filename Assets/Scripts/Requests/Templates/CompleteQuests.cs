using Quests;

namespace Requests.Templates
{
    public sealed class CompleteQuests : Request
    {
        public override string Description => $"Complete {Required} Quests";
        protected override int RequiredScaled => 1 + Tokens;

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
