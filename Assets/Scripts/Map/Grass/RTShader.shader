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

           float4      _Color;
           sampler2D   _Tex;

           float4 frag(v2f_customrendertexture IN) : COLOR
           {
            float c = tex2D(_Tex, IN.localTexcoord.xy).r;
            float4 k = step(0.1, c);
            return k;
           }
           ENDCG
           }
    }
}
