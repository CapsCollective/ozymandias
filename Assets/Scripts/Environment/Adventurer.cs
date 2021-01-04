using UnityEngine;

namespace Environment
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
            for (int i = 0; i < renderers.Length; i++)
            {
                renderers[i].GetPropertyBlock(_propertyBlocks[i]);
                Color currentColour = _propertyBlocks[i].GetColor(Color);
                Color newColour = currentColour;
                newColour.a = alpha;
                _propertyBlocks[i].SetColor(Color, newColour);
                renderers[i].SetPropertyBlock(_propertyBlocks[i]);
            }
        }
    }
}
