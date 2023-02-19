using Requests.Templates;
using Utilities;
using static Managers.GameManager;

namespace Events.Outcomes
{
    public class RequestAdded : Outcome
    {
        public Request request;

        protected override bool Execute()
        {
            Manager.Requests.Add(request);
            return true;
        }
        protected override string Description => customDescription != "" ? customDescription : $"{String.GuildWithIcon(request.guild)} request added: {request.Description}.";
    }
}
