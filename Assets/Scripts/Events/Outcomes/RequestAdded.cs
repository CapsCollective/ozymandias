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
            if (Manager.Requests.HasRequest(request.guild))
            {
                UnityEngine.Debug.LogWarning($"{request.guild} already has an active request");
                return false;
            }
            Manager.Requests.Add(request);
            return true;
        }
        protected override string Description => customDescription != "" ? customDescription : $"{String.GuildWithIcon(request.guild)} request added: {request.Description}.";
    }
}
