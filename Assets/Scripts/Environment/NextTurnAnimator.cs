using DG.Tweening;
using Managers;
using UnityEngine;
using static Managers.GameManager;

namespace Environment
{
    public class NextTurnAnimator : MonoBehaviour
    {
        [SerializeField] private Light sun;
        [SerializeField] private float sunSetTime = 2f;
        [SerializeField] private ParticleSystem glowflyPS;

        [SerializeField] private Gradient ambientGradient;   
        [SerializeField] private Gradient sunColorGradient;
        [SerializeField] private Gradient skyColorGradient;
        [SerializeField] private Material skyMaterial;

        private void Awake()
        {
            GameManager.OnNextTurn += OnNextTurn;
        }

        private void OnNextTurn()
        {
            glowflyPS.Play();
            float timer = 0;
            Transform t = sun.transform;
            t.DORotate(t.eulerAngles + new Vector3(360,0,0), sunSetTime, RotateMode.FastBeyond360).OnUpdate(() =>
            {
                timer += Time.deltaTime / sunSetTime;
                sun.color = sunColorGradient.Evaluate(timer);
                RenderSettings.ambientLight = ambientGradient.Evaluate(timer);
                RenderSettings.fogColor = sunColorGradient.Evaluate(timer);
                //RenderSettings.fogColor = sunColorGradient.Evaluate(timer);
                skyMaterial.SetColor("_Tint", skyColorGradient.Evaluate(timer));
            }).OnComplete(Manager.NewTurn);
        }
    }
}
