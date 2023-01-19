using NaughtyAttributes;
using System;
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

public class InputRumble : MonoBehaviour
{
    public static Action<RumbleType> OnPlayRumble;
    public static Action<RumbleType> OnPlayRumbleNoReset;
    public static Action OnStopRumble;

    [SerializeField] SerializedDictionary<RumbleType, RumblePattern> RumblePatterns = new SerializedDictionary<RumbleType, RumblePattern>();
    [SerializeField] private RumbleType rumbleTest;

    private RumbleType curRumb;
    private bool IsRumbling = false;
    private float timer = 999;
    private RumblePattern pattern = null;

    [Button("Test Rumble")]
    private void TestRumble()
    {
        //PlayRumble();
    }

    public void PlayRumble(RumbleType rumble)
    {
        pattern = RumblePatterns[rumble];
        timer = 0;
        IsRumbling = true;
    }

    public void StopRumble()
    {
        if (IsRumbling)
        {
            timer = 0;
            IsRumbling = false;
            InputSystem.ResetHaptics();
        }
    }

    private void Update()
    {
        Rumble();
    }

    private void Start()
    {
        OnPlayRumble += PlayRumble;
        OnStopRumble += StopRumble;
    }

    public void Rumble()
    {
        if (!IsRumbling) return;


        if (timer <= pattern.Length)
        {
            timer += Time.deltaTime;
            float lMotor = pattern.LowFreqMotor.Evaluate(timer / pattern.Length);
            float hMotor = pattern.HighFreqMotor.Evaluate(timer / pattern.Length);
            Gamepad.current.SetMotorSpeeds(lMotor, hMotor);
        }
        else
        {
            InputSystem.ResetHaptics();
            IsRumbling = false;
            timer = 0;
        }
        Debug.Log(timer);
    }
}
