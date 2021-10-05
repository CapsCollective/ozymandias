using Requests.Templates;
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
        protected override string Description => customDescription != "" ? customDescription : $"{request.guild} request added: {request.Description}.";
    }
}
