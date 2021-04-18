using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using static Managers.GameManager;

public class MainMenuEnvironmentController : MonoBehaviour
{
    [SerializeField] private Light sun;
    [SerializeField] private float speed = 2f;
    [SerializeField] private ParticleSystem glowflyPS;

    [SerializeField] private Gradient ambientGradient;
    [SerializeField] private Gradient sunColorGradient;
    [SerializeField] private Gradient skyColorGradient;
    [SerializeField] private Material skyMaterial;

    [SerializeField][Range(0, 1)]
    private float timer;

    private void Start()
    {
        RenderSettings.ambientLight = ambientGradient.Evaluate(0);
        RenderSettings.fogColor = sunColorGradient.Evaluate(0);
        skyMaterial.SetColor("_SkyColor", skyColorGradient.Evaluate(0));
        skyMaterial.SetColor("_HorizonColor", sunColorGradient.Evaluate(0));
    }

    private void Update()
    {
        timer += Time.deltaTime * speed;
        sun.transform.eulerAngles = Vector3.Lerp(new Vector3(50, -30, 0), new Vector3(50 + 360, -30, 0), timer);

        RenderSettings.ambientLight = ambientGradient.Evaluate(timer);
        RenderSettings.fogColor = sunColorGradient.Evaluate(timer);
        skyMaterial.SetColor("_SkyColor", skyColorGradient.Evaluate(timer));
        skyMaterial.SetColor("_HorizonColor", sunColorGradient.Evaluate(timer));

        if (timer >= 1)
            timer = 0;



        //Transform t = sun.transform;
        //t.DORotate(t.eulerAngles + new Vector3(360, 0, 0), sunSetTime, RotateMode.FastBeyond360).OnUpdate(() =>
        //{
        //    timer += Time.deltaTime / sunSetTime;
        //    sun.color = sunColorGradient.Evaluate(timer);
        //    RenderSettings.ambientLight = ambientGradient.Evaluate(timer);
        //    RenderSettings.fogColor = sunColorGradient.Evaluate(timer);
        //    RenderSettings.fogColor = sunColorGradient.Evaluate(timer);
        //    skyMaterial.SetColor("_Tint", skyColorGradient.Evaluate(timer));
        //}).OnComplete(Manager.NewTurn);
    }
}
