using static Managers.GameManager;

namespace Events.Outcomes
{
    public class ChainEvent : Outcome
    {
        public Event next;
        public bool toFront;

        protected override bool Execute()
        {
            Manager.EventQueue.Add(next, toFront);
            return true;
        }
    }
}
