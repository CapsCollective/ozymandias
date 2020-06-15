Shader "Particles/TwoFaceDissolve"
{
    Properties
    {
		_DissolveGuide("Dissolve Guide", 2D) = "white" {}
		[HDR] _FrontColour("Front Colour", Color) = (1, 1, 1, 1)
		[HDR] _BackColour("Back Colour", Color) = (1, 1, 1, 1)

		_DisplacementGuide("Displacement Guide", 2D) = "white" {}
		_PanSpeed ("Pan Speed", Float) = 1
		_Displacement ("Displacement Amount (World)", Float) = 1
		_DisplacementScale ("Displacement Scale", Float) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
				float4 colour : COLOR;
                float2 dissolveUV : TEXCOORD0;
				float3 normal : NORMAL;
            };

            struct v2f
            {
				float4 vertex : SV_POSITION;
				float4 colour : COLOR;
                float2 dissolveUV : TEXCOORD0;
            };

			sampler2D _DissolveGuide;
			float4 _DissolveGuide_ST;
			float4 _FrontColour;

			sampler2D _DisplacementGuide;
			float _DisplacementScale;
			float _PanSpeed;
			float _Displacement;

            v2f vert (appdata v)
            {
                v2f o;

				float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
				
				float4 displacementUV = float4(float2(worldPos.x + _PanSpeed * _Time[1], worldPos.z) * _DisplacementScale, 0, 0);
				float4 displacementSamp = tex2Dlod(_DisplacementGuide, displacementUV);

				worldPos.xyz += UnityObjectToWorldNormal(v.normal) * displacementSamp.r * _Displacement;
				
				o.vertex = UnityWorldToClipPos(worldPos);
				o.colour = v.colour;
				o.dissolveUV = TRANSFORM_TEX(v.dissolveUV, _DissolveGuide);

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
				clip(tex2D(_DissolveGuide, i.dissolveUV).r - i.colour.a);
				float4 col = _FrontColour;
				return col;
            }
            ENDCG
        }

		Pass
		{
			Cull front
			CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
				float4 colour : COLOR;
                float2 dissolveUV : TEXCOORD0;
				float3 normal : NORMAL;
            };

            struct v2f
            {
				float4 vertex : SV_POSITION;
				float4 colour : COLOR;
                float2 dissolveUV : TEXCOORD0;
            };

			sampler2D _DissolveGuide;
			float4 _DissolveGuide_ST;
			float4 _BackColour;

			sampler2D _DisplacementGuide;
			float _DisplacementScale;
			float _PanSpeed;
			float _Displacement;

            v2f vert (appdata v)
            {
                v2f o;

				float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
				
				float4 displacementUV = float4(float2(worldPos.x + _PanSpeed * _Time[1], worldPos.z) * _DisplacementScale, 0, 0);
				float4 displacementSamp = tex2Dlod(_DisplacementGuide, displacementUV);

				worldPos.xyz += UnityObjectToWorldNormal(v.normal) * displacementSamp.r * _Displacement;
				
				o.vertex = UnityWorldToClipPos(worldPos);
				o.colour = v.colour;
				o.dissolveUV = TRANSFORM_TEX(v.dissolveUV, _DissolveGuide);

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
				clip(tex2D(_DissolveGuide, i.dissolveUV).r - i.colour.a);
				float4 col = _BackColour;
				return col;
            }
            ENDCG
		}
    }
}
