using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using UnityEngine;
using static GameManager;

[CreateAssetMenu]
public class StatChange : Outcome
{

    public Metric StatToChange;
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

        Manager.modifiers[StatToChange] += Amount;

        Debug.Log($"{StatToChange} was changed by {Amount}. {turnsLeft} turns remaining.");
        turnsLeft--;
    }

    public override string Description
    {
        get
        {
            if (customDescription != "") return customDescription;
            if (Amount > 0) return StatToChange + " has increased by " + Amount + " for " + turnsLeft + " turns";
            return StatToChange + " has decreased by " + Amount + " for " + turnsLeft + " turns";
        }
    }
}
