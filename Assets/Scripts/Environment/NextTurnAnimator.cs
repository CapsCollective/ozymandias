using DG.Tweening;
using UnityEngine;
using static GameManager;

namespace Environment
{
    public class NextTurnAnimator : MonoBehaviour
    {
        public Light sun;
        public float sunSetTime = 2f;
        public ParticleSystem glowflyPS;

        #pragma warning disable 0649
        [SerializeField] private Gradient ambientGradient;   
        [SerializeField] private Gradient sunColorGradient;

        void Awake()
        {
            GameManager.OnNextTurn += OnNextTurn;
        }

        public void OnNextTurn()
        {
            glowflyPS.Play();
            float timer = 0;
            Transform t = sun.transform;
            t.DORotate(t.eulerAngles + new Vector3(360,0,0), sunSetTime, RotateMode.FastBeyond360).OnUpdate(() =>
            {
                timer += Time.deltaTime / sunSetTime;
                sun.color = sunColorGradient.Evaluate(timer);
                RenderSettings.ambientLight = ambientGradient.Evaluate(timer);
            }).OnComplete(Manager.NewTurn);
        }
    }
}
