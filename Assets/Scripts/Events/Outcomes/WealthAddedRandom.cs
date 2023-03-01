using System.Collections.Generic;
using UnityEngine;
using Utilities;
using static Managers.GameManager;
using String = Utilities.String;

namespace Events.Outcomes
{
    public class WealthAddedRandom : Outcome
    {
        private readonly List<float> TurnsWorth = new List<float>{0.1f, 0.3f, 0.8f, 1.5f, 4f};
        private readonly List<string> OutcomeDescriptions = new List<string>
        {
            "You get unlucky this time...",
            "You only just make your money back this time...",
            "You get a decent winnings...",
            "You get a massive payout!",
            "You hit the jackpot!"
        };

        private int Amount => Mathf.RoundToInt(Manager.Stats.WealthPerTurn * TurnsWorth[_random]);
        private int _random;
        protected override bool Execute()
        {
            _random = Random.Range(0, 5);
            Manager.Stats.Wealth += Mathf.RoundToInt(Manager.Stats.WealthPerTurn * TurnsWorth[_random]);
            return true;
        }

        protected override string Description => (
            customDescription != "" ? customDescription :
            $"{OutcomeDescriptions[_random]} {Amount.WithSign()} Wealth ({String.StatIcon(Stat.Spending)})"
        ).StatusColor(Amount);
    }
}
