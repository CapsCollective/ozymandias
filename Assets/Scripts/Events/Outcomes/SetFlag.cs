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
            UnityEngine.Debug.Log($"Setting flag {flag} to {value}");
            Manager.EventQueue.Flags[flag] = value;
            return true;
        }
    }
}
