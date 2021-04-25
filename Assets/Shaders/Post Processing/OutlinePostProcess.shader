Shader "Hidden/Custom/Post Process Outline"
{
    HLSLINCLUDE

#include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"

    TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
    TEXTURE2D_SAMPLER2D(_OutlineRT, sampler_OutlineRT);
    TEXTURE2D_SAMPLER2D(_CameraDepthTexture, sampler_CameraDepthTexture);
    float2 _MainTex_TexelSize;
    float2 _OutlineBuffer_TexelSize;
    float4 _Color;
    float _Threshold;
    float _Scale;

    float4 Frag(VaryingsDefault i) : SV_Target
    {
        float4 main = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord);
        float4 color = SAMPLE_TEXTURE2D(_OutlineRT, sampler_OutlineRT, i.texcoord);

        float halfScaleFloor = floor(_Scale * 0.5);
        float halfScaleCeil = ceil(_Scale * 0.5);

        float2 uv0 = i.texcoord - float2(_MainTex_TexelSize.x, _MainTex_TexelSize.y) * halfScaleFloor;
        float2 uv1 = i.texcoord + float2(_MainTex_TexelSize.x, _MainTex_TexelSize.y) * halfScaleCeil;
        float2 uv2 = i.texcoord + float2(_MainTex_TexelSize.x * halfScaleCeil, -_MainTex_TexelSize.y * halfScaleFloor);
        float2 uv3 = i.texcoord + float2(-_MainTex_TexelSize.x * halfScaleFloor, _MainTex_TexelSize.y * halfScaleCeil);

        half edge = 0;

        float3 c0 = SAMPLE_TEXTURE2D(_OutlineRT, sampler_OutlineRT, uv0).rgb;
        float3 c1 = SAMPLE_TEXTURE2D(_OutlineRT, sampler_OutlineRT, uv1).rgb;
        float3 c2 = SAMPLE_TEXTURE2D(_OutlineRT, sampler_OutlineRT, uv2).rgb;
        float3 c3 = SAMPLE_TEXTURE2D(_OutlineRT, sampler_OutlineRT, uv3).rgb;

        //float d0 = SAMPLE_TEXTURE2D(_CameraDepthTexture, sampler_CameraDepthTexture, uv0).r;
        //float d1 = SAMPLE_TEXTURE2D(_CameraDepthTexture, sampler_CameraDepthTexture, uv1).r;
        //float d2 = SAMPLE_TEXTURE2D(_CameraDepthTexture, sampler_CameraDepthTexture, uv2).r;
        //float d3 = SAMPLE_TEXTURE2D(_CameraDepthTexture, sampler_CameraDepthTexture, uv3).r;

        //c0 = distance(c0.r, d0) > 0.0001 ? 0 : 1;
        //c1 = distance(c1.r, d1) > 0.0001 ? 0 : 1;
        //c2 = distance(c2.r, d2) > 0.0001 ? 0 : 1;
        //c3 = distance(c3.r, d3) > 0.0001 ? 0 : 1;

        float3 cg1 = c1 - c0;
        float3 cg2 = c3 - c2;
        float cg = sqrt(dot(cg1, cg1) + dot(cg2, cg2));

        edge = cg * 1;

        edge = saturate((edge - _Threshold) * 0.5);
        float3 c = lerp(main, _Color, step(0.01, edge));
        float3 cAll = float3(c0.r * 0.1,0,0) * _Color;

        return float4(c, 1);
    }

        ENDHLSL

        SubShader
    {
        Cull Off ZWrite Off ZTest Always
            Stencil{
                Ref 2
                Comp equal
                Pass replace
        }

            Pass
        {
            HLSLPROGRAM

            #pragma vertex VertDefault
            #pragma fragment Frag

            ENDHLSL
        }
    }
}
