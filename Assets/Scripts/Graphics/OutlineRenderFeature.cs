using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RendererUtils;
using UnityEngine.Rendering.Universal;

public class OutlineRenderFeature : ScriptableRendererFeature
{
    [System.Serializable]
    public class Settings
    {
        public Material material;
        public Material overrideMaterial;
        public RenderTexture outlineRenderTexture;
        public RenderPassEvent passEvent = RenderPassEvent.BeforeRenderingPostProcessing;
        public Color color = Color.white;
        public float scale = 2.0f;
        public float opacity = 0.7f;
        public float threshold = 0.5f;
    }

    public Settings settings = new Settings();

    public class OutlineRenderPass : ScriptableRenderPass
    {
        public static List<MeshFilter> renderers = new List<MeshFilter>();
        public Settings settings;
        public RenderTargetIdentifier source;
        RenderTargetHandle tempTex;
        public static CommandBuffer OutlineBuffer;
        private string profilerTag = "Ouline";

        public OutlineRenderPass(string profilerTag)
        {
            this.profilerTag = profilerTag;
            OutlineBuffer = new CommandBuffer { name = profilerTag };
        }

        // This method is called before executing the render pass.
        // It can be used to configure render targets and their clear state. Also to create temporary render target textures.
        // When empty this render pass will render to the active camera render target.
        // You should never call CommandBuffer.SetRenderTarget. Instead call <c>ConfigureTarget</c> and <c>ConfigureClear</c>.
        // The render pipeline will ensure target setup and clearing happens in a performant manner.
        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            this.source = renderingData.cameraData.renderer.cameraColorTarget;
            var descriptor = renderingData.cameraData.cameraTargetDescriptor;
            descriptor.depthBufferBits = 0;
            descriptor.msaaSamples = 1;
            descriptor.colorFormat = RenderTextureFormat.ARGBFloat;
            cmd.GetTemporaryRT(tempTex.id, descriptor);
            ConfigureTarget(tempTex.Identifier());
            ConfigureClear(ClearFlag.All, Color.black);
        }

        // Here you can implement the rendering logic.
        // Use <c>ScriptableRenderContext</c> to issue drawing commands or execute command buffers
        // https://docs.unity3d.com/ScriptReference/Rendering.ScriptableRenderContext.html
        // You don't have to call ScriptableRenderContext.submit, the render pipeline will call it at specific points in the pipeline.
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            //if (!Application.isPlaying) return;
            //if (renderingData.cameraData.isSceneViewCamera) return;
            if (renderingData.cameraData.targetTexture != null) return;
            CommandBuffer cmd = CommandBufferPool.Get(profilerTag);
            //cmd.DrawRendererList(context.CreateRendererList(desc));
            settings.material.SetColor("_Color", settings.color);
            settings.material.SetFloat("_Scale", settings.scale);
            settings.material.SetFloat("_Threshold", settings.threshold);
            settings.material.SetFloat("_Opacity", settings.opacity);
            // _MainTex must be a PROPERTY not just a variable!!!
            cmd.Blit(source, tempTex.Identifier(), settings.material);
            cmd.Blit(tempTex.Identifier(), source);
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
            CommandBufferPool.Release(cmd);
        }

        // Cleanup any allocated resources that were created during the execution of this render pass.
        public override void OnCameraCleanup(CommandBuffer cmd)
        {
        }
    }

    OutlineRenderPass m_ScriptablePass;

    /// <inheritdoc/>
    public override void Create()
    {
        m_ScriptablePass = new OutlineRenderPass("Outline");
        m_ScriptablePass.settings = settings;
        m_ScriptablePass.renderPassEvent = settings.passEvent;
    }

    // Here you can inject one or multiple render passes in the renderer.
    // This method is called when setting up the renderer once per-camera.
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(m_ScriptablePass);
    }
}


