using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class BlitFeature : ScriptableRendererFeature
{
    public enum BufferType
    {
        CameraColor,
        Custom
    }

    class BlitPass : ScriptableRenderPass
    {
        public Settings settings;
        private ProfilingSampler m_ProfilingSampler = new ProfilingSampler("ColorBlit");
        private RenderTargetIdentifier m_RTSource;
        private RenderTargetIdentifier m_RTDestination;
        private string m_profilerTag;
        private int temporaryID = Shader.PropertyToID("_TempRT");

        bool isSourceAndDestinationSameTarget;
        int sourceID;
        int destinationID;

        public BlitPass(string tag)
        {
            m_profilerTag = tag;
        }

        // This method is called before executing the render pass.
        // It can be used to configure render targets and their clear state. Also to create temporary render target textures.
        // When empty this render pass will render to the active camera render target.
        // You should never call CommandBuffer.SetRenderTarget. Instead call <c>ConfigureTarget</c> and <c>ConfigureClear</c>.
        // The render pipeline will ensure target setup and clearing happens in a performant manner.
        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            RenderTextureDescriptor descriptor = renderingData.cameraData.cameraTargetDescriptor;
            descriptor.depthBufferBits = 0;

            isSourceAndDestinationSameTarget = settings.sourceType == settings.destinationType && 
                (settings.sourceType == BufferType.CameraColor || settings.sourceTextureID == settings.destinationTextureID);

            var renderer = renderingData.cameraData.renderer;

            if(settings.sourceType == BufferType.CameraColor)
            {
                sourceID = -1;
                m_RTSource = renderer.cameraColorTarget;
            }
            else
            {
                sourceID = Shader.PropertyToID(settings.sourceTextureID);
                cmd.GetTemporaryRT(sourceID, descriptor);
                m_RTSource = new RenderTargetIdentifier(sourceID);
            }

            if (isSourceAndDestinationSameTarget)
            {
                destinationID = temporaryID;
                cmd.GetTemporaryRT(destinationID, descriptor);
                m_RTDestination = new RenderTargetIdentifier(destinationID);
            }
            else if (settings.destinationType == BufferType.CameraColor)
            {
                destinationID = -1;
                m_RTDestination = renderer.cameraColorTarget;
            }
            else
            {
                destinationID = Shader.PropertyToID(settings.destinationTextureID);
                cmd.GetTemporaryRT(destinationID, descriptor);
                m_RTDestination = new RenderTargetIdentifier(destinationID);
            }
        }

        // Here you can implement the rendering logic.
        // Use <c>ScriptableRenderContext</c> to issue drawing commands or execute command buffers
        // https://docs.unity3d.com/ScriptReference/Rendering.ScriptableRenderContext.html
        // You don't have to call ScriptableRenderContext.submit, the render pipeline will call it at specific points in the pipeline.
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get(m_profilerTag);

            if (isSourceAndDestinationSameTarget)
            {
                Blit(cmd, m_RTSource, m_RTDestination, settings.blitMaterial, settings.blitPassIndex);
                Blit(cmd, m_RTDestination, m_RTSource);
            }
            else
            {
                Blit(cmd, m_RTSource, m_RTDestination, settings.blitMaterial, settings.blitPassIndex);
            }

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        // Cleanup any allocated resources that were created during the execution of this render pass.
        public override void OnCameraCleanup(CommandBuffer cmd)
        {
        }

        public override void FrameCleanup(CommandBuffer cmd)
        {
            if(destinationID != -1)
                cmd.ReleaseTemporaryRT(destinationID);

            if(m_RTSource == m_RTDestination && sourceID != -1)
                cmd.ReleaseTemporaryRT(sourceID);
        }
    }

    [System.Serializable]
    public class Settings
    {
        public RenderPassEvent passEvent = RenderPassEvent.AfterRenderingOpaques;
        public int blitPassIndex = -1;
        public Material blitMaterial = null;

        public BufferType sourceType = BufferType.CameraColor;
        public BufferType destinationType = BufferType.CameraColor;
        public string sourceTextureID = "_SourceTexture";
        public string destinationTextureID = "_DestinationTexture";
    }

    [SerializeField] private Settings m_settings;

    private Material m_Material;
    private BlitPass m_ScriptablePass;

    /// <inheritdoc/>
    public override void Create()
    {
        m_ScriptablePass = new BlitPass(name);
    }

    // Here you can inject one or multiple render passes in the renderer.
    // This method is called when setting up the renderer once per-camera.
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if(m_settings.blitMaterial == null)
        {
            Debug.LogWarning("Material is empty.");
            return;
        }

        m_ScriptablePass.renderPassEvent = m_settings.passEvent;
        m_ScriptablePass.settings = m_settings;
        renderer.EnqueuePass(m_ScriptablePass);
    }
}


