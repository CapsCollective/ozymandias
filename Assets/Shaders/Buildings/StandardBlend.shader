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

        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
		// Surface shader pass
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows

        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG

		Tags { "RenderType"="Opaque" "Queue"="Geometry" }
        LOD 200
		Blend SrcAlpha OneMinusSrcAlpha
        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows vertex:vert
        #pragma target 3.0
        struct Input
        {
            float2 uv_MainTex;
			float4 objectPos;
			float3 worldNormal;
			float3 worldPos;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

		sampler2D _BlendTex;
		float4 _Ground;
		float _Height;
		float _Exponent;
		float _Scale;
		float _Stretch;

        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

		void vert(inout appdata_full v, out Input o)
		{
			UNITY_INITIALIZE_OUTPUT(Input, o);
			o.objectPos = v.vertex;
			o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
			o.worldNormal = UnityObjectToWorldNormal(v.normal);
		}

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
			float x = tex2D(_BlendTex, float2(IN.worldPos.y, IN.worldPos.z) / _Scale).r;
			float y = tex2D(_BlendTex, float2(IN.worldPos.x, IN.worldPos.z) / _Scale).r;
			float z = tex2D(_BlendTex, float2(IN.worldPos.x, IN.worldPos.y) / _Scale).r;

			float blendSamp = saturate(x * IN.worldNormal.x) + saturate(y * IN.worldNormal.y) + saturate(z * IN.worldNormal.z);
			blendSamp = y;
			half blendStrength = pow(saturate(_Height - IN.worldPos.y), _Exponent) * saturate(blendSamp + .5) * (1 - saturate(dot(IN.worldNormal, float3(0, 1, 0))));
			fixed4 c = _Ground;

			if (blendStrength < 0.5)
				discard;

            o.Albedo = c.rgb;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
