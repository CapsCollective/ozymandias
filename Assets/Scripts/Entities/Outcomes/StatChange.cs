using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using static GameManager;

[CreateAssetMenu(fileName = "Stat Change Outcome", menuName = "Outcomes/Stat Change")]
public class StatChange : Outcome
{

    public Metric statToChange;
    public int amount;
    public int turns;

    private int turnsLeft;
    //[SerializeField] private int turns;

    public override bool Execute(bool fromChoice)
    {
        turnsLeft = turns;
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

        Manager.modifiers[statToChange] += amount;

        if (turnsLeft == -1) return;
        turnsLeft--;
    }

    public override string Description
    {
        get
        {
            string color;
            if (statToChange == Metric.Threat && amount > 0 || statToChange != Metric.Threat && amount < 0) color = "#820000ff";
            else color = "#007000ff";
                
            if (customDescription != "") return "<color="+color+">" + customDescription + "</color>";
            string desc = "";
            if (amount > 0) desc += "<color="+color+">" + statToChange + " has increased by " + amount;
            else desc += "<color="+color+">" + statToChange + " has decreased by " + Mathf.Abs(amount);
            
            if (turnsLeft != -1) desc += " for " + turns + " turns.";
            return desc + "</color>";
        }
    }
}
