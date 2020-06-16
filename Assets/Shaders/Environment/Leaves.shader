Shader "Custom/Leaves"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0

		_Detail ("Detail Wind Texture", 2D) = "grey" {}
		_Strength ("Wind Strength", Float) = 1
		_DetailScale ("Detail Wind Scale", Float) = 1
		_DetailSpeed ("Detail Wind Speed", Float) = 1

		_Global ("Global Wind Texture", 2D) = "black" {}
		_Offset ("Global Wind Offset", Float) = 0
		_GlobalScale ("Global Wind Scale", Float) = 1
		_GlobalSpeed ("Global Wind Speed", Float) = 1

		[Toggle] _Debug ("Wind Debug", Float) = 1
		_Direction ("Wind Direction", Vector) = (1, 1, 1, 1)
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows vertex:vert addshadow

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
			float2 detailUV;
			float2 globalUV;
        };

		#define PI 3.141592653589793238462

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

		sampler2D _Detail;
		half _DetailScale;
		half _DetailSpeed;
		half _Strength;

		sampler2D _Global;
		half _Offset;
		half _GlobalScale;
		half _GlobalSpeed;

		float4 _Direction;

		float _Debug;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)
		
		float4 sampleWind(float2 detailUV, float2 globalUV)
		{
			float3 detailSamp = tex2Dlod(_Detail, float4(detailUV, 0, 0)).rgb;
			float globalSamp = saturate(tex2Dlod(_Global, float4(globalUV, 0, 0)).r + _Offset);
			
			return float4(detailSamp, globalSamp);
		}

		float3x3 AngleAxis3x3(float angle, float3 axis)
		{
			float c, s;
			sincos(angle, s, c);

			float t = 1 - c;
			float x = axis.x;
			float y = axis.y;
			float z = axis.z;

			return float3x3(
				t * x * x + c, t * x * y - s * z, t * x * z + s * y,
				t * x * y + s * z, t * y * y + c, t * y * z - s * x,
				t * x * z - s * y, t * y * z + s * x, t * z * z + c
				);
		}

		float rand(float3 co)
		{
			return frac(sin(dot(co.xyz, float3(12.9898, 78.233, 53.539))) * 43758.5453);
		}

		void vert (inout appdata_full v, out Input o)
		{
			float2 direction = normalize(float2(_Direction.x, _Direction.z));
			float4 worldPos = mul(unity_ObjectToWorld, v.vertex);

			float2 detailUV = (float2(worldPos.x, worldPos.z) + direction * _Time[1] * _DetailSpeed) / _DetailScale;
			float2 globalUV = (float2(worldPos.x, worldPos.z) + direction * _Time[1] * _GlobalSpeed) / _GlobalScale;

			float4 windSamp = sampleWind(detailUV, globalUV);
			worldPos.xyz += windSamp.rgb * windSamp.a * _Strength;

			v.vertex = mul(unity_WorldToObject, worldPos);
			/*
			float random = rand(mul(unity_ObjectToWorld, float4(0, 0, 0, 1)));

			float3 bendAxis = normalize(cross(normalize(mul(unity_WorldToObject, float4(0, 1, 0, 0))), _Direction));

			float3x3 pivot = AngleAxis3x3(lerp(0, 2 * PI, random), float4(0, 1, 0, 0));
			bendAxis = mul(pivot, bendAxis);

			float frequency = 20;
			float angle = (.1 * PI / 180) * length(v.vertex) * sin((_Time[1] + random) * frequency) * windSamp.a;

			float3x3 rotation = AngleAxis3x3(angle, bendAxis);

			float3 worldObjectOrigin = mul(unity_ObjectToWorld, float4(0, 0, 0, 0)).xyz;
			worldPos.xyz = mul(rotation, worldPos.xyz - worldObjectOrigin) + worldObjectOrigin;
			*/
			v.vertex = mul(unity_WorldToObject, worldPos);
			
			UNITY_INITIALIZE_OUTPUT(Input, o);
			o.detailUV = detailUV;
			o.globalUV = globalUV;
		}

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			
			fixed4 windSample = sampleWind(IN.detailUV, IN.globalUV);
			fixed3 debug = windSample.rgb * windSample.a;
            o.Albedo = lerp(c.rgb, debug, _Debug);

            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
