// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "UIAnimations"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
		
		_StencilComp ("Stencil Comparison", Float) = 8
		_Stencil ("Stencil ID", Float) = 0
		_StencilOp ("Stencil Operation", Float) = 0
		_StencilWriteMask ("Stencil Write Mask", Float) = 255
		_StencilReadMask ("Stencil Read Mask", Float) = 255

		_ColorMask ("Color Mask", Float) = 15

		[Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
		_SliderMin("Slider Min", Float) = 0.075
		_SliderMax("Slider Max", Float) = 0.925
		_Float1("Float 1", Float) = 0.1
		_DefenseSize("Defense Size", Float) = 0
		_DefenseOffset("Defense Offset", Float) = 1
		_ThreatSize("Threat Size", Float) = 0
		_ThreatOffset("Threat Offset", Float) = 25
		_BarWidth("Bar Width", Float) = 0
		_Cutoff("Cutoff", Float) = 0
		_BarWidthMin("Bar Width Min", Float) = 0
		_BarWidthMax("Bar Width Max", Float) = 0
		_PanSpeed("Pan Speed", Range( 0 , 3)) = 0
		_Float3("Float 3", Float) = 15.21
		_Float4("Float 4", Float) = 4.2
		_ShadowMask("Shadow Mask", Float) = 0
		_Shadow1("Shadow 1", Float) = 0
		_Shadow2("Shadow 2", Float) = 0

	}

	SubShader
	{
		LOD 0

		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" "CanUseSpriteAtlas"="True" }
		
		Stencil
		{
			Ref [_Stencil]
			ReadMask [_StencilReadMask]
			WriteMask [_StencilWriteMask]
			CompFront [_StencilComp]
			PassFront [_StencilOp]
			FailFront Keep
			ZFailFront Keep
			CompBack Always
			PassBack Keep
			FailBack Keep
			ZFailBack Keep
		}


		Cull Off
		Lighting Off
		ZWrite Off
		ZTest [unity_GUIZTestMode]
		Blend SrcAlpha OneMinusSrcAlpha
		ColorMask [_ColorMask]

		
		Pass
		{
			Name "Default"
		CGPROGRAM
			
			#ifndef UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX
			#define UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input)
			#endif
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0

			#include "UnityCG.cginc"
			#include "UnityUI.cginc"

			#pragma multi_compile __ UNITY_UI_CLIP_RECT
			#pragma multi_compile __ UNITY_UI_ALPHACLIP
			
			#include "UnityShaderVariables.cginc"

			
			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color    : COLOR;
				half2 texcoord  : TEXCOORD0;
				float4 worldPosition : TEXCOORD1;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
				
			};
			
			uniform fixed4 _Color;
			uniform fixed4 _TextureSampleAdd;
			uniform float4 _ClipRect;
			uniform sampler2D _MainTex;
			uniform float _SliderMin;
			uniform float _SliderMax;
			uniform float _Float1;
			uniform float Stability;
			uniform float _PanSpeed;
			uniform float _DefenseOffset;
			uniform float _DefenseSize;
			uniform float _BarWidthMin;
			uniform float _BarWidthMax;
			uniform float _BarWidth;
			uniform float _ThreatOffset;
			uniform float _ThreatSize;
			uniform float _Cutoff;
			uniform float _Float4;
			uniform float _Float3;
			uniform float _ShadowMask;
			uniform float _Shadow1;
			uniform float _Shadow2;
			float3 mod2D289( float3 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }
			float2 mod2D289( float2 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }
			float3 permute( float3 x ) { return mod2D289( ( ( x * 34.0 ) + 1.0 ) * x ); }
			float snoise( float2 v )
			{
				const float4 C = float4( 0.211324865405187, 0.366025403784439, -0.577350269189626, 0.024390243902439 );
				float2 i = floor( v + dot( v, C.yy ) );
				float2 x0 = v - i + dot( i, C.xx );
				float2 i1;
				i1 = ( x0.x > x0.y ) ? float2( 1.0, 0.0 ) : float2( 0.0, 1.0 );
				float4 x12 = x0.xyxy + C.xxzz;
				x12.xy -= i1;
				i = mod2D289( i );
				float3 p = permute( permute( i.y + float3( 0.0, i1.y, 1.0 ) ) + i.x + float3( 0.0, i1.x, 1.0 ) );
				float3 m = max( 0.5 - float3( dot( x0, x0 ), dot( x12.xy, x12.xy ), dot( x12.zw, x12.zw ) ), 0.0 );
				m = m * m;
				m = m * m;
				float3 x = 2.0 * frac( p * C.www ) - 1.0;
				float3 h = abs( x ) - 0.5;
				float3 ox = floor( x + 0.5 );
				float3 a0 = x - ox;
				m *= 1.79284291400159 - 0.85373472095314 * ( a0 * a0 + h * h );
				float3 g;
				g.x = a0.x * x0.x + h.x * x0.y;
				g.yz = a0.yz * x12.xz + h.yz * x12.yw;
				return 130.0 * dot( m, g );
			}
			

			
			v2f vert( appdata_t IN  )
			{
				v2f OUT;
				UNITY_SETUP_INSTANCE_ID( IN );
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
				UNITY_TRANSFER_INSTANCE_ID(IN, OUT);
				OUT.worldPosition = IN.vertex;
				
				
				OUT.worldPosition.xyz +=  float3( 0, 0, 0 ) ;
				OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);

				OUT.texcoord = IN.texcoord;
				
				OUT.color = IN.color * _Color;
				return OUT;
			}

			fixed4 frag(v2f IN  ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

				float4 color285 = IsGammaSpace() ? float4(0,0.3496235,0.702,1) : float4(0,0.1002575,0.4508419,1);
				float4 DefenceColors302 = color285;
				float4 color291 = IsGammaSpace() ? float4(0.7924528,0,0,1) : float4(0.5911142,0,0,1);
				float4 ThreatColors303 = color291;
				float lerpResult108 = lerp( _SliderMin , ( _SliderMax + ( _Float1 * 0.5 ) ) , Stability);
				float2 appendResult50 = (float2(0.0 , _PanSpeed));
				float2 texCoord19 = IN.texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 panner49 = ( 1.0 * _Time.y * appendResult50 + texCoord19);
				float simplePerlin2D44 = snoise( panner49*2.0 );
				simplePerlin2D44 = simplePerlin2D44*0.5 + 0.5;
				float smoothstepResult42 = smoothstep( lerpResult108 , ( lerpResult108 + ( _Float1 * simplePerlin2D44 ) ) , texCoord19.x);
				float4 lerpResult190 = lerp( DefenceColors302 , ThreatColors303 , smoothstepResult42);
				float2 appendResult68 = (float2(_DefenseOffset , 2.0));
				float2 texCoord53 = IN.texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_56_0 = ( texCoord53.x * 26.0 );
				float temp_output_91_0 = ( texCoord53.y * 4.0 );
				float2 appendResult67 = (float2(temp_output_56_0 , temp_output_91_0));
				float temp_output_83_0 = ( distance( appendResult68 , appendResult67 ) - _DefenseSize );
				float clampResult61 = clamp( temp_output_56_0 , _BarWidthMin , _BarWidthMax );
				float2 appendResult64 = (float2(clampResult61 , 2.0));
				float temp_output_92_0 = ( distance( appendResult67 , appendResult64 ) - _BarWidth );
				float2 appendResult84 = (float2(temp_output_56_0 , temp_output_91_0));
				float2 appendResult85 = (float2(_ThreatOffset , 2.0));
				float temp_output_88_0 = ( distance( appendResult84 , appendResult85 ) - _ThreatSize );
				float4 appendResult140 = (float4(( ( 1.0 - temp_output_83_0 ) * texCoord53.y ) , ( ( 1.0 - temp_output_92_0 ) * texCoord53.y ) , ( ( 1.0 - temp_output_88_0 ) * texCoord53.y ) , 1.0));
				float4 break186 = appendResult140;
				float smoothstepResult299 = smoothstep( _Cutoff , 1.0 , ( 1.0 - min( min( temp_output_83_0 , temp_output_92_0 ) , temp_output_88_0 ) ));
				float Mask101 = smoothstepResult299;
				float3 appendResult252 = (float3(( break186.x * Mask101 ) , 0.0 , ( break186.z * Mask101 )));
				float3 PackedMaskedMasks251 = appendResult252;
				float3 break343 = PackedMaskedMasks251;
				float2 texCoord332 = IN.texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_335_0 = sin( ( ( texCoord332.x * _Float4 ) + _Float3 ) );
				float Shadowmask353 = ( ( saturate( break343.x ) + saturate( break343.z ) ) * temp_output_335_0 );
				float3 lerpResult354 = lerp( (lerpResult190).rgb , float3(0,0,0) , Shadowmask353);
				float3 Color100 = saturate( lerpResult354 );
				float3 temp_cast_0 = (_ShadowMask).xxx;
				float3 appendResult210 = (float3(temp_output_83_0 , temp_output_92_0 , temp_output_88_0));
				float3 PackedMasks211 = appendResult210;
				float3 smoothstepResult416 = smoothstep( float3( 0,0,0 ) , temp_cast_0 , (PackedMasks211).xyz);
				float3 break420 = saturate( smoothstepResult416 );
				float3 lerpResult422 = lerp( float3( 0.01,0.01,0.01 ) , Color100 , saturate( pow( pow( saturate( ( break420.x + break420.y + break420.z ) ) , _Shadow1 ) , _Shadow2 ) ));
				float4 appendResult120 = (float4(lerpResult422 , Mask101));
				
				half4 color = appendResult120;
				
				#ifdef UNITY_UI_CLIP_RECT
                color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
                #endif
				
				#ifdef UNITY_UI_ALPHACLIP
				clip (color.a - 0.001);
				#endif

				return color;
			}
		ENDCG
		}
	}
	CustomEditor "ASEMaterialInspector"
	
	
}
/*ASEBEGIN
Version=18921
1920;0;1920;1059;3091.072;152.3605;1;True;False
Node;AmplifyShaderEditor.RangedFloatNode;58;-3715.417,-1255.628;Inherit;False;Constant;_Float2;Float 2;2;0;Create;True;0;0;0;False;0;False;26;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;53;-3947.362,-1510.005;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;132;-2995.741,-1248.944;Inherit;False;Property;_BarWidthMin;Bar Width Min;9;0;Create;True;0;0;0;False;0;False;0;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;133;-2987.741,-1173.944;Inherit;False;Property;_BarWidthMax;Bar Width Max;10;0;Create;True;0;0;0;False;0;False;0;24;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;56;-3542.556,-1256.22;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;116;-2728.538,-1406.649;Inherit;False;872.2551;349.4607;Bar;6;64;63;92;60;93;61;;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;115;-2660.836,-1908.428;Inherit;False;754.5939;386.0146;Defense;7;118;68;67;71;83;76;73;;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;114;-2684.857,-1026.695;Inherit;False;718.6239;380.481;Threat;7;84;85;86;89;88;87;119;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;118;-2647.822,-1829.087;Inherit;False;Property;_DefenseOffset;Defense Offset;4;0;Create;True;0;0;0;False;0;False;1;1.4;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;61;-2678.538,-1317.557;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;1;False;2;FLOAT;25;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;91;-3407.953,-1579.714;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;4;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;119;-2668.849,-882.3743;Inherit;False;Property;_ThreatOffset;Threat Offset;6;0;Create;True;0;0;0;False;0;False;25;24.6;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;64;-2447.027,-1259.611;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;2;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;68;-2485.863,-1803.091;Inherit;False;FLOAT2;4;0;FLOAT;1;False;1;FLOAT;2;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;67;-2574.866,-1675.808;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;85;-2570.534,-805.1419;Inherit;False;FLOAT2;4;0;FLOAT;25;False;1;FLOAT;2;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;84;-2592.891,-976.695;Inherit;False;FLOAT2;4;0;FLOAT;1;False;1;FLOAT;0.5;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;93;-2299.273,-1173.189;Inherit;False;Property;_BarWidth;Bar Width;7;0;Create;True;0;0;0;False;0;False;0;0.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DistanceOpNode;73;-2356.345,-1765.574;Inherit;False;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DistanceOpNode;63;-2273.535,-1306.888;Inherit;False;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;76;-2412.795,-1653.864;Inherit;False;Property;_DefenseSize;Defense Size;3;0;Create;True;0;0;0;False;0;False;0;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;89;-2435.953,-762.2139;Inherit;False;Property;_ThreatSize;Threat Size;5;0;Create;True;0;0;0;False;0;False;0;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DistanceOpNode;86;-2416.336,-883.8401;Inherit;False;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;92;-2148.843,-1303.894;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;83;-2228.18,-1689.721;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;88;-2288.17,-807.9872;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMinOpNode;82;-1779.183,-1354.261;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMinOpNode;94;-1662.54,-1345.604;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;149;-1737.553,-850.1884;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;95;-1587.176,-1230.145;Inherit;False;Property;_Cutoff;Cutoff;8;0;Create;True;0;0;0;False;0;False;0;0.9;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;300;-1523.449,-1318.141;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;144;-1717.219,-1656.949;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;147;-1720.865,-1502.666;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;148;-1540.632,-1499.073;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;299;-1349.272,-1320.661;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;141;-1485.378,-1649.216;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;150;-1479.123,-896.5204;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;140;-1345.08,-1530.686;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;1;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;101;-1176.386,-1319.769;Inherit;False;Mask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;173;-1231.023,-1668.533;Inherit;False;101;Mask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;186;-1188.061,-1538.231;Inherit;False;FLOAT4;1;0;FLOAT4;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;175;-930.435,-1664.604;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;177;-926.3076,-1451.054;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;301;-2367.727,1013.999;Inherit;False;Property;_PanSpeed;Pan Speed;12;0;Create;True;0;0;0;False;0;False;0;0.15;0;3;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;252;-398.2357,-1580.099;Inherit;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;41;-1512.464,923.8068;Inherit;False;Property;_Float1;Float 1;2;0;Create;True;0;0;0;False;0;False;0.1;0.05;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;19;-2094.503,789.2147;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;50;-1988.933,995.326;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0.5;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;210;-1739.619,-1111.704;Inherit;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;251;-224.5254,-1546.905;Inherit;False;PackedMaskedMasks;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.CommentaryNode;352;-3258.282,37.28274;Inherit;False;907.8655;574.8445;Shadow Mask;12;338;336;339;337;335;341;328;343;348;351;350;332;;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;113;-2066.367,570.1153;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;211;-1544.409,-1109.75;Inherit;False;PackedMasks;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;332;-3208.282,289.9785;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PannerNode;49;-1789.234,977.8255;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;107;-2034.655,426.0905;Inherit;False;Property;_SliderMax;Slider Max;1;0;Create;True;0;0;0;False;0;False;0.925;0.875;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;328;-3169.393,98.78854;Inherit;True;251;PackedMaskedMasks;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;338;-3129.858,424.9841;Inherit;False;Property;_Float4;Float 4;15;0;Create;True;0;0;0;False;0;False;4.2;5.8;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;112;-1814.106,445.6816;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;339;-2970.122,294.7789;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;106;-1886.767,281.577;Inherit;False;Property;_SliderMin;Slider Min;0;0;Create;True;0;0;0;False;0;False;0.075;0.075;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;20;-1880.631,593.0428;Inherit;False;Global;Stability;Stability;0;0;Create;True;0;0;0;False;0;False;1;0.6;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;343;-2917.717,106.2827;Inherit;False;FLOAT3;1;0;FLOAT3;0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.GetLocalVarNode;397;616.7174,85.03387;Inherit;False;211;PackedMasks;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;44;-1569.434,1099.026;Inherit;True;Simplex2D;True;False;2;0;FLOAT2;0,0;False;1;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;336;-3109.724,496.1272;Inherit;False;Property;_Float3;Float 3;14;0;Create;True;0;0;0;False;0;False;15.21;14.4;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;45;-1295.993,953.4172;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;348;-2781.717,87.28274;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;337;-2776.828,280.0134;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;291;-135.1198,12.37366;Inherit;False;Constant;_Color4;Color 4;12;0;Create;True;0;0;0;False;0;False;0.7924528,0,0,1;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ComponentMaskNode;413;907.5056,84.72626;Inherit;False;True;True;True;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SaturateNode;351;-2784.033,154.8387;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;410;803.9075,249.0453;Inherit;False;Property;_ShadowMask;Shadow Mask;16;0;Create;True;0;0;0;False;0;False;0;-1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;285;-144.9173,-838.5121;Inherit;False;Constant;_Color2;Color 2;12;0;Create;True;0;0;0;False;0;False;0,0.3496235,0.702,1;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;108;-1452.914,501.3542;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;350;-2635.033,140.8388;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;303;342.9445,-99.05182;Inherit;False;ThreatColors;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SinOpNode;335;-2653.676,278.9055;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;43;-1052.133,961.3259;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;302;294.9445,-459.0518;Inherit;False;DefenceColors;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SmoothstepOpNode;416;1154.559,97.94904;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SmoothstepOpNode;42;-919.3307,785.843;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;305;-893.6731,463.342;Inherit;False;302;DefenceColors;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;341;-2535.417,219.8827;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;304;-887.6731,550.342;Inherit;False;303;ThreatColors;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;418;1306.559,97.94904;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.BreakToComponentsNode;420;1433.559,90.94904;Inherit;False;FLOAT3;1;0;FLOAT3;0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.LerpOp;190;-425.5276,734.3361;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;1,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;353;-2298.212,216.7157;Inherit;False;Shadowmask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;355;-298.2048,397.4672;Inherit;False;353;Shadowmask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;316;-240.4873,732.9601;Inherit;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.Vector3Node;356;-301.3264,533.2546;Inherit;False;Constant;_Vector0;Vector 0;15;0;Create;True;0;0;0;False;0;False;0,0,0;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleAddOpNode;419;1585.559,111.949;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;426;1615.559,33.94904;Inherit;False;Property;_Shadow1;Shadow 1;17;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;354;14.79517,654.4672;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SaturateNode;421;1702.559,103.949;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;431;1773.559,52.94904;Inherit;False;Property;_Shadow2;Shadow 2;18;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;429;1830.559,118.949;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;195;191.592,750.7363;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.PowerNode;430;1950.559,24.94904;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;100;386.7325,731.1537;Inherit;False;Color;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SaturateNode;427;1717.559,-29.05096;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;218;1082.665,-228.3562;Inherit;False;100;Color;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LerpOp;422;1361.559,-222.051;Inherit;False;3;0;FLOAT3;0.01,0.01,0.01;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;103;1009.966,-53.85251;Inherit;False;101;Mask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;327;-461.6021,1043.903;Inherit;False;Property;_Float0;Float 0;13;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;280;-150.3055,-351.7923;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;248;-1503.636,77.68979;Inherit;False;251;PackedMaskedMasks;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SaturateNode;349;-688.3105,-1300.809;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;290;178.1355,-92.28038;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.DynamicAppendNode;124;-3333.857,-1819.783;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SmoothstepOpNode;293;-606.7209,-333.4338;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0.5;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;297;-1062.717,-247.8733;Inherit;False;Property;_BarSmoothing;Bar Smoothing;11;0;Create;True;0;0;0;False;0;False;0;0.9;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;411;-999.1029,-402.3314;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ColorNode;286;-158.9173,-624.5121;Inherit;False;Constant;_Color3;Color 3;12;0;Create;True;0;0;0;False;0;False;0,0.2221255,0.446,1;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;292;-133.8255,183.0058;Inherit;False;Constant;_Color5;Color 5;12;0;Create;True;0;0;0;False;0;False;0.404,0,0,1;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;359;896.5851,338.3596;Inherit;False;357;ShadowMaskDebug;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;408;807.9075,10.04535;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.StepOpNode;279;-441.8491,-338.9123;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;357;-2287.706,385.1952;Inherit;False;ShadowMaskDebug;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;288;-67.79517,-91.70642;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;282;-234.9018,-170.3535;Inherit;False;FLOAT3;1;0;FLOAT3;0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.SaturateNode;71;-2036.488,-1749.573;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DistanceOpNode;365;1334.002,314.6362;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.FractNode;129;-3625.869,-1806.379;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;294;-613.3519,-88.24643;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;125;-3005.599,-1805.42;Inherit;False;DebugUV;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.LerpOp;306;-1089.911,-1007.1;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;213;-1218.922,-428.4159;Inherit;False;211;PackedMasks;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.OneMinusNode;412;795.5056,173.7263;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;281;-1102.675,-124.7487;Inherit;False;251;PackedMaskedMasks;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.BreakToComponentsNode;278;-848.3152,-401.2;Inherit;False;FLOAT3;1;0;FLOAT3;0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.SaturateNode;295;-359.9323,112.9685;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;131;1653.279,320.6309;Inherit;False;True;True;True;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SaturateNode;60;-2031.815,-1107.884;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;326;-314.6021,906.9026;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;283;116.7796,-468.7107;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.FractNode;130;-3524.132,-1728.579;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;87;-2118.641,-687.6981;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;330;-827.4614,-1786.274;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;120;1675.613,-126.567;Inherit;False;FLOAT4;4;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;1;False;1;FLOAT4;0
Node;AmplifyShaderEditor.BreakToComponentsNode;272;-1229.205,125.5453;Inherit;False;FLOAT3;1;0;FLOAT3;0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.GetLocalVarNode;126;880.2161,-224.1193;Inherit;False;125;DebugUV;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;310;-645.5956,925.4507;Inherit;False;101;Mask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;287;-390.3845,-50.8977;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;424;2121.559,109.949;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;1;False;1;FLOAT4;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;3;2213.31,-38.11879;Float;False;True;-1;2;ASEMaterialInspector;0;6;UIAnimations;5056123faa0c79b47ab6ad7e8bf059a4;True;Default;0;0;Default;2;False;True;2;5;False;-1;10;False;-1;0;1;False;-1;0;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;-1;False;True;True;True;True;True;0;True;-9;False;False;False;False;False;False;False;True;True;0;True;-5;255;True;-8;255;True;-7;0;True;-4;0;True;-6;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;False;True;2;False;-1;True;0;True;-11;False;True;5;Queue=Transparent=Queue=0;IgnoreProjector=True;RenderType=Transparent=RenderType;PreviewType=Plane;CanUseSpriteAtlas=True;False;False;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;0;;0;0;Standard;0;0;1;True;False;;False;0
WireConnection;56;0;53;1
WireConnection;56;1;58;0
WireConnection;61;0;56;0
WireConnection;61;1;132;0
WireConnection;61;2;133;0
WireConnection;91;0;53;2
WireConnection;64;0;61;0
WireConnection;68;0;118;0
WireConnection;67;0;56;0
WireConnection;67;1;91;0
WireConnection;85;0;119;0
WireConnection;84;0;56;0
WireConnection;84;1;91;0
WireConnection;73;0;68;0
WireConnection;73;1;67;0
WireConnection;63;0;67;0
WireConnection;63;1;64;0
WireConnection;86;0;84;0
WireConnection;86;1;85;0
WireConnection;92;0;63;0
WireConnection;92;1;93;0
WireConnection;83;0;73;0
WireConnection;83;1;76;0
WireConnection;88;0;86;0
WireConnection;88;1;89;0
WireConnection;82;0;83;0
WireConnection;82;1;92;0
WireConnection;94;0;82;0
WireConnection;94;1;88;0
WireConnection;149;0;88;0
WireConnection;300;0;94;0
WireConnection;144;0;83;0
WireConnection;147;0;92;0
WireConnection;148;0;147;0
WireConnection;148;1;53;2
WireConnection;299;0;300;0
WireConnection;299;1;95;0
WireConnection;141;0;144;0
WireConnection;141;1;53;2
WireConnection;150;0;149;0
WireConnection;150;1;53;2
WireConnection;140;0;141;0
WireConnection;140;1;148;0
WireConnection;140;2;150;0
WireConnection;101;0;299;0
WireConnection;186;0;140;0
WireConnection;175;0;186;0
WireConnection;175;1;173;0
WireConnection;177;0;186;2
WireConnection;177;1;173;0
WireConnection;252;0;175;0
WireConnection;252;2;177;0
WireConnection;50;1;301;0
WireConnection;210;0;83;0
WireConnection;210;1;92;0
WireConnection;210;2;88;0
WireConnection;251;0;252;0
WireConnection;113;0;41;0
WireConnection;211;0;210;0
WireConnection;49;0;19;0
WireConnection;49;2;50;0
WireConnection;112;0;107;0
WireConnection;112;1;113;0
WireConnection;339;0;332;1
WireConnection;339;1;338;0
WireConnection;343;0;328;0
WireConnection;44;0;49;0
WireConnection;45;0;41;0
WireConnection;45;1;44;0
WireConnection;348;0;343;0
WireConnection;337;0;339;0
WireConnection;337;1;336;0
WireConnection;413;0;397;0
WireConnection;351;0;343;2
WireConnection;108;0;106;0
WireConnection;108;1;112;0
WireConnection;108;2;20;0
WireConnection;350;0;348;0
WireConnection;350;1;351;0
WireConnection;303;0;291;0
WireConnection;335;0;337;0
WireConnection;43;0;108;0
WireConnection;43;1;45;0
WireConnection;302;0;285;0
WireConnection;416;0;413;0
WireConnection;416;2;410;0
WireConnection;42;0;19;1
WireConnection;42;1;108;0
WireConnection;42;2;43;0
WireConnection;341;0;350;0
WireConnection;341;1;335;0
WireConnection;418;0;416;0
WireConnection;420;0;418;0
WireConnection;190;0;305;0
WireConnection;190;1;304;0
WireConnection;190;2;42;0
WireConnection;353;0;341;0
WireConnection;316;0;190;0
WireConnection;419;0;420;0
WireConnection;419;1;420;1
WireConnection;419;2;420;2
WireConnection;354;0;316;0
WireConnection;354;1;356;0
WireConnection;354;2;355;0
WireConnection;421;0;419;0
WireConnection;429;0;421;0
WireConnection;429;1;426;0
WireConnection;195;0;354;0
WireConnection;430;0;429;0
WireConnection;430;1;431;0
WireConnection;100;0;195;0
WireConnection;427;0;430;0
WireConnection;422;1;218;0
WireConnection;422;2;427;0
WireConnection;280;0;279;0
WireConnection;280;1;282;0
WireConnection;290;1;292;0
WireConnection;290;2;288;0
WireConnection;124;0;129;0
WireConnection;124;1;130;0
WireConnection;293;0;278;0
WireConnection;293;1;297;0
WireConnection;411;0;213;0
WireConnection;279;0;293;0
WireConnection;357;0;335;0
WireConnection;288;0;287;0
WireConnection;288;1;282;2
WireConnection;282;0;281;0
WireConnection;129;0;56;0
WireConnection;294;0;278;2
WireConnection;294;1;297;0
WireConnection;125;0;124;0
WireConnection;278;0;411;0
WireConnection;326;0;310;0
WireConnection;326;1;327;0
WireConnection;283;1;286;0
WireConnection;283;2;280;0
WireConnection;130;0;91;0
WireConnection;120;0;422;0
WireConnection;120;3;103;0
WireConnection;272;0;248;0
WireConnection;287;0;294;0
WireConnection;3;0;120;0
ASEEND*/
//CHKSM=6823D26AE8115F663223953C8D21419E9683CE40