using NaughtyAttributes;
using UnityEngine;

[ExecuteInEditMode]
public class RenderTextureSave : MonoBehaviour
{
    [SerializeField] private CustomRenderTexture renderTexture;
    [SerializeField] private string fileName;

    [Button]
    public void SaveToTexture()
    {
        RenderTexture.active = renderTexture;
        Texture2D tex = new Texture2D(renderTexture.width, renderTexture.height);
        tex.ReadPixels(new Rect(0, 0, tex.width, tex.height), 0, 0);
        byte[] data = tex.EncodeToPNG();
        System.IO.File.WriteAllBytes($"{Application.dataPath}/Textures/{fileName}.png", data);
    }
}
