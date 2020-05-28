using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Debug Outcome", menuName = "Outcomes/Debug")]
public class DebugOutcome : Outcome
{
    [TextArea]
    public string DebugText;

    public override bool Execute()
    {
        Debug.Log(DebugText);
        return true;
    }
}
