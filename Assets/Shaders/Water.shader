// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Water"
{
	Properties
	{
		_Opacity("Opacity", Range( 0 , 1)) = 0.9
		_Color("Color", Color) = (0,0.7231321,1,0)
		_Depth("Depth", Range( 0 , 5)) = 0
		_NormalStrength("Normal Strength", Float) = 0
		_FoamPower("Foam Power", Range( 0 , 1)) = 0
		_WaveHeight("Wave Height", Float) = 1
		_WaveSpeed("Wave Speed", Range( 0 , 2)) = 0
		_Smoothness("Smoothness", Range( 0 , 1)) = 0
		_FoamSpeed("Foam Speed", Float) = 1
		_Metallic("Metallic", Range( 0 , 1)) = 0
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" }
		Cull Back
		CGINCLUDE
		#include "UnityShaderVariables.cginc"
		#include "UnityCG.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 4.6
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
			float3 worldPos;
			float3 worldNormal;
			INTERNAL_DATA
			float4 screenPos;
		};

		uniform float _WaveSpeed;
		uniform float _WaveHeight;
		uniform float _NormalStrength;
		uniform float4 _Color;
		UNITY_DECLARE_DEPTH_TEXTURE( _CameraDepthTexture );
		uniform float4 _CameraDepthTexture_TexelSize;
		uniform float _Depth;
		uniform float _FoamSpeed;
		uniform float _FoamPower;
		uniform float _Metallic;
		uniform float _Smoothness;
		uniform float _Opacity;


		float2 UnityGradientNoiseDir( float2 p )
		{
			p = fmod(p , 289);
			float x = fmod((34 * p.x + 1) * p.x , 289) + p.y;
			x = fmod( (34 * x + 1) * x , 289);
			x = frac( x / 41 ) * 2 - 1;
			return normalize( float2(x - floor(x + 0.5 ), abs( x ) - 0.5 ) );
		}
		
		float UnityGradientNoise( float2 UV, float Scale )
		{
			float2 p = UV * Scale;
			float2 ip = floor( p );
			float2 fp = frac( p );
			float d00 = dot( UnityGradientNoiseDir( ip ), fp );
			float d01 = dot( UnityGradientNoiseDir( ip + float2( 0, 1 ) ), fp - float2( 0, 1 ) );
			float d10 = dot( UnityGradientNoiseDir( ip + float2( 1, 0 ) ), fp - float2( 1, 0 ) );
			float d11 = dot( UnityGradientNoiseDir( ip + float2( 1, 1 ) ), fp - float2( 1, 1 ) );
			fp = fp * fp * fp * ( fp * ( fp * 6 - 15 ) + 10 );
			return lerp( lerp( d00, d01, fp.y ), lerp( d10, d11, fp.y ), fp.x ) + 0.5;
		}


		float3 PerturbNormal107_g1( float3 surf_pos, float3 surf_norm, float height, float scale )
		{
			// "Bump Mapping Unparametrized Surfaces on the GPU" by Morten S. Mikkelsen
			float3 vSigmaS = ddx( surf_pos );
			float3 vSigmaT = ddy( surf_pos );
			float3 vN = surf_norm;
			float3 vR1 = cross( vSigmaT , vN );
			float3 vR2 = cross( vN , vSigmaS );
			float fDet = dot( vSigmaS , vR1 );
			float dBs = ddx( height );
			float dBt = ddy( height );
			float3 vSurfGrad = scale * 0.05 * sign( fDet ) * ( dBs * vR1 + dBt * vR2 );
			return normalize ( abs( fDet ) * vN - vSurfGrad );
		}


		float2 voronoihash42( float2 p )
		{
			
			p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
			return frac( sin( p ) *43758.5453);
		}


		float voronoi42( float2 v, float time, inout float2 id, inout float2 mr, float smoothness )
		{
			float2 n = floor( v );
			float2 f = frac( v );
			float F1 = 8.0;
			float F2 = 8.0; float2 mg = 0;
			for ( int j = -1; j <= 1; j++ )
			{
				for ( int i = -1; i <= 1; i++ )
			 	{
			 		float2 g = float2( i, j );
			 		float2 o = voronoihash42( n + g );
					o = ( sin( time + o * 6.2831 ) * 0.5 + 0.5 ); float2 r = f - g - o;
					float d = 0.707 * sqrt(dot( r, r ));
			 		if( d<F1 ) {
			 			F2 = F1;
			 			F1 = d; mg = g; mr = r; id = o;
			 		} else if( d<F2 ) {
			 			F2 = d;
			 		}
			 	}
			}
			return (F2 + F1) * 0.5;
		}


		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float temp_output_35_0 = ( _Time.x * _WaveSpeed );
			float2 temp_cast_0 = (temp_output_35_0).xx;
			float2 uv_TexCoord28 = v.texcoord.xy + temp_cast_0;
			float gradientNoise27 = UnityGradientNoise(( uv_TexCoord28 + temp_output_35_0 ),500.0);
			gradientNoise27 = gradientNoise27*0.5 + 0.5;
			float3 appendResult31 = (float3(0.0 , gradientNoise27 , 0.0));
			float3 temp_output_29_0 = ( float3( 0,0,0 ) + ( appendResult31 * _WaveHeight ) );
			v.vertex.xyz += temp_output_29_0;
			v.vertex.w = 1;
		}

		void surf( Input i , inout SurfaceOutputStandardSpecular o )
		{
			float3 ase_worldPos = i.worldPos;
			float3 surf_pos107_g1 = ase_worldPos;
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float3 surf_norm107_g1 = ase_worldNormal;
			float3 ase_vertex3Pos = mul( unity_WorldToObject, float4( i.worldPos , 1 ) );
			float height107_g1 = ase_vertex3Pos.y;
			float scale107_g1 = _NormalStrength;
			float3 localPerturbNormal107_g1 = PerturbNormal107_g1( surf_pos107_g1 , surf_norm107_g1 , height107_g1 , scale107_g1 );
			float3 ase_worldTangent = WorldNormalVector( i, float3( 1, 0, 0 ) );
			float3 ase_worldBitangent = WorldNormalVector( i, float3( 0, 1, 0 ) );
			float3x3 ase_worldToTangent = float3x3( ase_worldTangent, ase_worldBitangent, ase_worldNormal );
			float3 worldToTangentDir42_g1 = mul( ase_worldToTangent, localPerturbNormal107_g1);
			o.Normal = worldToTangentDir42_g1;
			float4 color7 = IsGammaSpace() ? float4(0.9,0.9,0.9,0) : float4(0.7874123,0.7874123,0.7874123,0);
			float4 blendOpSrc13 = float4( 0,0.1998745,0.277,1 );
			float4 blendOpDest13 = _Color;
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float screenDepth1 = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE( _CameraDepthTexture, ase_screenPosNorm.xy ));
			float distanceDepth1 = saturate( abs( ( screenDepth1 - LinearEyeDepth( ase_screenPosNorm.z ) ) / ( _Depth ) ) );
			float4 lerpBlendMode13 = lerp(blendOpDest13,abs( blendOpSrc13 - blendOpDest13 ),distanceDepth1);
			float4 temp_output_13_0 = ( saturate( lerpBlendMode13 ));
			float time42 = ( _Time.y * _FoamSpeed );
			float2 appendResult47 = (float2(ase_worldPos.x , ase_worldPos.z));
			float2 coords42 = appendResult47 * 5.0;
			float2 id42 = 0;
			float2 uv42 = 0;
			float voroi42 = voronoi42( coords42, time42, id42, uv42, 0 );
			float4 lerpResult48 = lerp( color7 , temp_output_13_0 , step( ( 1.0 - voroi42 ) , pow( distanceDepth1 , _FoamPower ) ));
			float4 lerpResult5 = lerp( lerpResult48 , temp_output_13_0 , round( distanceDepth1 ));
			o.Albedo = lerpResult5.rgb;
			float3 temp_cast_1 = (_Metallic).xxx;
			o.Specular = temp_cast_1;
			o.Smoothness = _Smoothness;
			o.Alpha = _Opacity;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf StandardSpecular alpha:fade keepalpha fullforwardshadows vertex:vertexDataFunc 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 4.6
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
			sampler3D _DitherMaskLOD;
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float4 screenPos : TEXCOORD1;
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
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				o.screenPos = ComputeScreenPos( o.pos );
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
				float3 worldPos = float3( IN.tSpace0.w, IN.tSpace1.w, IN.tSpace2.w );
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = float3( IN.tSpace0.z, IN.tSpace1.z, IN.tSpace2.z );
				surfIN.internalSurfaceTtoW0 = IN.tSpace0.xyz;
				surfIN.internalSurfaceTtoW1 = IN.tSpace1.xyz;
				surfIN.internalSurfaceTtoW2 = IN.tSpace2.xyz;
				surfIN.screenPos = IN.screenPos;
				SurfaceOutputStandardSpecular o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputStandardSpecular, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				half alphaRef = tex3D( _DitherMaskLOD, float3( vpos.xy * 0.25, o.Alpha * 0.9375 ) ).a;
				clip( alphaRef - 0.01 );
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18712
2041;99;1445;840;826.2554;340.9821;1.371011;True;True
Node;AmplifyShaderEditor.TimeNode;98;-2032.745,530.9809;Inherit;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;34;-2041.983,791.2507;Inherit;False;Property;_WaveSpeed;Wave Speed;6;0;Create;True;0;0;0;False;0;False;0;0.0025;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;96;-1812.011,-460.2595;Inherit;False;Property;_FoamSpeed;Foam Speed;8;0;Create;True;0;0;0;False;0;False;1;0.25;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;35;-1747.303,663.2013;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldPosInputsNode;46;-1587.534,-751.7591;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.TimeNode;55;-1794.207,-633.6312;Inherit;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;28;-1317.163,699.8085;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;47;-1404.233,-764.7588;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;56;-1540.498,-575.2246;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;18;-1362.373,96.24106;Inherit;False;Property;_Depth;Depth;2;0;Create;True;0;0;0;False;0;False;0;1;0;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;66;-1149.704,502.5193;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.VoronoiNode;42;-1250.378,-785.4618;Inherit;True;0;1;1;3;1;False;3;False;False;4;0;FLOAT2;0,0;False;1;FLOAT;0.26;False;2;FLOAT;5;False;3;FLOAT;0;False;3;FLOAT;0;FLOAT2;1;FLOAT2;2
Node;AmplifyShaderEditor.DepthFade;1;-1005.57,88.04207;Inherit;False;True;True;True;2;1;FLOAT3;0,0,0;False;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;94;-1335.044,-501.1738;Inherit;False;Property;_FoamPower;Foam Power;4;0;Create;True;0;0;0;False;0;False;0;0.3;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;93;-1021.279,-646.3656;Inherit;True;False;2;0;FLOAT;0;False;1;FLOAT;0.42;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;6;-1170.269,-128.5823;Inherit;False;Property;_Color;Color;1;0;Create;True;0;0;0;False;0;False;0,0.7231321,1,0;0,0.593851,0.823,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;87;-1029.364,-740.3497;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;27;-1001.734,527.0084;Inherit;True;Gradient;True;True;2;0;FLOAT2;0,0;False;1;FLOAT;500;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;31;-670.7631,621.4085;Inherit;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;33;-727.5883,791.0085;Inherit;False;Property;_WaveHeight;Wave Height;5;0;Create;True;0;0;0;False;0;False;1;0.25;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;7;-1145.414,-367.3719;Inherit;False;Constant;_Color0;Color 0;0;0;Create;True;0;0;0;False;0;False;0.9,0.9,0.9,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StepOpNode;52;-771.1306,-683.2949;Inherit;True;2;0;FLOAT;0.39;False;1;FLOAT;0.6;False;1;FLOAT;0
Node;AmplifyShaderEditor.BlendOpsNode;13;-887.978,-161.3829;Inherit;False;Difference;True;3;0;COLOR;0,0.1998745,0.277,1;False;1;COLOR;0,0.1443137,0.2,1;False;2;FLOAT;1;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;85;47.51061,550.4164;Inherit;False;Property;_NormalStrength;Normal Strength;3;0;Create;True;0;0;0;False;0;False;0;10;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;48;-671.653,-303.9705;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RoundOpNode;9;-648.8348,10.76335;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;32;-493.9879,751.0085;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.PosVertexDataNode;95;-171.2326,-144.2584;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FunctionNode;58;462.8943,305.4965;Inherit;True;Normal From Height;-1;;1;1942fe2c5f1a1f94881a33d532e4afeb;0;2;20;FLOAT;0;False;110;FLOAT;100;False;2;FLOAT3;40;FLOAT3;0
Node;AmplifyShaderEditor.PosVertexDataNode;64;-1632.006,406.3192;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;97;274.6684,-121.6199;Inherit;False;Property;_Metallic;Metallic;9;0;Create;True;0;0;0;False;0;False;0;0.255;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;5;-425.4415,-279.9375;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;67;-8.104645,286.8191;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;57;245.6898,-46.9523;Inherit;False;Property;_Smoothness;Smoothness;7;0;Create;True;0;0;0;False;0;False;0;0.8;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;10;-78.73928,45.207;Inherit;False;Property;_Opacity;Opacity;0;0;Create;True;0;0;0;False;0;False;0.9;0.9;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;65;-1339.504,454.4189;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ComponentMaskNode;61;-254.6033,320.4188;Inherit;True;False;True;False;True;1;0;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DistanceOpNode;63;-590.7039,373.8188;Inherit;False;2;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;29;-302.6538,155.7753;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;782.9642,-188.0142;Float;False;True;-1;6;ASEMaterialInspector;0;0;StandardSpecular;Water;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Transparent;0.5;True;True;0;False;Transparent;;Transparent;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;1;32;0;100;False;0.5;True;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;35;0;98;1
WireConnection;35;1;34;0
WireConnection;28;1;35;0
WireConnection;47;0;46;1
WireConnection;47;1;46;3
WireConnection;56;0;55;2
WireConnection;56;1;96;0
WireConnection;66;0;28;0
WireConnection;66;1;35;0
WireConnection;42;0;47;0
WireConnection;42;1;56;0
WireConnection;1;0;18;0
WireConnection;93;0;1;0
WireConnection;93;1;94;0
WireConnection;87;0;42;0
WireConnection;27;0;66;0
WireConnection;31;1;27;0
WireConnection;52;0;87;0
WireConnection;52;1;93;0
WireConnection;13;1;6;0
WireConnection;13;2;1;0
WireConnection;48;0;7;0
WireConnection;48;1;13;0
WireConnection;48;2;52;0
WireConnection;9;0;1;0
WireConnection;32;0;31;0
WireConnection;32;1;33;0
WireConnection;58;20;95;2
WireConnection;58;110;85;0
WireConnection;5;0;48;0
WireConnection;5;1;13;0
WireConnection;5;2;9;0
WireConnection;67;0;61;0
WireConnection;65;0;64;1
WireConnection;65;1;64;3
WireConnection;61;0;29;0
WireConnection;29;1;32;0
WireConnection;0;0;5;0
WireConnection;0;1;58;40
WireConnection;0;3;97;0
WireConnection;0;4;57;0
WireConnection;0;9;10;0
WireConnection;0;11;29;0
ASEEND*/
//CHKSM=9EFB930018C799DE00CE1D69BFFB41932959827B