Shader "Grid/GridHighlight"
{
    Properties
    {
        _Mask ("Mask Texture", 2D) = "white" {}
		_Inactive ("Inactive Color", Color) = (1, 1, 1, 1)
		_Active ("Active Color", Color) = (1, 1, 1, 1)
		_Invalid ("Invalid Color", Color) = (1, 1, 1, 1)
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }

		Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _Mask;
			float4 _Inactive;
			float4 _Active;
			float4 _Invalid;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 mask = tex2D(_Mask, i.uv);
				
				float4 col;

				if (mask.r > 0.5)
					col = _Inactive;
				else if (mask.g > 0.5)
					col = _Active;
				else
					col = _Invalid;

                return col;
            }
            ENDCG
        }
    }
}
