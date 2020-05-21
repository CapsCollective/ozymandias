using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using UnityEngine;
using static GameManager;

[CreateAssetMenu]
public class StatChange : Outcome
{
    public enum StatToEffect
    {
        Chaos,
        Defense,
        Adventurers,
        Threat
    }

    public StatToEffect StatToChange;
    public int Amount;
    public int Turns;

    private int turnsLeft;
    //[SerializeField] private int turns;

    public override bool Execute()
    {
        turnsLeft = Turns;
        OnNewTurn += ProcessStatChange;
        ProcessStatChange();
        return true;
    }

    public void ProcessStatChange()
    {
        if (turnsLeft == 0)
        {
            OnNewTurn -= ProcessStatChange;
            //EventQueue.OnStatEffectComplete?.Invoke(this);
            return;
        }

        switch (StatToChange)
        {
            case StatToEffect.Chaos:
                Manager.ChaosMod += Amount;
                break;
            case StatToEffect.Defense:
                Manager.DefenseMod += Amount;
                break;
            case StatToEffect.Adventurers:
                Manager.AdventurersMod += Amount;
                break;
            case StatToEffect.Threat:
                Manager.ThreatMod += Amount;
                break;
        }
        
        Debug.Log($"{StatToChange} was changed by {Amount}. {turnsLeft} turns remaining.");
        turnsLeft--;
    }

    public override string GetOutcomeString()
    {
        if(OutcomeFlavourText == "")
            return "•" + StatToChange +" has been changed by " + Amount +" for " + turnsLeft + " turns";

        return OutcomeFlavourText;
    }
}
