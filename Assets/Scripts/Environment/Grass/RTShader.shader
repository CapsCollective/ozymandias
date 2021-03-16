Shader "Unlit/RTShader"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
        _Tex("InputTex", 2D) = "white" {}
    }

        SubShader
    {
       Lighting Off
       Blend One Zero

       Pass
       {
           CGPROGRAM
           #include "UnityCustomRenderTexture.cginc"
           #pragma vertex CustomRenderTextureVertexShader
           #pragma fragment frag
           #pragma target 3.0

           float4 _Color;
           sampler2D _Tex;
           float2 _Tex_TexelSize;

           float4 frag(v2f_customrendertexture IN) : COLOR
           {
                float3 c = tex2D(_Tex, IN.localTexcoord.xy);

                float c1 = tex2D(_Tex, IN.localTexcoord.xy + float2(_Tex_TexelSize.x, 0));
                float c2 = tex2D(_Tex, IN.localTexcoord.xy + float2(0, _Tex_TexelSize.y));
                float c3 = tex2D(_Tex, IN.localTexcoord.xy + float2(-_Tex_TexelSize.x, 0));
                float c4 = tex2D(_Tex, IN.localTexcoord.xy + float2(0, -_Tex_TexelSize.y));

                float3 k = step(0.01, c);

                //if (k < 1) {
                //    k = lerp(k, (c1 + c2 + c3 + c4) * 0.25, 0.75);
                //}
                //float kf = lerp(k, (c1 + c2 + c3 + c4) * 0.25, step(0, k));

                return float4(k, 1);
           }
           ENDCG
           }
    }
}
