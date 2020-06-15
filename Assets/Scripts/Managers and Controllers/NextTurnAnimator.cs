using System.Collections;
using System.Collections.Generic;
using Managers_and_Controllers;
using UnityEngine;
using static GameManager;

public class NextTurnAnimator : MonoBehaviour
{
    public Light sun;
    public float sunSetTime = 2f;
    public ParticleSystem glowflyPS;
    private float orig_angle;
    private Vector3 origSunPos;
    private float t = 0f;
    private float x = 0f;

    private Color ambCol;

    void Awake()
    {
        GameManager.OnNextTurn += OnNextTurn;
        origSunPos = sun.transform.eulerAngles;
        orig_angle = origSunPos.x;
    }

    public void OnNextTurn()
    {
        ambCol = RenderSettings.ambientLight;
        StartCoroutine(AnimateSun());
    }

    public IEnumerator AnimateSun()
    {
        while (t < sunSetTime)
        {
            t += Time.deltaTime;
            x = Mathf.Lerp(orig_angle, orig_angle + 360f, t / sunSetTime);
            if(!glowflyPS.isEmitting&& (x>170 || x < 10))
            {
                glowflyPS.Play();
                RenderSettings.ambientLight = Color.Lerp(ambCol * 0.25f, ambCol, t / sunSetTime);
            }
            sun.transform.eulerAngles = new Vector3(x, origSunPos.y, origSunPos.z);
            if(x >150 && x < 210)
            {
                RenderSettings.ambientLight = Color.Lerp(ambCol, ambCol * 0.25f, t / sunSetTime);
            }
            else if(x > 330 && x <390f)
            {
                RenderSettings.ambientLight = Color.Lerp(0.25f*ambCol, ambCol, t / sunSetTime);
            }
            
            yield return null;
        }
        t = 0f;
        sun.transform.eulerAngles = new Vector3(orig_angle, origSunPos.y, origSunPos.z);
        Manager.NewTurn();
    }
}
