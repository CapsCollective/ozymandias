using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(Camera))]
[ExecuteAlways]
public class CameraOutlineController : MonoBehaviour
{
    public static CommandBuffer OutlineBuffer { get; private set; }

    private Camera cam;
    private int bufferName;

    // Start is called before the first frame update
    void OnEnable()
    {
        cam = GetComponent<Camera>();
        OutlineBuffer = new CommandBuffer();
        OutlineBuffer.name = "Outline Buffer";
        cam.AddCommandBuffer(CameraEvent.BeforeImageEffectsOpaque, OutlineBuffer);

        bufferName = Shader.PropertyToID("_OutlineBuffer");
    }

    // Update is called once per frame
    void Update()
    {
        OutlineBuffer.Clear();
        OutlineBuffer.GetTemporaryRT(bufferName, -1, -1, 32, FilterMode.Point, RenderTextureFormat.RFloat);
        OutlineBuffer.SetGlobalTexture("_OutlineRT", bufferName);
        OutlineBuffer.SetRenderTarget(bufferName);

        OutlineBuffer.ClearRenderTarget(true, true, Color.black);
    }

    private void OnDisable()
    {
        cam.RemoveCommandBuffer(CameraEvent.BeforeImageEffectsOpaque, OutlineBuffer);
        OutlineBuffer.Clear();
        OutlineBuffer.Dispose();

    }
}
