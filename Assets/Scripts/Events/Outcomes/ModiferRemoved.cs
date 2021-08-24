using Managers;
using UnityEngine;
using Utilities;
using static Managers.GameManager;

namespace Events.Outcomes
{
    [CreateAssetMenu(fileName = "Stat Change Outcome", menuName = "Outcomes/Stat Change")]
    public class ModiferRemoved : Outcome
    {
        public int idToRemove;
        
        public override bool Execute(bool fromChoice)
        {
            //TODO: Remove for permanent modifiers
            return true;
        }
    
        public override string Description
        {
            get
            {
                return "TODO";
            }
        }
    }
}
