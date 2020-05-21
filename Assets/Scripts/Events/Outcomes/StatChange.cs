using System.Collections;
using System.Collections.Generic;
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


    public override bool Execute()
    {
        EventQueue.OnNewStatEffect?.Invoke(Instantiate(this));
        return true;
    }

    public void ProcessStatChange()
    {
        if(Turns == 0)
        {
            EventQueue.OnStatEffectComplete?.Invoke(this);
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
        
        Debug.Log($"{StatToChange} was changed by {Amount}. {Turns} turns remaining.");
        Turns--;
    }

    public override string GetOutcomeString()
    {
        if(OutcomeFlavourText != "")
            return StatToChange +" has been changed by " + Amount +" for " + Turns + " turns";

        return OutcomeFlavourText;
    }
}
