using Utilities;
using static Managers.GameManager;

namespace Events.Outcomes
{
    public class SetFlag : Outcome
    {
        public Flag flag;
        public bool value;

        protected override bool Execute()
        {
            if (Manager.EventQueue.Flags[flag] == value)
            {
                UnityEngine.Debug.LogWarning($"Flag {flag} already set to {value}");
                return false;
            }
            
            UnityEngine.Debug.Log($"Setting flag {flag} to {value}");
            Manager.EventQueue.Flags[flag] = value;
            return true;
        }
    }
}
