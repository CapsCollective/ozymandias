using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace Structures
{
    [Serializable]
    [PostProcess(typeof(OutlineRenderer), PostProcessEvent.BeforeStack, "Custom/Post Process Outline")]
    public sealed class OutlinePostProcess : PostProcessEffectSettings
    {
        [ColorUsage(false, true)]public ColorParameter color = new ColorParameter() { value = Color.red };
        public FloatParameter threshold = new FloatParameter() { value = 0.5f };
        public IntParameter scale = new IntParameter() { value = 1 };
        public FloatParameter opacity = new FloatParameter() { value = 1 };
        public BoolParameter debug = new BoolParameter() { value = false };
    }

    public sealed class OutlineRenderer : PostProcessEffectRenderer<OutlinePostProcess>
    {
        public override void Render(PostProcessRenderContext context)
        {
            var sheet = context.propertySheets.Get(Shader.Find("Hidden/Custom/Post Process Outline"));
            sheet.properties.SetColor("_Color", settings.color);
            sheet.properties.SetFloat("_Threshold", settings.threshold);
            sheet.properties.SetFloat("_Scale", settings.scale);
            sheet.properties.SetFloat("_Opacity", settings.opacity);
            sheet.properties.SetInt("_Debug", settings.debug ? 1 : 0);
            context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
        }
    }
}
