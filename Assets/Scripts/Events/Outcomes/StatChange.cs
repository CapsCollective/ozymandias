using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class StatChange : Outcome
{

    public Metric MetricAffected;
    public int Amount;
    public int Turns;

    private bool processedThisTurn = false;

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

        Debug.Log("Do the stat changing here");
        Turns--;

        processedThisTurn = true;
    }
}
