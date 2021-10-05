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
            _tokens = Manager.Requests.TokenCount(guild);
            Manager.Requests.Remove(guild);
            return true;
        }

        protected override string Description => customDescription != "" ?
            $"{Colors.GreenText}{customDescription}{Colors.EndText}" :
            $"{Colors.GreenText}{guild} request completed, {_tokens} token{(_tokens > 1 ? "s" :"")} rewarded.{Colors.EndText}";
    }
}
