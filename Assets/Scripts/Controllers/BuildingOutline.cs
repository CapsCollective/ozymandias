using UnityEngine;
using UnityEngine.Rendering;

namespace Controllers
{
    [RequireComponent(typeof(Camera))]
    [ExecuteAlways]
    public class BuildingOutline : MonoBehaviour
    {
        public static CommandBuffer OutlineBuffer { get; private set; }

        private Camera _cam;
        private int _bufferName;

        private void OnEnable()
        {
            _cam = GetComponent<Camera>();
            OutlineBuffer = new CommandBuffer {name = "Outline Buffer"};
            _cam.AddCommandBuffer(CameraEvent.BeforeImageEffectsOpaque, OutlineBuffer);

            _bufferName = Shader.PropertyToID("_OutlineBuffer");
        }

        // Update is called once per frame
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
            _cam.RemoveCommandBuffer(CameraEvent.BeforeImageEffectsOpaque, OutlineBuffer);
            OutlineBuffer.Clear();
            OutlineBuffer.Dispose();

        }
    }
}
