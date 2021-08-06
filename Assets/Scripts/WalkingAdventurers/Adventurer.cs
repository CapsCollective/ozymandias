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
            _propertyBlocks = new[] {new MaterialPropertyBlock(), new MaterialPropertyBlock()};
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
