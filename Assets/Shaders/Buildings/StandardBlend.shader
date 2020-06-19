Shader "Custom/StandardBlend"
{
    Properties
    {
		_Ground ("Ground Color", Color) = (1, 1, 1, 1)
		_Height ("Blend Height", Float) = 1
		_Exponent ("Blend Exponent", Float) = 1

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

		Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 200
		Cull Off
		//Blend SrcAlpha OneMinusSrcAlpha
        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows vertex:vert //alpha:fade
        #pragma target 3.0
        struct Input
        {
            float2 uv_MainTex;
			float4 objectPos;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

		float4 _Ground;
		float _Height;
		float _Exponent;

        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

		void vert(inout appdata_full v, out Input o)
		{
			v.normal = mul(unity_WorldToObject, float3(0, 1, 0));
			UNITY_INITIALIZE_OUTPUT(Input, o);
			o.objectPos = v.vertex;
		}

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
			half blendStrength = pow(saturate(_Height - IN.objectPos.y), _Exponent);
			fixed4 c = _Ground;
			if (blendStrength < 0.5)
				discard;
            o.Albedo = c.rgb;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = blendStrength;
        }
        ENDCG

		// Blending pass
		// 1. Align all vertex normals with the terrain (world-space positive Y)
		// 2. Sample a noise texture with triplanar mapping
		// 3. Combine the noise and the local Y position to create a blending strength
		// 4. Alpha-based blending using the blending strength, return the colour of the terrain

		/*
		Pass
        {
			Tags { "RenderType"="Opaque" "Queue"="Transparent" }
			Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
				float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
				float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
				float4 objectPos : TEXCOORD1;
                UNITY_FOG_COORDS(2)
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

			float4 _Ground;
			float _Height;
			float _Exponent;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.normal = float3(0, 1, 0);
				o.objectPos = v.vertex;
                UNITY_TRANSFER_FOG(o, o.vertex);
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                float4 col = _Ground;
				float blendStrength = pow(saturate(_Height - i.objectPos.y), _Exponent);
				
				col.a = blendStrength;
				UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
            }
            ENDCG
        }
		*/
    }
    FallBack "Diffuse"
}
