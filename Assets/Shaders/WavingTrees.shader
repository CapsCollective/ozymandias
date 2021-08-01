Shader "Custom/WavingTrees"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _WindStrength("Wind Strength", Range(0,1)) = 0.0
        _WindSpeed("Wind Speed", Range(0,1)) = 0.0
        _NoiseTex("Noise Texture", 2D) = "white" {}
        _AlphaDistance("Transparency Distance", Float) = 1.0
        _AlphaFalloff("Transparency Falloff", Float) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows vertex:vert

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 4.0

        sampler2D _MainTex;
        sampler2D _NoiseTex;

        struct Input
        {
            float2 uv_MainTex;
            float3 wPos;
            float3 oPos;
            float4 screenPos;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
        half _AlphaDistance;
        half _AlphaFalloff;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        float3 mod2D289(float3 x) { return x - floor(x * (1.0 / 289.0)) * 289.0; }
        float2 mod2D289(float2 x) { return x - floor(x * (1.0 / 289.0)) * 289.0; }
        float3 permute(float3 x) { return mod2D289(((x * 34.0) + 1.0) * x); }

        float snoise(float2 v)
        {
            const float4 C = float4(0.211324865405187, 0.366025403784439, -0.577350269189626, 0.024390243902439);
            float2 i = floor(v + dot(v, C.yy));
            float2 x0 = v - i + dot(i, C.xx);
            float2 i1;
            i1 = (x0.x > x0.y) ? float2(1.0, 0.0) : float2(0.0, 1.0);
            float4 x12 = x0.xyxy + C.xxzz;
            x12.xy -= i1;
            i = mod2D289(i);
            float3 p = permute(permute(i.y + float3(0.0, i1.y, 1.0)) + i.x + float3(0.0, i1.x, 1.0));
            float3 m = max(0.5 - float3(dot(x0, x0), dot(x12.xy, x12.xy), dot(x12.zw, x12.zw)), 0.0);
            m = m * m;
            m = m * m;
            float3 x = 2.0 * frac(p * C.www) - 1.0;
            float3 h = abs(x) - 0.5;
            float3 ox = floor(x + 0.5);
            float3 a0 = x - ox;
            m *= 1.79284291400159 - 0.85373472095314 * (a0 * a0 + h * h);
            float3 g;
            g.x = a0.x * x0.x + h.x * x0.y;
            g.yz = a0.yz * x12.xz + h.yz * x12.yw;
            return 130.0 * dot(m, g);
        }

        inline float Dither4x4Bayer(int x, int y)
        {
            const float dither[16] = {
                 1,  9,  3, 11,
                13,  5, 15,  7,
                 4, 12,  2, 10,
                16,  8, 14,  6 };
            int r = y * 4 + x;
            return dither[r] / 16; // same # of instructions as pre-dividing due to compiler magic
        }

        half _WindStrength;
        half _WindSpeed;
        half _MaskSub;
        half _MaskPower;

        float GetMask(float2 uv) {
            return saturate(pow(uv.y - -_MaskSub, _MaskPower));
        }

        void vert(inout appdata_full v, out Input o) {
            UNITY_INITIALIZE_OUTPUT(Input, o);
            float3 vertexWorldSpace = mul(unity_ObjectToWorld, v.vertex);
            o.wPos = vertexWorldSpace;
            o.oPos = unity_ObjectToWorld._m03_m13_m23;
            v.vertex.xz += GetMask(vertexWorldSpace.xy) * (snoise(vertexWorldSpace.xz * (_Time.x * _WindSpeed)) * _WindStrength);
            o.screenPos = ComputeScreenPos(UnityObjectToClipPos(v.vertex));
        }

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * ((_Color + snoise(IN.oPos.xy) * 0.25));
            fixed n = tex2D(_NoiseTex, IN.uv_MainTex).r;
            float alpha = saturate(pow(distance(IN.wPos, _WorldSpaceCameraPos) / _AlphaDistance, _AlphaFalloff));

            o.Albedo = c.rgb;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;

            float4 normScreenPos = IN.screenPos / IN.screenPos.w;
            float2 clipScreen = normScreenPos.xy * _ScreenParams.xy;
            float dither = Dither4x4Bayer(fmod(clipScreen.x, 4), fmod(clipScreen.y, 4));
            dither = step(dither, alpha);

            clip(dither - 0.5);
        }
        ENDCG
    }
    FallBack "Diffuse"
}
