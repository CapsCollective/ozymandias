using Utilities;
using static Managers.GameManager;

namespace Events.Outcomes
{
    public class RequestCompleted : Outcome
    {
        public Guild guild;
        private int _tokens;

        protected override bool Execute()
        {
            if (!Manager.Requests.HasRequest(guild))
            {
                UnityEngine.Debug.LogWarning($"Events: {guild} has no active request to complete");
                return false;
            }
            _tokens = Manager.Requests.TokenCount(guild);
            Newspaper.OnNextClosed += () => Manager.Requests.Remove(guild);
            return true;
        }

        protected override string Description => (
            customDescription != "" ? customDescription :
            $"{String.GuildWithIcon(guild)} request completed, {_tokens} {"token".Pluralise(_tokens)} rewarded."
        ).StatusColor(1);
    }
}
