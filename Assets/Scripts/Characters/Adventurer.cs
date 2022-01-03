using System.Linq;
using UnityEngine;

namespace Characters
{
    public class Adventurer : MonoBehaviour
    {
        [SerializeField] private Renderer[] renderers;
        private MaterialPropertyBlock[] _propertyBlocks;
        private static readonly int Dither = Shader.PropertyToID("_CharacterDither");

        private void Awake()
        {
            _propertyBlocks = renderers.Select(_ => new MaterialPropertyBlock()).ToArray();
        }
        
        public void SetAlphaTo(float alpha)
        {
            if (gameObject == null) return;

            for (var i = 0; i < renderers.Length; i++)
            {
                renderers[i].GetPropertyBlock(_propertyBlocks[i]);
                var currentDither = _propertyBlocks[i].GetFloat(Dither);
                var newDither = currentDither;
                newDither = alpha;
                _propertyBlocks[i].SetFloat(Dither, newDither);
                renderers[i].SetPropertyBlock(_propertyBlocks[i]);
            }
        }
    }
}
