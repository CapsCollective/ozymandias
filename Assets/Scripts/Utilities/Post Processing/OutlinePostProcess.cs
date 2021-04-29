using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;


[Serializable]
[PostProcess(typeof(OutlineRenderer), PostProcessEvent.BeforeStack, "Custom/Post Process Outline")]
public sealed class OutlinePostProcess : PostProcessEffectSettings
{
    [ColorUsage(false, true)]public ColorParameter color = new ColorParameter() { value = Color.red };
    public FloatParameter threshold = new FloatParameter() { value = 0.5f };
    public IntParameter scale = new IntParameter() { value = 1 };
    public FloatParameter opacity = new FloatParameter() { value = 1 };
}

public sealed class OutlineRenderer : PostProcessEffectRenderer<OutlinePostProcess>
{

    public override void Init()
    {
        base.Init();
    }

    public override void Render(PostProcessRenderContext context)
    {
        var sheet = context.propertySheets.Get(Shader.Find("Hidden/Custom/Post Process Outline"));
        sheet.properties.SetColor("_Color", settings.color);
        sheet.properties.SetFloat("_Threshold", settings.threshold);
        sheet.properties.SetFloat("_Scale", settings.scale);
        sheet.properties.SetFloat("_Opacity", settings.opacity);
        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }
}