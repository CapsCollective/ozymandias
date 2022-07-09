using System.Linq;
using UnityEngine;
using DG.Tweening;

namespace Characters
{
    public class Adventurer : MonoBehaviour
    {
        [SerializeField] private Renderer[] renderers;
        private static readonly int Dither = Shader.PropertyToID("_CharacterDither");
        private float alpha;


        public void FadeTo(float time, float from, float to, bool destroy)
        {
            alpha = from;
            DOTween.To(() => alpha, x => alpha = x, to, time).OnUpdate(() =>
            {
                for (var i = 0; i < renderers.Length; i++)
                {
                    renderers[i].material.SetFloat(Dither, alpha);
                }
            }).OnComplete(() =>
            {
                if(destroy) Destroy(gameObject);
            });
        }
        
        public void SetAlphaTo(float alpha)
        {
            if (gameObject == null) return;

            for (var i = 0; i < renderers.Length; i++)
            {
                var currentDither = renderers[i].material.GetFloat(Dither);
                var newDither = currentDither;
                newDither = alpha;
                renderers[i].material.SetFloat(Dither, newDither);
            }
        }
    }
}
