using System.Linq;
using UnityEngine;

namespace WalkingAdventurers
{
    public class Adventurer : MonoBehaviour
    {
        [SerializeField] private Renderer[] renderers;
        private MaterialPropertyBlock[] _propertyBlocks;
        private static readonly int Color = Shader.PropertyToID("_Color");

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
                var currentColour = _propertyBlocks[i].GetColor(Color);
                var newColour = currentColour;
                newColour.a = alpha;
                _propertyBlocks[i].SetColor(Color, newColour);
                renderers[i].SetPropertyBlock(_propertyBlocks[i]);
            }
        }
    }
}
