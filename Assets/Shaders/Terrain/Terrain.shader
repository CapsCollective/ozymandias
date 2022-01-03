Shader "Custom/Terrain Custom"
{
    Properties{
        // used in fallback on old cards & base map
        [HideInInspector] _MainTex("BaseMap (RGB)", 2D) = "white" {}
        [HideInInspector] _Color("Main Color", Color) = (1,1,1,1)
        [HideInInspector] _TerrainHolesTexture("Holes Map (RGB)", 2D) = "white" {}

        _NoiseTexture("Noise Texture", 2D) = "white" {}
        _Height("Height", Float) = 0.0
        _Sand("Sand", Color) = (1,1,1,1)
        _Grass("Grass", Color) = (1,1,1,1)
        _SnowColor("Snow", Color) = (1,1,1,1)
        _AutumnColor("Autumn", Color) = (1,1,1,1)
    }

        SubShader{
            Tags {
                "Queue" = "Geometry-100"
                "RenderType" = "Opaque"
            }

            CGPROGRAM
            #pragma surface surf Standard vertex:vert addshadow fullforwardshadows
            #pragma multi_compile_instancing
            #pragma instancing_options assumeuniformscaling nomatrices nolightprobe nolightmap forwardadd
            #pragma target 4.0
            #include "UnityPBSLighting.cginc"

            #pragma multi_compile_local __ _ALPHATEST_ON
            #pragma multi_compile_local __ _NORMALMAP

            #define TERRAIN_STANDARD_SHADER
            #define TERRAIN_INSTANCED_PERPIXEL_NORMAL
            #define TERRAIN_SURFACE_OUTPUT SurfaceOutputStandard
            //#include "TerrainSplatmapCommon.cginc"

            struct Input
            {
                float3 wPos: POSITION;
                float3 oPos : POSITION;
                float3 wNormal : NORMAL;
            };

            sampler2D _NoiseTexture;
            half _Height;
            half3 _Sand;
            half3 _Grass;
            half3 _SnowColor;
            half3 _AutumnColor;
            half _Winter;
            half _Autumn;

            inline float4 TriplanarSampling(sampler2D topTexMap, float3 worldPos, float3 worldNormal, float falloff, float2 tiling, float3 normalScale, float3 index)
            {
                float3 projNormal = (pow(abs(worldNormal), falloff));
                projNormal /= (projNormal.x + projNormal.y + projNormal.z) + 0.00001;
                float3 nsign = sign(worldNormal);
                half4 xNorm; half4 yNorm; half4 zNorm;
                xNorm = tex2D(topTexMap, tiling * worldPos.zy * float2(nsign.x, 1.0));
                yNorm = tex2D(topTexMap, tiling * worldPos.xz * float2(nsign.y, 1.0));
                zNorm = tex2D(topTexMap, tiling * worldPos.xy * float2(-nsign.z, 1.0));
                return xNorm * projNormal.x + yNorm * projNormal.y + zNorm * projNormal.z;
            }

            void vert(inout appdata_full v, out Input o) {
                UNITY_INITIALIZE_OUTPUT(Input, o);
                float3 vertexWorldSpace = mul(unity_ObjectToWorld, v.vertex);
                o.wPos = vertexWorldSpace;
                o.oPos = unity_ObjectToWorld._m03_m13_m23;
                o.wNormal = UnityObjectToWorldNormal(v.normal);
            }

            void surf(Input IN, inout SurfaceOutputStandard o) {
                //half4 splat_control;
                //half weight;
                //fixed4 mixedDiffuse;
                //half4 defaultSmoothness = half4(_Smoothness0, _Smoothness1, _Smoothness2, _Smoothness3);
                //SplatmapMix(IN, defaultSmoothness, splat_control, weight, mixedDiffuse, o.Normal);

                float noise = TriplanarSampling(_NoiseTexture, IN.wPos, IN.wNormal, 1.0, float2(0.03, 0.03), 1.0, 0).r;
                float height = saturate(smoothstep(0.5, 0.5, noise + (IN.wPos.y - _Height)));

                float3 color = lerp(lerp(_Grass, _SnowColor, _Winter), _AutumnColor, _Autumn);
                color = lerp(_Sand, color, height);

                o.Albedo = color;//mixedDiffuse.rgb;
                //o.Alpha = weight;
                //o.Smoothness = mixedDiffuse.a;
                //o.Metallic = dot(splat_control, half4(_Metallic0, _Metallic1, _Metallic2, _Metallic3));
            }
            ENDCG

            UsePass "Hidden/Nature/Terrain/Utilities/PICKING"
            UsePass "Hidden/Nature/Terrain/Utilities/SELECTION"
        }

        Dependency "AddPassShader" = "Hidden/TerrainEngine/Splatmap/Standard-AddPass"
        Dependency "BaseMapShader" = "Hidden/TerrainEngine/Splatmap/Standard-Base"
        Dependency "BaseMapGenShader" = "Hidden/TerrainEngine/Splatmap/Standard-BaseGen"

        Fallback "Nature/Terrain/Diffuse"
}