using UnityEngine;
using UnityEngine.Rendering;

namespace Structures
{
    [ExecuteAlways]
    public class Outline : MonoBehaviour
    {
        public static CommandBuffer OutlineBuffer { get; private set; }

        private Camera _cam;
        private int _bufferName;

        private void OnEnable()
        {
            _cam = Camera.main;
            OutlineBuffer = new CommandBuffer {name = "Outline Buffer"};
            if (_cam) _cam.AddCommandBuffer(CameraEvent.BeforeImageEffectsOpaque, OutlineBuffer);

            _bufferName = Shader.PropertyToID("_OutlineBuffer");
        }

        private void Update()
        {
            OutlineBuffer.Clear();
            OutlineBuffer.GetTemporaryRT(_bufferName, -1, -1, 32, FilterMode.Point, RenderTextureFormat.RFloat);
            OutlineBuffer.SetGlobalTexture("_OutlineRT", _bufferName);
            OutlineBuffer.SetRenderTarget(_bufferName);

            OutlineBuffer.ClearRenderTarget(true, true, Color.black);
        }

        private void OnDisable()
        {
            if (_cam) _cam.RemoveCommandBuffer(CameraEvent.BeforeImageEffectsOpaque, OutlineBuffer);
            OutlineBuffer.Clear();
            OutlineBuffer.Dispose();
        }
    }
}
