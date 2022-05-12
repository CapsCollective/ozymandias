Shader "Custom/Post Processing/Post Process Outline"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
    }

    SubShader
    {
        Cull Off ZWrite Off ZTest Always
        Stencil
        {
            Ref 2
            Comp equal
            Pass replace
        }

        Pass
        {
            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment Frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                // The positionOS variable contains the vertex positions in object
                // space.
                float4 positionOS   : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                // The positions in this struct must have the SV_POSITION semantic.
                float4 positionHCS  : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            sampler2D _OutlineRT;
            //TEXTURE2D_SAMPLER2D(_CameraDepthTexture, sampler_CameraDepthTexture);
            float4 _MainTex_TexelSize;
            float2 _OutlineRT_TexelSize;
            float4 _Color;
            float _Threshold;
            float _Scale;
            float _Opacity;
            int _Debug;

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                // The TRANSFORM_TEX macro performs the tiling and offset
                // transformation.
                OUT.uv = IN.uv;
                return OUT;
            }


            float4 Frag(Varyings i) : SV_Target
            {
                float4 main = tex2D(_MainTex, i.uv);
                float4 color = tex2Dlod(_OutlineRT, float4(i.uv,0,0));

                float halfScaleFloor = floor(_Scale * 0.5);
                float halfScaleCeil = ceil(_Scale * 0.5);

                float2 uv0 = i.uv - float2(_MainTex_TexelSize.x, _MainTex_TexelSize.y) * halfScaleFloor;
                float2 uv1 = i.uv + float2(_MainTex_TexelSize.x, _MainTex_TexelSize.y) * halfScaleCeil;
                float2 uv2 = i.uv + float2(_MainTex_TexelSize.x * halfScaleCeil, -_MainTex_TexelSize.y * halfScaleFloor);
                float2 uv3 = i.uv + float2(-_MainTex_TexelSize.x * halfScaleFloor, _MainTex_TexelSize.y * halfScaleCeil);

                half edge = 0;

                float3 c0 = tex2D(_OutlineRT, uv0).rgb;
                float3 c1 = tex2D(_OutlineRT, uv1).rgb;
                float3 c2 = tex2D(_OutlineRT, uv2).rgb;
                float3 c3 = tex2D(_OutlineRT, uv3).rgb;

                if (_Debug == 1) return float4(c0.rgb, 1);

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

                c = lerp(c, main, _Opacity);

                return float4(c, edge);
            }

        ENDHLSL
        }
    }
}
