Shader "Unlit/DebugBounds"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
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
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float3 wPos : POSITIONT;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float3 _Position;
            float _Distance;
            float _DotMultiplier;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.wPos = mul(unity_ObjectToWorld, v.vertex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                // apply fog
                float dist = distance(i.wPos, _Position) / _Distance;
                float dd = dot(float3(0, 0, -1), normalize(i.wPos)) * _DotMultiplier;

                if (dd > 0.0) {
                    dist -= dd;
                }

                float s = step(dist, 1.0);

                UNITY_APPLY_FOG(i.fogCoord, col);
                return float4(s,s,s,1);
            }
            ENDCG
        }
    }
}
