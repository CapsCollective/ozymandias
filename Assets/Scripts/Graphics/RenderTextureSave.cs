using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

[ExecuteAlways]
public class RenderTextureSave : MonoBehaviour
{
    [SerializeField] private CustomRenderTexture renderTexture;

    [Button]
    public void SaveToTexture()
    {
        RenderTexture.active = renderTexture;
        Texture2D tex = new Texture2D(renderTexture.width, renderTexture.height);
        tex.ReadPixels(new Rect(0, 0, tex.width, tex.height), 0, 0);
        byte[] data = tex.EncodeToPNG();
        System.IO.File.WriteAllBytes($"{Application.dataPath}/Textures/GrassRT.png", data);
    }
}
