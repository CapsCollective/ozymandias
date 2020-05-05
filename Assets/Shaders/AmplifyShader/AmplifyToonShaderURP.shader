// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "ToonShader"
{
	Properties
	{
		_Tint("Tint", Color) = (0.6226415,0.6226415,0.6226415,0)
		_Albedo("Albedo", 2D) = "white" {}
		_ToonRamp("ToonRamp", 2D) = "white" {}
		_ShadowOffsetFloat("ShadowOffsetFloat", Range( 0 , 0.5)) = 0.5
		_Normal_Map("Normal_Map", 2D) = "bump" {}
		_RimPower("Rim Power", Range( 0 , 1)) = 0
		_RimOffset("Rim Offset", Range( 0 , 3)) = 0.6
		_RimTint("Rim Tint", Color) = (0.003604494,0.5902472,0.764151,0)
		_SpecularGloss("Specular Gloss", Range( 0 , 1)) = 0.2
		_min("min", Float) = 1.1
		_max("max", Float) = 1.12
		_SpecularIntensity("Specular Intensity", Range( 0 , 1)) = 0.2
		_SpecularMap("Specular Map", 2D) = "white" {}
		_Spec_Color("Spec_Color", Color) = (1,1,1,0)
		_SpecLerpAmount("Spec Lerp Amount", Range( 0 , 1)) = 0
		_dabs_texture("dabs_texture", 2D) = "white" {}
		[IntRange]_dabs_tiling("dabs_tiling", Range( 0 , 100)) = 35
		_dabs_rotate("dabs_rotate", Range( 0 , 180)) = 15
		[Toggle]_Outline1("Outline", Float) = 0
		_OutlineColor("Outline Color", Color) = (0,0,0,0)
		_DistanceScale("Distance Scale", Float) = 20
		_NearLineWidth1("Near Line Width", Range( 0 , 0.5)) = 0.02
		_FarLineWidth1("Far Line Width", Range( 0 , 0.1)) = 0.05
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ }
		Cull Front
		CGPROGRAM
		#pragma target 3.0
		#pragma surface outlineSurf Outline nofog  keepalpha noshadow noambient novertexlights nolightmap nodynlightmap nodirlightmap nometa noforwardadd vertex:outlineVertexDataFunc 
		void outlineVertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float cameraDepthFade192 = (( -UnityObjectToViewPos( v.vertex.xyz ).z -_ProjectionParams.y - 0.0 ) / 1.0);
			float lerpResult197 = lerp( _NearLineWidth1 , _FarLineWidth1 , saturate( ( cameraDepthFade192 / _DistanceScale ) ));
			float outlineVar = (( _Outline1 )?( lerpResult197 ):( 0.0 ));
			v.vertex.xyz *= ( 1 + outlineVar);
		}
		inline half4 LightingOutline( SurfaceOutput s, half3 lightDir, half atten ) { return half4 ( 0,0,0, s.Alpha); }
		void outlineSurf( Input i, inout SurfaceOutput o )
		{
			o.Emission = _OutlineColor.rgb;
		}
		ENDCG
		

		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGINCLUDE
		#include "UnityPBSLighting.cginc"
		#include "UnityCG.cginc"
		#include "UnityShaderVariables.cginc"
		#include "Lighting.cginc"
		#pragma target 5.0
		#ifdef UNITY_PASS_SHADOWCASTER
			#undef INTERNAL_DATA
			#undef WorldReflectionVector
			#undef WorldNormalVector
			#define INTERNAL_DATA half3 internalSurfaceTtoW0; half3 internalSurfaceTtoW1; half3 internalSurfaceTtoW2;
			#define WorldReflectionVector(data,normal) reflect (data.worldRefl, half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal)))
			#define WorldNormalVector(data,normal) half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal))
		#endif
		struct Input
		{
			float3 worldNormal;
			INTERNAL_DATA
			float2 uv_texcoord;
			float3 worldPos;
		};

		struct SurfaceOutputCustomLightingCustom
		{
			half3 Albedo;
			half3 Normal;
			half3 Emission;
			half Metallic;
			half Smoothness;
			half Occlusion;
			half Alpha;
			Input SurfInput;
			UnityGIInput GIData;
		};

		uniform sampler2D _ToonRamp;
		uniform sampler2D _Normal_Map;
		uniform float4 _Normal_Map_ST;
		uniform float _ShadowOffsetFloat;
		uniform float4 _Tint;
		uniform sampler2D _Albedo;
		uniform float4 _Albedo_ST;
		uniform float _RimOffset;
		uniform float _RimPower;
		uniform float4 _RimTint;
		uniform float _min;
		uniform float _max;
		uniform float _SpecularGloss;
		uniform sampler2D _SpecularMap;
		uniform float4 _SpecularMap_ST;
		uniform float4 _Spec_Color;
		uniform float _SpecLerpAmount;
		uniform float _SpecularIntensity;
		uniform sampler2D _dabs_texture;
		uniform float _dabs_tiling;
		uniform float _dabs_rotate;
		uniform float _Outline1;
		uniform float _NearLineWidth1;
		uniform float _FarLineWidth1;
		uniform float _DistanceScale;
		uniform float4 _OutlineColor;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float3 OutlineEffect181 = 0;
			v.vertex.xyz += OutlineEffect181;
		}

		inline half4 LightingStandardCustomLighting( inout SurfaceOutputCustomLightingCustom s, half3 viewDir, UnityGI gi )
		{
			UnityGIInput data = s.GIData;
			Input i = s.SurfInput;
			half4 c = 0;
			#ifdef UNITY_PASS_FORWARDBASE
			float ase_lightAtten = data.atten;
			if( _LightColor0.a == 0)
			ase_lightAtten = 0;
			#else
			float3 ase_lightAttenRGB = gi.light.color / ( ( _LightColor0.rgb ) + 0.000001 );
			float ase_lightAtten = max( max( ase_lightAttenRGB.r, ase_lightAttenRGB.g ), ase_lightAttenRGB.b );
			#endif
			#if defined(HANDLE_SHADOWS_BLENDING_IN_GI)
			half bakedAtten = UnitySampleBakedOcclusion(data.lightmapUV.xy, data.worldPos);
			float zDist = dot(_WorldSpaceCameraPos - data.worldPos, UNITY_MATRIX_V[2].xyz);
			float fadeDist = UnityComputeShadowFadeDistance(data.worldPos, zDist);
			ase_lightAtten = UnityMixRealtimeAndBakedShadows(data.atten, bakedAtten, UnityComputeShadowFade(fadeDist));
			#endif
			float2 uv_Normal_Map = i.uv_texcoord * _Normal_Map_ST.xy + _Normal_Map_ST.zw;
			float4 normal48 = tex2D( _Normal_Map, uv_Normal_Map );
			float3 ase_worldPos = i.worldPos;
			#if defined(LIGHTMAP_ON) && UNITY_VERSION < 560 //aseld
			float3 ase_worldlightDir = 0;
			#else //aseld
			float3 ase_worldlightDir = Unity_SafeNormalize( UnityWorldSpaceLightDir( ase_worldPos ) );
			#endif //aseld
			float dotResult22 = dot( normalize( (WorldNormalVector( i , normal48.rgb )) ) , ase_worldlightDir );
			float normal_lightdir26 = dotResult22;
			float2 temp_cast_1 = ((normal_lightdir26*_ShadowOffsetFloat + _ShadowOffsetFloat)).xx;
			float2 uv_Albedo = i.uv_texcoord * _Albedo_ST.xy + _Albedo_ST.zw;
			float4 albedo55 = ( _Tint * tex2D( _Albedo, uv_Albedo ) );
			float4 shadow36 = ( tex2D( _ToonRamp, temp_cast_1 ) * albedo55 );
			#if defined(LIGHTMAP_ON) && ( UNITY_VERSION < 560 || ( defined(LIGHTMAP_SHADOW_MIXING) && !defined(SHADOWS_SHADOWMASK) && defined(SHADOWS_SCREEN) ) )//aselc
			float4 ase_lightColor = 0;
			#else //aselc
			float4 ase_lightColor = _LightColor0;
			#endif //aselc
			UnityGI gi63 = gi;
			float3 diffNorm63 = WorldNormalVector( i , normal48.rgb );
			gi63 = UnityGI_Base( data, 1, diffNorm63 );
			float3 indirectDiffuse63 = gi63.indirect.diffuse + diffNorm63 * 0.0001;
			float4 lighting62 = ( shadow36 * ( ase_lightColor * float4( ( indirectDiffuse63 + ase_lightAtten ) , 0.0 ) ) );
			float3 ase_worldViewDir = Unity_SafeNormalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float dotResult25 = dot( normalize( (WorldNormalVector( i , normal48.rgb )) ) , ase_worldViewDir );
			float normal_Viewdir29 = dotResult25;
			float4 RimLighting106 = ( saturate( ( pow( ( 1.0 - saturate( ( _RimOffset + normal_Viewdir29 ) ) ) , _RimPower ) * ( normal_lightdir26 * ase_lightAtten ) ) ) * ( ase_lightColor * _RimTint ) );
			float dotResult132 = dot( ( ase_worldViewDir + _WorldSpaceLightPos0.xyz ) , normalize( (WorldNormalVector( i , normal48.rgb )) ) );
			float smoothstepResult135 = smoothstep( _min , _max , pow( dotResult132 , _SpecularGloss ));
			float2 uv_SpecularMap = i.uv_texcoord * _SpecularMap_ST.xy + _SpecularMap_ST.zw;
			float4 lerpResult150 = lerp( _Spec_Color , ase_lightColor , _SpecLerpAmount);
			float4 SpecularLighting142 = ( ase_lightAtten * ( ( smoothstepResult135 * ( tex2D( _SpecularMap, uv_SpecularMap ) * lerpResult150 ) ) * _SpecularIntensity ) );
			float2 temp_cast_6 = (_dabs_tiling).xx;
			float2 uv_TexCoord157 = i.uv_texcoord * temp_cast_6;
			float cos161 = cos( radians( _dabs_rotate ) );
			float sin161 = sin( radians( _dabs_rotate ) );
			float2 rotator161 = mul( uv_TexCoord157 - float2( 0.5,0.5 ) , float2x2( cos161 , -sin161 , sin161 , cos161 )) + float2( 0.5,0.5 );
			float4 spec_dabs171 = ( SpecularLighting142 * tex2D( _dabs_texture, rotator161 ).b );
			c.rgb = ( ( lighting62 + RimLighting106 ) + spec_dabs171 ).rgb;
			c.a = 1;
			return c;
		}

		inline void LightingStandardCustomLighting_GI( inout SurfaceOutputCustomLightingCustom s, UnityGIInput data, inout UnityGI gi )
		{
			s.GIData = data;
		}

		void surf( Input i , inout SurfaceOutputCustomLightingCustom o )
		{
			o.SurfInput = i;
			o.Normal = float3(0,0,1);
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf StandardCustomLighting keepalpha fullforwardshadows vertex:vertexDataFunc 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 5.0
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float2 customPack1 : TEXCOORD1;
				float4 tSpace0 : TEXCOORD2;
				float4 tSpace1 : TEXCOORD3;
				float4 tSpace2 : TEXCOORD4;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				vertexDataFunc( v, customInputData );
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				half3 worldTangent = UnityObjectToWorldDir( v.tangent.xyz );
				half tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				half3 worldBinormal = cross( worldNormal, worldTangent ) * tangentSign;
				o.tSpace0 = float4( worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x );
				o.tSpace1 = float4( worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y );
				o.tSpace2 = float4( worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z );
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				return o;
			}
			half4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				surfIN.uv_texcoord = IN.customPack1.xy;
				float3 worldPos = float3( IN.tSpace0.w, IN.tSpace1.w, IN.tSpace2.w );
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = float3( IN.tSpace0.z, IN.tSpace1.z, IN.tSpace2.z );
				surfIN.internalSurfaceTtoW0 = IN.tSpace0.xyz;
				surfIN.internalSurfaceTtoW1 = IN.tSpace1.xyz;
				surfIN.internalSurfaceTtoW2 = IN.tSpace2.xyz;
				SurfaceOutputCustomLightingCustom o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputCustomLightingCustom, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18000
435;73;1072;655;1569.57;1607.893;3.961805;True;True
Node;AmplifyShaderEditor.CommentaryNode;67;-1587.403,-26.44946;Inherit;False;694.79;441.8033;Comment;2;47;48;Normal Map;1,1,1,1;0;0
Node;AmplifyShaderEditor.SamplerNode;47;-1537.403,23.55054;Inherit;True;Property;_Normal_Map;Normal_Map;4;0;Create;True;0;0;False;0;-1;None;None;True;0;False;bump;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;48;-1162.557,111.7478;Inherit;False;normal;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;31;-774.9788,429.4265;Inherit;False;656.7184;451.0589;Comment;4;23;24;25;29;Normal.ViewDir;1,1,1,1;0;0
Node;AmplifyShaderEditor.GetLocalVarNode;96;-984.136,600.6316;Inherit;False;48;normal;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;24;-707.0447,692.4854;Inherit;False;World;True;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.GetLocalVarNode;130;-899.3048,-183.8795;Inherit;False;48;normal;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.WorldNormalVector;23;-724.9788,518.2222;Inherit;False;True;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.CommentaryNode;30;-756.0356,-289.6815;Inherit;False;791.3931;478.1478;Comment;4;20;26;22;21;Normal.LightDir;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;146;-26.77859,2156.877;Inherit;False;2280.898;1389.712;Comment;23;124;125;136;135;133;134;141;140;151;137;126;129;132;127;142;152;139;138;145;144;148;149;150;Specular Lighting;1,1,1,1;0;0
Node;AmplifyShaderEditor.WorldSpaceLightPos;125;23.22141,2435.008;Inherit;False;0;3;FLOAT4;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.GetLocalVarNode;129;88.02747,2673.13;Inherit;False;48;normal;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.DotProductOpNode;25;-455.2471,589.1838;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;124;65.75446,2206.877;Inherit;False;World;True;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldNormalVector;20;-698.436,-239.6815;Inherit;False;True;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldSpaceLightDirHlpNode;21;-706.0356,4.466363;Inherit;False;True;1;0;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.DotProductOpNode;22;-388.6957,-152.7807;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;56;348.3728,-1348;Inherit;False;915.7958;540.7098;Comment;4;55;54;53;52;Albedo;1,1,1,1;0;0
Node;AmplifyShaderEditor.WorldNormalVector;127;245.0189,2618.667;Inherit;False;True;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RegisterLocalVarNode;29;-342.2604,479.4265;Inherit;False;normal_Viewdir;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;121;30.1251,1291.11;Inherit;False;1752.399;720.5275;Comment;16;105;103;100;116;120;104;118;106;99;101;117;115;114;113;102;119;Rim Light;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleAddOpNode;126;348.0193,2346.076;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;99;150.9037,1452.669;Inherit;False;29;normal_Viewdir;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.LightColorNode;149;1002.478,3258.723;Inherit;False;0;3;COLOR;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.DotProductOpNode;132;480.3332,2508.438;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;148;988.0076,3030.081;Inherit;False;Property;_Spec_Color;Spec_Color;13;0;Create;True;0;0;False;0;1,1,1,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;101;80.1251,1341.11;Inherit;False;Property;_RimOffset;Rim Offset;6;0;Create;True;0;0;False;0;0.6;1;0;3;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;52;398.3728,-1298;Inherit;False;Property;_Tint;Tint;0;0;Create;True;0;0;False;0;0.6226415,0.6226415,0.6226415,0;0.6226415,0.6226415,0.6226415,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;134;468.5943,2657.167;Inherit;False;Property;_SpecularGloss;Specular Gloss;8;0;Create;True;0;0;False;0;0.2;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;46;290.5221,-565.5997;Inherit;False;1137.372;638.7133;Comment;7;34;36;33;44;40;57;58;Shadow;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;26;-188.6424,-210.0079;Inherit;False;normal_lightdir;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;151;994.0109,3424.258;Inherit;False;Property;_SpecLerpAmount;Spec Lerp Amount;14;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;53;401.1803,-1037.29;Inherit;True;Property;_Albedo;Albedo;1;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;33;330.2868,-397.884;Inherit;False;26;normal_lightdir;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;144;948.273,2730.698;Inherit;True;Property;_SpecularMap;Specular Map;12;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;54;780.1803,-1075.29;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;150;1277.427,3082.176;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;44;316.9789,-135.8839;Inherit;False;Property;_ShadowOffsetFloat;ShadowOffsetFloat;3;0;Create;True;0;0;False;0;0.5;0;0;0.5;0;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;133;693.036,2512.451;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;137;776.6115,2779.332;Inherit;False;Property;_max;max;10;0;Create;True;0;0;False;0;1.12;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;136;776.6113,2666.961;Inherit;False;Property;_min;min;9;0;Create;True;0;0;False;0;1.1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;100;363.9594,1385.207;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ScaleAndOffsetNode;40;547.1154,-283.6864;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;1;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;55;1040.169,-1028.852;Inherit;False;albedo;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;69;192.0052,444.2209;Inherit;False;1185.091;689.0374;Comment;9;62;59;65;60;68;61;63;64;93;Lighting;1,1,1,1;0;0
Node;AmplifyShaderEditor.SmoothstepOpNode;135;903.0294,2500.411;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;102;501.0257,1381.613;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;152;1269.901,2946.426;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LightAttenuation;118;540.0034,1900.638;Inherit;False;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;117;520.3109,1736.507;Inherit;False;26;normal_lightdir;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;176;2261.104,2692.278;Inherit;False;1666.181;786.4619;Comment;10;164;158;162;156;168;169;157;161;165;171;Specular Paint Dabs;1,1,1,1;0;0
Node;AmplifyShaderEditor.GetLocalVarNode;58;853.6678,-235.381;Inherit;False;55;albedo;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;145;1424.218,2522.585;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;105;512.4811,1523.588;Inherit;False;Property;_RimPower;Rim Power;5;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;138;1273.506,2343.961;Inherit;False;Property;_SpecularIntensity;Specular Intensity;11;0;Create;True;0;0;False;0;0.2;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;103;647.0479,1378.909;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;93;207.4478,906.2361;Inherit;False;48;normal;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;34;721.8561,-464.6679;Inherit;True;Property;_ToonRamp;ToonRamp;2;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CameraDepthFade;192;263.8671,5068.072;Inherit;False;3;2;FLOAT3;0,0,0;False;0;FLOAT;1;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.IndirectDiffuseLighting;63;434.4915,887.9617;Inherit;False;Tangent;1;0;FLOAT3;0,0,1;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;164;2313.135,3362.74;Inherit;False;Property;_dabs_rotate;dabs_rotate;17;0;Create;True;0;0;False;0;15;0;0;180;0;1;FLOAT;0
Node;AmplifyShaderEditor.LightAttenuation;64;474.5449,1022.258;Inherit;False;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;191;391.4077,5247.945;Inherit;False;Property;_DistanceScale;Distance Scale;20;0;Create;True;0;0;False;0;20;20;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;139;1629.205,2514.142;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LightAttenuation;140;1644.918,2271.336;Inherit;False;0;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;104;807.904,1378.909;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;158;2311.104,3069.448;Inherit;False;Property;_dabs_tiling;dabs_tiling;16;1;[IntRange];Create;True;0;0;False;0;35;0;0;100;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;119;737.7955,1752.293;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;57;1033.023,-416.4388;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;65;707.7969,975.1366;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;120;972.6738,1398.74;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LightColorNode;113;1047.805,1631.505;Inherit;False;0;3;COLOR;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.ColorNode;115;982.2127,1786.85;Inherit;False;Property;_RimTint;Rim Tint;7;0;Create;True;0;0;False;0;0.003604494,0.5902472,0.764151,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;162;2667.027,3184.387;Inherit;False;Constant;_Vector0;Vector 0;16;0;Create;True;0;0;False;0;0.5,0.5;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.TextureCoordinatesNode;157;2605.025,3050.573;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LightColorNode;60;394.7716,661.1113;Inherit;True;0;3;COLOR;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleDivideOpNode;193;587.1932,5101.164;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RadiansOpNode;165;2690.13,3362.791;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;141;1825.514,2500.091;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;36;1169.494,-393.6799;Inherit;False;shadow;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RotatorNode;161;2917.825,3010.468;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SaturateNode;195;753.4076,5117.568;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;123;1171.51,1406.879;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;114;1233.019,1640.289;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;194;243.0077,4895.968;Inherit;False;Property;_NearLineWidth1;Near Line Width;21;0;Create;True;0;0;False;0;0.02;0.006;0;0.5;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;59;393.3197,494.2209;Inherit;False;36;shadow;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;68;894.8014,918.4769;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT3;0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;142;2008.118,2516.144;Inherit;False;SpecularLighting;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;196;237.2076,4980.711;Inherit;False;Property;_FarLineWidth1;Far Line Width;22;0;Create;True;0;0;False;0;0.05;0.0179;0;0.1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;200;565.5069,4824.505;Inherit;False;Constant;_Float1;Float 0;20;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;169;3169.619,2742.278;Inherit;False;142;SpecularLighting;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;156;3145.073,2966.355;Inherit;True;Property;_dabs_texture;dabs_texture;15;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;197;881.6079,5005.845;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;61;978.7461,595.8078;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;116;1396.762,1375.818;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;106;1558.524,1372.463;Inherit;False;RimLighting;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;168;3504.55,2834.932;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;198;207.6251,4711.002;Inherit;False;Property;_OutlineColor;Outline Color;19;0;Create;True;0;0;False;0;0,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;62;1153.096,605.2322;Inherit;False;lighting;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ToggleSwitchNode;201;716.6621,4755.151;Inherit;False;Property;_Outline1;Outline;18;0;Create;True;0;0;False;0;0;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;107;1470.534,186.306;Inherit;False;106;RimLighting;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;171;3703.285,2833.653;Inherit;False;spec_dabs;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;37;1494.123,4.333431;Inherit;False;62;lighting;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.OutlineNode;199;921.5026,4639.073;Inherit;False;1;True;None;0;0;Front;3;0;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.CommentaryNode;189;-66.01164,4018.968;Inherit;False;1087.276;497.481;Comment;1;181;Outline;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleAddOpNode;122;1634.683,103.1243;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;172;1568.966,445.3972;Inherit;False;171;spec_dabs;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;181;797.2643,4183.026;Inherit;False;OutlineEffect;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;147;1688.364,233.1999;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;182;1589.298,547.6257;Inherit;False;181;OutlineEffect;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;143;1442.081,348.8783;Inherit;False;142;SpecularLighting;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;177;1821.627,153.9669;Float;False;True;-1;7;ASEMaterialInspector;0;0;CustomLighting;ToonShader;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;48;0;47;0
WireConnection;23;0;96;0
WireConnection;25;0;23;0
WireConnection;25;1;24;0
WireConnection;20;0;130;0
WireConnection;22;0;20;0
WireConnection;22;1;21;0
WireConnection;127;0;129;0
WireConnection;29;0;25;0
WireConnection;126;0;124;0
WireConnection;126;1;125;1
WireConnection;132;0;126;0
WireConnection;132;1;127;0
WireConnection;26;0;22;0
WireConnection;54;0;52;0
WireConnection;54;1;53;0
WireConnection;150;0;148;0
WireConnection;150;1;149;0
WireConnection;150;2;151;0
WireConnection;133;0;132;0
WireConnection;133;1;134;0
WireConnection;100;0;101;0
WireConnection;100;1;99;0
WireConnection;40;0;33;0
WireConnection;40;1;44;0
WireConnection;40;2;44;0
WireConnection;55;0;54;0
WireConnection;135;0;133;0
WireConnection;135;1;136;0
WireConnection;135;2;137;0
WireConnection;102;0;100;0
WireConnection;152;0;144;0
WireConnection;152;1;150;0
WireConnection;145;0;135;0
WireConnection;145;1;152;0
WireConnection;103;0;102;0
WireConnection;34;1;40;0
WireConnection;63;0;93;0
WireConnection;139;0;145;0
WireConnection;139;1;138;0
WireConnection;104;0;103;0
WireConnection;104;1;105;0
WireConnection;119;0;117;0
WireConnection;119;1;118;0
WireConnection;57;0;34;0
WireConnection;57;1;58;0
WireConnection;65;0;63;0
WireConnection;65;1;64;0
WireConnection;120;0;104;0
WireConnection;120;1;119;0
WireConnection;157;0;158;0
WireConnection;193;0;192;0
WireConnection;193;1;191;0
WireConnection;165;0;164;0
WireConnection;141;0;140;0
WireConnection;141;1;139;0
WireConnection;36;0;57;0
WireConnection;161;0;157;0
WireConnection;161;1;162;0
WireConnection;161;2;165;0
WireConnection;195;0;193;0
WireConnection;123;0;120;0
WireConnection;114;0;113;0
WireConnection;114;1;115;0
WireConnection;68;0;60;0
WireConnection;68;1;65;0
WireConnection;142;0;141;0
WireConnection;156;1;161;0
WireConnection;197;0;194;0
WireConnection;197;1;196;0
WireConnection;197;2;195;0
WireConnection;61;0;59;0
WireConnection;61;1;68;0
WireConnection;116;0;123;0
WireConnection;116;1;114;0
WireConnection;106;0;116;0
WireConnection;168;0;169;0
WireConnection;168;1;156;3
WireConnection;62;0;61;0
WireConnection;201;0;200;0
WireConnection;201;1;197;0
WireConnection;171;0;168;0
WireConnection;199;0;198;0
WireConnection;199;1;201;0
WireConnection;122;0;37;0
WireConnection;122;1;107;0
WireConnection;181;0;199;0
WireConnection;147;0;122;0
WireConnection;147;1;172;0
WireConnection;177;13;147;0
WireConnection;177;11;182;0
ASEEND*/
//CHKSM=EEBD42320916E04F652F54EB2209E24E55F195DC