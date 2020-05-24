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

    public override bool Execute()
    {
        turnsLeft = Turns;
        OnNewTurn += ProcessStatChange;
        SceneManager.activeSceneChanged += HandleSceneChange;
        ProcessStatChange();
        return true;
    }

    private void HandleSceneChange(Scene a, Scene b)
    { 
        OnNewTurn -= ProcessStatChange;
        SceneManager.activeSceneChanged -= HandleSceneChange;
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
