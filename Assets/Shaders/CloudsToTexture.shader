Shader "Unlit/CloudsToTexture"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
        _Tex("InputTex", 2D) = "white" {}
        _Smoothstep("Smoothing", Vector) = (1,1,1,1)
    }

        SubShader
    {
       Lighting Off
       Blend One Zero

       Pass
       {
           HLSLPROGRAM
           #include "Assets/Shaders/VoronoiExtension.hlsl"
           #include "UnityCustomRenderTexture.cginc"
           #pragma vertex CustomRenderTextureVertexShader
           #pragma fragment frag
           #pragma target 3.0

           float4      _Color;
           sampler2D   _Tex;
           float2 _Smoothstep;

           float4 frag(v2f_customrendertexture IN) : COLOR
           {
               float c = VoronoiColor_float(_Color.rgb, IN.localTexcoord.xy * 10 + _Time.x, 0.3, 0.3, 1.0, 0.171);
           c = smoothstep(_Smoothstep.x, _Smoothstep.y, 1 - c);
               return c;//VoronoiColor_float(_Color.rgb, IN.localTexcoord.xy * 10, _Time.x, 0.3, 1.0);
           }
           ENDHLSL
           }
    }
}
