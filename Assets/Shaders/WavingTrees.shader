Shader "Custom/WavingTrees"
{
    Properties
    {
        _MainTex("Albedo (RGB)", 2D) = "white" {}
        _MaskTex ("Mask (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _WindStrength("Wind Strength", Range(0,1)) = 0.0
        _WindSpeed("Wind Speed", Range(0,1)) = 0.0
        _NoiseTex1("Noise Texture", 2D) = "white" {}
        _NoiseTex2("Noise Texture", 2D) = "white" {}
        _AlphaDistance("Transparency Distance", Float) = 1.0
        _AlphaFalloff("Transparency Falloff", Float) = 1.0
        
        // Seasonal effects
        
    	_TrunkCol ("Trunk Colour", Color) = (1, 1, 1, 1)
    	    	
    	_SpringCol1 ("Spring Colour 1", Color) = (1, 1, 1, 1)
    	_SpringCol2 ("Spring Colour 2", Color) = (1, 1, 1, 1)
        
    	_AutumnCol1 ("Autumn Colour 1", Color) = (1, 1, 1, 1)
    	_AutumnCol2 ("Autumn Colour 2", Color) = (1, 1, 1, 1)
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
        sampler2D _NoiseTex1;
        sampler2D _NoiseTex2;
        sampler2D _MaskTex;

        struct Input
        {
            float2 uv_MainTex;
            float3 wPos;
            float3 oPos;
            float4 screenPos;
        	float4 autumn_uv;
        	float2 lerp_uv;
        };

        half _Glossiness;
        half _Metallic;
        half _AlphaDistance;
        half _AlphaFalloff;

        // Seasonal effects

        float4 _TrunkCol;

        uniform float _Autumn;
        half _Snow;
        
        float4 _AutumnCol1;
        float4 _AutumnCol2;
        
        float4 _SpringCol1;
        float4 _SpringCol2;

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

        float3 WorldScale() {
            return float3(
                length(float3(unity_ObjectToWorld[0].x, unity_ObjectToWorld[1].x, unity_ObjectToWorld[2].x)), // scale x axis
                length(float3(unity_ObjectToWorld[0].y, unity_ObjectToWorld[1].y, unity_ObjectToWorld[2].y)), // scale y axis
                length(float3(unity_ObjectToWorld[0].z, unity_ObjectToWorld[1].z, unity_ObjectToWorld[2].z))  // scale z axis
                );
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
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex);
            half mask = tex2D(_MaskTex, IN.uv_MainTex);
            fixed nO1 = tex2D(_NoiseTex1, IN.oPos.xz).r;
            fixed nO2 = tex2D(_NoiseTex2, IN.oPos.xz).r;
            fixed nW = tex2D(_NoiseTex1, IN.wPos.xz * 0.04).r;

            const half autumn_depth = step(saturate(0.99 * nO2), _Autumn);
            _Autumn = step(saturate(0.99 * nO2), autumn_depth);
            const half snow_depth = step(saturate(0.99 * nO2), _Snow);
            _Snow = lerp(0, 0.15, snow_depth);
            
        	const float4 season_colour1 = lerp(_SpringCol1, _AutumnCol1, _Autumn);
        	const float4 season_colour2 = lerp(_SpringCol2, _AutumnCol2, _Autumn);

            const float4 leaf_col = lerp(lerp(season_colour1, season_colour2, nO1) *
            	saturate(nO1 + 0.7), fixed4(1,1,1,1),
            	step(0.15, 1 - pow(mask, _Snow * saturate(nW + 0.5))));
        	c = lerp(_TrunkCol, leaf_col, c >= 1);

        	o.Albedo = c.rgb;
            
            float alpha = saturate(pow(distance(IN.wPos, _WorldSpaceCameraPos) / _AlphaDistance, _AlphaFalloff));

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
