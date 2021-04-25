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
        _EmissionIntensity("Intensity (R)", Float) = 0.0
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
        sampler2D _SmoothnessTex;
        sampler2D _EmissionTex;
        fixed3 _RoofColor;
        int _Selected;

        //UNITY_INSTANCING_BUFFER_START(Props)
        //UNITY_DEFINE_INSTANCED_PROP(fixed3, _RoofColor)
        //UNITY_INSTANCING_BUFFER_END(Props)


        struct Input
        {
            float2 uv_MainTex;
            float4 color : COLOR;
        };

        half _EmissionIntensity;
        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
            o.Albedo = lerp(c.rgb, _RoofColor, step(float3(1, 1, 1), c.rgb)).rgb;//c.rgb;
            o.Metallic = _Metallic;
            o.Smoothness = tex2D(_SmoothnessTex, IN.uv_MainTex);

            fixed4 selection = fixed4(0, 0, 0, 0);

            if (_Selected){
                selection += lerp(fixed4(0.5, 0, 0, 0), fixed4(0.1,0,0,0), abs(sin(_Time.z)));
            }

            o.Emission = (tex2D(_EmissionTex, IN.uv_MainTex) * _EmissionIntensity) + selection;
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

        UNITY_INSTANCING_BUFFER_START(Props)
        UNITY_DEFINE_INSTANCED_PROP(int, _HasGrass)
        UNITY_INSTANCING_BUFFER_END(Props)

		sampler2D _BlendTex;
		float4 _Ground;
		float _Height;
		float _Exponent;
		float _Scale;
		float _Stretch;

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
			half blendStrength = lerp(0, pow(saturate(_Height - IN.worldPosition.y), _Exponent) * saturate(blendSamp + .5) * (1 - saturate(IN.blendDot)), _HasGrass);

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
