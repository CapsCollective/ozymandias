using UnityEngine;

[CreateAssetMenu(fileName = "Debug Outcome", menuName = "Outcomes/Debug")]
public class DebugOutcome : Outcome
{
    [TextArea] public string debugText;

    public override bool Execute()
    {
        Debug.Log(debugText);
        return true;
    }
}
