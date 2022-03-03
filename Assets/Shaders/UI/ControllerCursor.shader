Shader "Unlit/ControllerCursor"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _Clip ("Float", Float) = 0.0
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
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Clip;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = float4(1,1,1,1);
                i.uv -= float2(0.5, 0.5);
                float x = abs(sin(distance(i.uv, 0) - 0.49));
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                clip(_Clip - x);
                return col;
            }
            ENDCG
        }
    }
}
