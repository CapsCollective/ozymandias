
namespace Events.Outcomes
{
    public class ModifierRemoved : Outcome
    {
        public int idToRemove;
        
        public override bool Execute(bool fromChoice)
        {
            //TODO: Remove for permanent modifiers
            return true;
        }

        protected override string Description
        {
            get
            {
                return "TODO";
            }
        }
    }
}
