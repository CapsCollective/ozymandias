
namespace Events.Outcomes
{
    public class Debug : Outcome
    {
        protected override bool Execute()
        {
            UnityEngine.Debug.Log(Description);
            return true;
        }
    }
}
