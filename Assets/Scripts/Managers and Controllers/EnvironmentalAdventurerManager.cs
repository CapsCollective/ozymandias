using UnityEngine;

namespace Managers_and_Controllers
{
    public class EnvironmentalAdventurerManager : MonoBehaviour
    {
        [SerializeField] private Renderer[] renderers;
        private MaterialPropertyBlock[] propertyBlocks;
        private static readonly int Color = Shader.PropertyToID("_Color");

        private void Awake()
        {
            propertyBlocks = new[] {new MaterialPropertyBlock(), new MaterialPropertyBlock()};
        }
        
        public void SetAlphaTo(float alpha)
        {
            for (var i = 0; i < renderers.Length; i++)
            {
                renderers[i].GetPropertyBlock(propertyBlocks[i]);
                var currentColour = propertyBlocks[i].GetColor(Color);
                var newColour = currentColour;
                newColour.a = alpha;
                propertyBlocks[i].SetColor(Color, newColour);
                renderers[i].SetPropertyBlock(propertyBlocks[i]);
            }
        }
    }
}