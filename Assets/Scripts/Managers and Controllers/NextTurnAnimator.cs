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
    
    [SerializeField] private JukeboxController jukebox;
    private float orig_angle;
    private float t = 0f;
    private float x = 0f;

    private Color ambCol;

    void Awake()
    {
        GameManager.OnNextTurn += OnNextTurn;
        orig_angle = sun.transform.eulerAngles.x;
    }

    private void OnDestroy()
    {
        GameManager.OnNextTurn -= OnNextTurn;
    }

    public void OnNextTurn()
    {
        ambCol = RenderSettings.ambientLight;
        StartCoroutine(AnimateSun());
        jukebox.StartNightAmbience();
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
            sun.transform.eulerAngles = new Vector3(x, sun.transform.rotation.y, sun.transform.rotation.z);
            RenderSettings.ambientLight = Color.Lerp(ambCol, ambCol * 0.25f, t / sunSetTime);
            yield return null;
        }
        t = 0f;
        Manager.NewTurn();
    }
}
