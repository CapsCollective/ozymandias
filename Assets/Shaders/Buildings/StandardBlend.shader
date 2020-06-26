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
			float3 blendDot;
			float3 worldPosition;
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
			o.worldPosition = mul(unity_ObjectToWorld, v.vertex).xyz;
			o.blendDot = dot(UnityObjectToWorldNormal(v.normal), float3(0, 1, 0));
			
			v.normal = normalize(mul(unity_WorldToObject, float3(0, 1, 0)));
		}

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
			float blendSamp = tex2D(_BlendTex, float2(IN.worldPosition.x, IN.worldPosition.z) / _Scale).r;

			half blendStrength = pow(saturate(_Height - IN.worldPosition.y), _Exponent) * saturate(blendSamp + .5) * (1 - saturate(IN.blendDot));
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
