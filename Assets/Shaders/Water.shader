// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Water"
{
	Properties
	{
		_CloseColor("Close Color", Color) = (0,0.7231321,1,0)
		_FarColor("Far Color", Color) = (0,0,0,0)
		_FoamPower("Foam Power", Range( 0 , 1)) = 0
		_Depth("Depth", Range( 0 , 5)) = 0
		_WaveHeight("Wave Height", Float) = 1
		_WaveSpeed("Wave Speed", Range( 0 , 2)) = 0
		_Smoothness("Smoothness", Range( 0 , 1)) = 0
		_FoamSpeed("Foam Speed", Float) = 1
		_DistanceDensity("Distance Density", Range( 0 , 1)) = 500
		_WaveNormalStrength("Wave Normal Strength", Float) = 0
		_Float2("Float 2", Float) = 0
		_TextureSample0("Texture Sample 0", 2D) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
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
		#pragma target 3.0
		struct Input
		{
			float3 worldPos;
			float4 screenPos;
			float3 viewDir;
			float3 worldNormal;
			float2 uv_texcoord;
		};

		uniform float _WaveHeight;
		uniform float4 _FarColor;
		uniform float4 _CloseColor;
		uniform float _DistanceDensity;
		uniform float _FoamSpeed;
		UNITY_DECLARE_DEPTH_TEXTURE( _CameraDepthTexture );
		uniform float4 _CameraDepthTexture_TexelSize;
		uniform float _Depth;
		uniform float _FoamPower;
		uniform float _Float2;
		uniform sampler2D _TextureSample0;
		uniform float _WaveSpeed;
		uniform float _WaveNormalStrength;
		uniform float _Smoothness;


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
			float Noise202 = 0.0;
			float3 appendResult31 = (float3(0.0 , Noise202 , 0.0));
			v.vertex.xyz += ( appendResult31 * _WaveHeight );
			v.vertex.w = 1;
		}

		void surf( Input i , inout SurfaceOutputStandardSpecular o )
		{
			float4 color7 = IsGammaSpace() ? float4(0.9,0.9,0.9,0) : float4(0.7874123,0.7874123,0.7874123,0);
			float3 ase_worldPos = i.worldPos;
			float DistanceMask193 = exp( ( -_DistanceDensity * length( ( ase_worldPos - _WorldSpaceCameraPos ) ) ) );
			float4 lerpResult194 = lerp( _FarColor , _CloseColor , DistanceMask193);
			float time42 = ( _Time.y * _FoamSpeed );
			float2 appendResult47 = (float2(ase_worldPos.x , ase_worldPos.z));
			float2 coords42 = appendResult47 * 5.0;
			float2 id42 = 0;
			float2 uv42 = 0;
			float voroi42 = voronoi42( coords42, time42, id42, uv42, 0 );
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float screenDepth207 = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE( _CameraDepthTexture, ase_screenPosNorm.xy ));
			float distanceDepth207 = abs( ( screenDepth207 - LinearEyeDepth( ase_screenPosNorm.z ) ) / ( _Depth ) );
			float Foam205 = step( ( 1.0 - voroi42 ) , pow( distanceDepth207 , _FoamPower ) );
			float4 lerpResult48 = lerp( color7 , lerpResult194 , Foam205);
			#if defined(LIGHTMAP_ON) && UNITY_VERSION < 560 //aseld
			float3 ase_worldlightDir = 0;
			#else //aseld
			float3 ase_worldlightDir = normalize( UnityWorldSpaceLightDir( ase_worldPos ) );
			#endif //aseld
			float3 normalizeResult4_g32 = normalize( ( i.viewDir + ase_worldlightDir ) );
			float3 ase_worldNormal = i.worldNormal;
			float3 ase_vertexNormal = mul( unity_WorldToObject, float4( ase_worldNormal, 0 ) );
			float dotResult459 = dot( normalizeResult4_g32 , ase_vertexNormal );
			float smoothstepResult461 = smoothstep( 0.9 , 1.0 , dotResult459);
			float Time222 = ( _Time.x * _WaveSpeed );
			float2 temp_cast_0 = (Time222).xx;
			float2 uv_TexCoord474 = i.uv_texcoord * float2( 25,25 ) + temp_cast_0;
			float temp_output_450_0 = pow( tex2D( _TextureSample0, uv_TexCoord474 ).r , _WaveNormalStrength );
			o.Albedo = saturate( ( lerpResult48 + ( smoothstepResult461 * step( pow( dotResult459 , _Float2 ) , temp_output_450_0 ) ) ) ).rgb;
			o.Smoothness = _Smoothness;
			o.Alpha = 1;
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
			#pragma target 3.0
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
				float3 worldPos : TEXCOORD2;
				float4 screenPos : TEXCOORD3;
				float3 worldNormal : TEXCOORD4;
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
				o.worldNormal = worldNormal;
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
				o.worldPos = worldPos;
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
				surfIN.uv_texcoord = IN.customPack1.xy;
				float3 worldPos = IN.worldPos;
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.viewDir = worldViewDir;
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = IN.worldNormal;
				surfIN.screenPos = IN.screenPos;
				SurfaceOutputStandardSpecular o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputStandardSpecular, o )
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
Version=18712
1920;0;1920;1059;1753.858;137.3936;1;True;True
Node;AmplifyShaderEditor.CommentaryNode;200;-2231.348,1059.842;Inherit;False;1468.378;775.8786;Comment;9;187;182;184;196;183;181;193;185;186;;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;201;-2681.406,273.9269;Inherit;False;1361.849;550.9315;Comment;11;34;98;35;202;222;425;427;428;430;431;405;;1,1,1,1;0;0
Node;AmplifyShaderEditor.WorldSpaceCameraPos;186;-2181.348,1652.72;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldPosInputsNode;185;-2125.448,1479.82;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.TimeNode;55;-3000.125,-776.9795;Inherit;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;96;-3017.929,-603.6078;Inherit;False;Property;_FoamSpeed;Foam Speed;8;0;Create;True;0;0;0;False;0;False;1;0.25;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.WorldPosInputsNode;46;-2793.452,-895.1074;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;182;-1892.277,1213.549;Inherit;False;Property;_DistanceDensity;Distance Density;10;0;Create;True;0;0;0;False;0;False;500;0.1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;187;-1663.302,1543.122;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.TimeNode;98;-2622.167,448.5887;Inherit;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;34;-2650.406,710.8585;Inherit;False;Property;_WaveSpeed;Wave Speed;6;0;Create;True;0;0;0;False;0;False;0;2;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.NegateNode;196;-1565.141,1212.439;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;35;-2362.725,580.8091;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LengthOpNode;184;-1556.703,1339.022;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;56;-2746.416,-718.5729;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;47;-2610.151,-908.1071;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;18;-2847.403,-500.7164;Inherit;False;Property;_Depth;Depth;4;0;Create;True;0;0;0;False;0;False;0;1;0;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;94;-2540.961,-644.5221;Inherit;False;Property;_FoamPower;Foam Power;3;0;Create;True;0;0;0;False;0;False;0;0.3;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.VoronoiNode;42;-2456.295,-928.8101;Inherit;True;0;1;1;3;1;False;3;False;False;4;0;FLOAT2;0,0;False;1;FLOAT;0.26;False;2;FLOAT;5;False;3;FLOAT;0;False;3;FLOAT;0;FLOAT2;1;FLOAT2;2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;183;-1416.302,1189.522;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DepthFade;207;-2488.811,-540.4614;Inherit;False;True;False;True;2;1;FLOAT3;0,0,0;False;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;475;-2196.858,130.6064;Inherit;False;Constant;_Vector0;Vector 0;17;0;Create;True;0;0;0;False;0;False;25,25;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.RegisterLocalVarNode;222;-2188.511,686.3024;Inherit;False;Time;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;474;-1975.858,143.6064;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ExpOpNode;181;-1227.802,1124.522;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NormalVertexDataNode;460;-847.8677,390.2689;Inherit;False;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PowerNode;93;-2227.197,-789.7139;Inherit;True;False;2;0;FLOAT;0;False;1;FLOAT;0.42;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;458;-819.318,250.6307;Inherit;False;Blinn-Phong Half Vector;-1;;32;91a149ac9d615be429126c95e20753ce;0;0;1;FLOAT3;0
Node;AmplifyShaderEditor.OneMinusNode;87;-2235.282,-883.6979;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;466;-1271.156,193.2994;Inherit;False;Property;_Float2;Float 2;15;0;Create;True;0;0;0;False;0;False;0;-25;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;468;-1671.568,119.5082;Inherit;True;Property;_TextureSample0;Texture Sample 0;16;0;Create;True;0;0;0;False;0;False;-1;None;c37e97c8024b1c84aa7d45200a9a8c9b;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;405;-1543.323,586.8987;Inherit;False;Property;_WaveNormalStrength;Wave Normal Strength;13;0;Create;True;0;0;0;False;0;False;0;-3.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;193;-986.969,1109.841;Inherit;False;DistanceMask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;52;-1977.047,-826.6432;Inherit;True;2;0;FLOAT;0.39;False;1;FLOAT;0.6;False;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;459;-588.4691,326.2055;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;6;-1219.869,-365.8822;Inherit;False;Property;_CloseColor;Close Color;0;0;Create;True;0;0;0;False;0;False;0,0.7231321,1,0;0.2909926,0.8034455,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;192;-1262.553,-192.9543;Inherit;False;Property;_FarColor;Far Color;1;0;Create;True;0;0;0;False;0;False;0,0,0,0;0.09899337,0.2862594,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PowerNode;465;-433.1561,406.2994;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;195;-1249.845,-13.08221;Inherit;False;193;DistanceMask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;450;-1285.126,305.8725;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;205;-1728.064,-894.7114;Inherit;False;Foam;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;194;-892.1763,-198.4009;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;206;-967.3059,-355.9927;Inherit;False;205;Foam;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;202;-1746.445,702.2274;Inherit;False;Noise;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;461;-402.9323,251.1082;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0.9;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;463;-289.7948,534.8247;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;7;-1204.838,-537.9887;Inherit;False;Constant;_Color0;Color 0;0;0;Create;True;0;0;0;False;0;False;0.9,0.9,0.9,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;204;353.0406,550.4502;Inherit;False;202;Noise;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;462;-118.7234,391.896;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;48;-636.6526,-409.8705;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;301;695.4984,-322.4672;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;455;-509.0977,1039.603;Inherit;False;1591.486;683.6069;Comment;13;447;298;294;299;295;297;305;368;304;396;446;451;300;;1,1,1,1;0;0
Node;AmplifyShaderEditor.DynamicAppendNode;31;552.1687,482.8468;Inherit;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;33;663.4988,597.8698;Inherit;False;Property;_WaveHeight;Wave Height;5;0;Create;True;0;0;0;False;0;False;1;-0.08;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;57;1428.185,-83.33109;Inherit;False;Property;_Smoothness;Smoothness;7;0;Create;True;0;0;0;False;0;False;0;0.533;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;97;135.0709,-233.2323;Inherit;False;Property;_Metallic;Metallic;9;0;Create;True;0;0;0;False;0;False;0;0.255;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.PosVertexDataNode;452;419.1412,320.6225;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;299;369.1815,1541.829;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;100;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;238;885.7306,-254.1807;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.PowerNode;297;475.4706,1354.083;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;304;647.3583,1293.205;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;213;458.5578,-1099.731;Inherit;False;193;DistanceMask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;298;25.74839,1607.21;Inherit;False;Property;_Smoth;Smoth;11;0;Create;True;0;0;0;False;0;False;0.5;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;434;265.0839,-101.8264;Inherit;False;Constant;_Float1;Float 1;17;0;Create;True;0;0;0;False;0;False;0.255;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;211;-455.4394,-148.1719;Inherit;False;193;DistanceMask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;447;-459.0977,1268.245;Inherit;False;NormalFromHeightSwiggies;-1;;26;5503043ebd93333488a25a0ccd478405;0;1;13;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;210;-162.7866,-236.2871;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;212;532.431,-1222.559;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;203;82.97716,-1191.448;Inherit;False;202;Noise;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;428;-2030.791,363.7072;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;32;958.1994,460.3704;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMaxOpNode;295;331.8834,1372.165;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldPosInputsNode;427;-2318.204,324.7828;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.FunctionNode;451;867.3881,1089.603;Inherit;False;Step Antialiasing;-1;;27;2a825e80dfb3290468194f83380797bd;0;2;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldPosInputsNode;454;593.2789,208.1163;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.FunctionNode;300;-148.8056,1289.752;Inherit;False;Blinn-Phong Half Vector;-1;;25;91a149ac9d615be429126c95e20753ce;0;0;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;85;468.8487,-996.6462;Inherit;False;Property;_NormalStrength;Normal Strength;2;0;Create;True;0;0;0;False;0;False;0;4.02;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;453;849.7825,246.2984;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.TransformDirectionNode;446;-75.96971,1125.663;Inherit;False;World;Tangent;False;Fast;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.DotProductOpNode;294;185.0599,1428.97;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;368;601.9771,1130.051;Inherit;False;Property;_Float0;Float 0;12;0;Create;True;0;0;0;False;0;False;0;0.05;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;396;909.2533,1268.563;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;430;-1854.757,547.7703;Inherit;False;Property;_WaveScale;Wave Scale;14;0;Create;True;0;0;0;False;0;False;0;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;58;749.1378,-1237.53;Inherit;True;Normal From Height;-1;;28;1942fe2c5f1a1f94881a33d532e4afeb;0;2;20;FLOAT;0;False;110;FLOAT;100;False;2;FLOAT3;40;FLOAT3;0
Node;AmplifyShaderEditor.SaturateNode;67;293.0878,-1226.267;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;431;-1854.38,370.0799;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;425;-1657.321,347.9041;Inherit;True;Gradient;True;True;2;0;FLOAT2;0,0;False;1;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;305;595.3636,1510.564;Inherit;False;193;DistanceMask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;469;-1065.858,467.6064;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;381;1031.636,-212.7979;Float;False;True;-1;2;ASEMaterialInspector;0;0;StandardSpecular;Water;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Transparent;0.5;True;True;0;False;Transparent;;Transparent;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;187;0;185;0
WireConnection;187;1;186;0
WireConnection;196;0;182;0
WireConnection;35;0;98;1
WireConnection;35;1;34;0
WireConnection;184;0;187;0
WireConnection;56;0;55;2
WireConnection;56;1;96;0
WireConnection;47;0;46;1
WireConnection;47;1;46;3
WireConnection;42;0;47;0
WireConnection;42;1;56;0
WireConnection;183;0;196;0
WireConnection;183;1;184;0
WireConnection;207;0;18;0
WireConnection;222;0;35;0
WireConnection;474;0;475;0
WireConnection;474;1;222;0
WireConnection;181;0;183;0
WireConnection;93;0;207;0
WireConnection;93;1;94;0
WireConnection;87;0;42;0
WireConnection;468;1;474;0
WireConnection;193;0;181;0
WireConnection;52;0;87;0
WireConnection;52;1;93;0
WireConnection;459;0;458;0
WireConnection;459;1;460;0
WireConnection;465;0;459;0
WireConnection;465;1;466;0
WireConnection;450;0;468;1
WireConnection;450;1;405;0
WireConnection;205;0;52;0
WireConnection;194;0;192;0
WireConnection;194;1;6;0
WireConnection;194;2;195;0
WireConnection;461;0;459;0
WireConnection;463;0;465;0
WireConnection;463;1;450;0
WireConnection;462;0;461;0
WireConnection;462;1;463;0
WireConnection;48;0;7;0
WireConnection;48;1;194;0
WireConnection;48;2;206;0
WireConnection;301;0;48;0
WireConnection;301;1;462;0
WireConnection;31;1;204;0
WireConnection;299;0;298;0
WireConnection;238;0;301;0
WireConnection;297;0;295;0
WireConnection;297;1;299;0
WireConnection;304;0;297;0
WireConnection;304;1;305;0
WireConnection;210;0;97;0
WireConnection;210;1;211;0
WireConnection;212;0;67;0
WireConnection;212;1;213;0
WireConnection;428;0;427;1
WireConnection;428;1;427;3
WireConnection;32;0;31;0
WireConnection;32;1;33;0
WireConnection;295;1;294;0
WireConnection;451;1;368;0
WireConnection;451;2;304;0
WireConnection;453;1;31;0
WireConnection;446;0;447;0
WireConnection;294;0;300;0
WireConnection;294;1;447;0
WireConnection;58;20;212;0
WireConnection;58;110;85;0
WireConnection;67;0;203;0
WireConnection;431;0;428;0
WireConnection;431;1;222;0
WireConnection;425;0;431;0
WireConnection;425;1;430;0
WireConnection;469;1;450;0
WireConnection;381;0;238;0
WireConnection;381;4;57;0
WireConnection;381;11;32;0
ASEEND*/
//CHKSM=3A2EDA3E1F4B5FCED6D8BF00377119971093D52E