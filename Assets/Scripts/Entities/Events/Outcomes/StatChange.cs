using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using static GameManager;

[CreateAssetMenu(fileName = "Stat Change Outcome", menuName = "Outcomes/Stat Change")]
public class StatChange : Outcome
{

    public Metric StatToChange;
    public int Amount;
    public int Turns;

    private int turnsLeft;
    //[SerializeField] private int turns;

    public override bool Execute(bool fromChoice)
    {
        turnsLeft = Turns;
        OnNewTurn += ProcessStatChange;
        if (fromChoice) ProcessStatChange();
        return true;
    }
    
    public void ProcessStatChange()
    {
        if (turnsLeft == 0)
        {
            OnNewTurn -= ProcessStatChange;
            return;
        }

        Manager.modifiers[StatToChange] += Amount;

        if (turnsLeft == -1) return;
        turnsLeft--;
    }

    public override string Description
    {
        get
        {
            string color;
            if (StatToChange == Metric.Threat && Amount > 0 || StatToChange != Metric.Threat && Amount < 0) color = "#820000ff";
            else color = "#007000ff";
                
            if (customDescription != "") return "<color="+color+">" + customDescription + "</color>";
            string desc = "";
            if (Amount > 0) desc += "<color="+color+">" + StatToChange + " has increased by " + Amount;
            else desc += "<color="+color+">" + StatToChange + " has decreased by " + Amount;
            
            if (turnsLeft != -1) desc += " for " + Turns + " turns.";
            return desc + "</color>";
        }
    }
}
