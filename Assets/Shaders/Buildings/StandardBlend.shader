Shader "Custom/StandardBlend"
{
    Properties
    {
		_BlendTex ("Blend Texture", 2D) = "white" {}
		_Ground ("Ground Color", Color) = (1, 1, 1, 1)
		_Height ("Blend Height", Float) = 1
		_Exponent ("Blend Exponent", Float) = 1
		_Scale ("Blend Scale", Float) = 1
		_Stretch ("Blend Stretch", Float) = 1

        _Color("Color", Color) = (1,1,1,1)
        _RoofColor ("Roof Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _SmoothnessTex("Smoothness (R)", 2D) = "black" {}
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _EmissionTex("Emission (RGB)", 2D) = "black" {}
        _WindowEmissionTex("Window (RGB)", 2D) = "black" {}
        _WindowMaxBrightness("Window Max Brightness", Float) = 10.0
        _EmissionIntensity("Intensity (R)", Float) = 0.0
        _NoiseTex("Noise Texture", 2D) = "white" {}
        _NoiseScale("Noise Scale ", Float) = 0.0
    }
    SubShader
    {
		// Surface shader pass
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard vertex:vert fullforwardshadows

        #pragma target 4.0

        struct Input
        {
            float2 uv_MainTex;
            float4 color : COLOR;
            float3 oPos;
            float3 wPos;
            float3 blendDot;
        };

        UNITY_INSTANCING_BUFFER_START(Props)
        UNITY_DEFINE_INSTANCED_PROP(int, _HasGrass)
        UNITY_INSTANCING_BUFFER_END(Props)

        sampler2D _MainTex;
        sampler2D _SmoothnessTex;
        sampler2D _EmissionTex;
        sampler2D _WindowEmissionTex;
        sampler2D _NoiseTex;
        fixed3 _RoofColor;
        int _Selected;

        half _EmissionIntensity;
        half _WindowEmissionIntensity;
        half _WindowMaxBrightness = 10;
        half _Glossiness;
        half _Metallic;
        half _NoiseScale;
        fixed4 _Color;

        sampler2D _BlendTex;
        float4 _Ground;
        float _Height;
        float _Exponent;
        float _Scale;
        float _Stretch;

        void vert(inout appdata_full v, out Input o) 
        {
            UNITY_INITIALIZE_OUTPUT(Input, o);
            o.wPos = mul(unity_ObjectToWorld, v.vertex);
            o.oPos = unity_ObjectToWorld._m03_m13_m23;
			o.blendDot = dot(UnityObjectToWorldNormal(v.normal), float3(0, 1, 0));

        }

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            float blendSamp = tex2D(_BlendTex, float2(IN.wPos.x, IN.wPos.z) / _Scale).r;
            half blendStrength = lerp(0, pow(saturate(_Height - IN.wPos.y), _Exponent) * saturate(blendSamp + .5) * (1 - saturate(IN.blendDot)), _HasGrass);

            fixed4 c = 0;
            if (blendStrength > 0.45)
                c = _Ground;
            else
                c = tex2D(_MainTex, IN.uv_MainTex);

            //fixed4 c = step(tex2D(_MainTex, IN.uv_MainTex), _Ground, blendStrength);
            o.Albedo = lerp(c.rgb, _RoofColor, step(float3(1, 1, 1), c.rgb)).rgb;//c.rgb;
            o.Metallic = _Metallic;
            o.Smoothness = tex2D(_SmoothnessTex, IN.uv_MainTex);
    
            half positionalIntensity = tex2D(_NoiseTex, IN.oPos.xz * _NoiseScale) + 0.25;
            half windowI = lerp(0, _WindowMaxBrightness, step(0.25, _WindowEmissionIntensity * positionalIntensity));
            o.Emission = (tex2D(_EmissionTex, IN.uv_MainTex) * _EmissionIntensity) + (tex2D(_WindowEmissionTex, IN.uv_MainTex) * (windowI + positionalIntensity));
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
