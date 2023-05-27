using NaughtyAttributes;
using Structures;
using Utilities;
using static Managers.GameManager;

namespace Events.Outcomes
{
    public class CardUnlocked : Outcome
    {
        public Blueprint blueprint;

        private bool _isAlreadyUnlocked;
        
        [Button]
        protected override bool Execute()
        {
            if (!blueprint) return false;
            
            _isAlreadyUnlocked = Manager.Cards.IsPlayable(blueprint);
            if (_isAlreadyUnlocked) Manager.Stats.Wealth += Manager.Stats.WealthPerTurn * 3;
            else Manager.Cards.Unlock(blueprint);
            return true;
        }

        protected override string Description => _isAlreadyUnlocked ?
            $"{blueprint.name.Pluralise()} already unlocked, have some money instead. +{Manager.Stats.WealthPerTurn * 3} Wealth ({String.StatIcon(Stat.Spending)})".StatusColor(1) :
            $"{blueprint.name.Pluralise()} have been unlocked.".StatusColor(1);
    }
}
