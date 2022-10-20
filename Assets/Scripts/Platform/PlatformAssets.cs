using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;


namespace Platform
{
    [CreateAssetMenu(fileName = "Platform Assets", menuName = "Platform Assets")]
    public class PlatformAssets : ScriptableObject
    {
        public UniversalRendererData RendererData;

        public bool GenerateColliders;
    }
}
