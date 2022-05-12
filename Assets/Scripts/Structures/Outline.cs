using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections.Generic;
using System;

namespace Structures
{
    public class Outline : MonoBehaviour
    {
        private RenderTexture outlineRT;
        private Camera cam;

        private void OnEnable()
        {
            Managers.Settings.NewResolution += OnNewResolution;
            cam = GetComponent<Camera>();
            outlineRT = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.RFloat);
            outlineRT.useMipMap = false;
        }

        private void OnNewResolution(int width, int height)
        {
            outlineRT.Release();
            outlineRT = new RenderTexture(width, height, 0, RenderTextureFormat.RFloat);
        }

        private void Update()
        {
            cam.targetTexture = outlineRT;
            Shader.SetGlobalTexture("_OutlineRT", outlineRT);
        }

        private void OnDisable()
        {
            Managers.Settings.NewResolution -= OnNewResolution;
            outlineRT.Release();
            cam.targetTexture = null;
        }
    }
}
