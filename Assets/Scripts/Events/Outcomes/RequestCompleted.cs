using UnityEngine;
using UnityEngineInternal;
using Utilities;
using static Managers.GameManager;

namespace Events.Outcomes
{
    [CreateAssetMenu(fileName = "Request Completed Outcome", menuName = "Outcomes/Request Completed")]

    public class RequestCompleted : Outcome
    {
        public Guild guild;
    
        public override bool Execute()
        {
            Manager.Requests.Remove(guild);
            return true;
        }
    
        public override string Description
        {
            get
            {
                if (customDescription != "") return "<color=#007000ff>" + customDescription + "</color>";
                return "<color=#007000ff>" + guild + " Request Completed</color>";
            }
        }
    }
}
