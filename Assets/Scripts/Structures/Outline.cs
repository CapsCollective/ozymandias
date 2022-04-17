using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections.Generic;

namespace Structures
{
    [ExecuteAlways]
    public class Outline : MonoBehaviour
    {
        private RenderTexture outlineRT;
        private Camera cam;

        private void OnEnable()
        {
            cam = GetComponent<Camera>();
            outlineRT = RenderTexture.GetTemporary(Screen.width, Screen.height, 0, RenderTextureFormat.RFloat);
        }

        private void Update()
        {
            cam.targetTexture = outlineRT;
            Shader.SetGlobalTexture("_OutlineRT", outlineRT);
        }

        private void OnDisable()
        {
            RenderTexture.ReleaseTemporary(outlineRT);
            cam.targetTexture = null;
        }
    }
}
