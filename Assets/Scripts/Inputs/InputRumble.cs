using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Utilities;

[System.Serializable]
public class RumblePattern
{
    public float Length;
    public AnimationCurve LowFreqMotor;
    public AnimationCurve HighFreqMotor;
}

public enum RumbleType
{
    PlaceBuilding,
    Invalid,
    DestroyBuilding,

}

[System.Serializable]
public class InputRumble
{
    public SerializedDictionary<RumbleType, RumblePattern> RumblePatterns = new SerializedDictionary<RumbleType, RumblePattern>();
    public bool IsRumbling;

    public IEnumerator Rumble(RumbleType pattern)
    {
        RumblePattern curPattern = RumblePatterns[pattern];

        IsRumbling = true;
        for (float i = 0; i < curPattern.Length; i += Time.deltaTime)
        {
            float lMotor = curPattern.LowFreqMotor.Evaluate(i / curPattern.Length);
            float hMotor = curPattern.HighFreqMotor.Evaluate(i / curPattern.Length);
            Gamepad.current.SetMotorSpeeds(lMotor, hMotor);
            yield return null;
        }
        InputSystem.ResetHaptics();
        IsRumbling = false;
    }
}
