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
		_SliderValue("Slider Value", Range( 0 , 1)) = 0
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
		_Float0("Float 0", Float) = 0
		_PanSpeed("Pan Speed", Range( 0 , 3)) = 0

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
			uniform float _Float0;
			uniform float _DefenseOffset;
			uniform float _DefenseSize;
			uniform float _BarWidthMin;
			uniform float _BarWidthMax;
			uniform float _BarWidth;
			uniform float _ThreatOffset;
			uniform float _ThreatSize;
			uniform float _Cutoff;
			uniform float _SliderMin;
			uniform float _SliderMax;
			uniform float _Float1;
			uniform float _SliderValue;
			uniform float _PanSpeed;
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

				float4 color285 = IsGammaSpace() ? float4(0,0.4998641,1,1) : float4(0,0.2139154,1,1);
				float4 color286 = IsGammaSpace() ? float4(0,0.2221255,0.446,1) : float4(0,0.04042199,0.167419,1);
				float2 appendResult68 = (float2(_DefenseOffset , 2.0));
				float2 texCoord53 = IN.texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_56_0 = ( texCoord53.x * 26.0 );
				float temp_output_91_0 = ( texCoord53.y * 4.0 );
				float2 appendResult67 = (float2(temp_output_56_0 , temp_output_91_0));
				float temp_output_83_0 = ( distance( appendResult68 , appendResult67 ) - _DefenseSize );
				float temp_output_71_0 = saturate( temp_output_83_0 );
				float clampResult61 = clamp( temp_output_56_0 , _BarWidthMin , _BarWidthMax );
				float2 appendResult64 = (float2(clampResult61 , 2.0));
				float temp_output_92_0 = ( distance( appendResult67 , appendResult64 ) - _BarWidth );
				float temp_output_60_0 = saturate( temp_output_92_0 );
				float2 appendResult84 = (float2(temp_output_56_0 , temp_output_91_0));
				float2 appendResult85 = (float2(_ThreatOffset , 2.0));
				float temp_output_88_0 = ( distance( appendResult84 , appendResult85 ) - _ThreatSize );
				float temp_output_87_0 = saturate( temp_output_88_0 );
				float3 appendResult210 = (float3(temp_output_71_0 , temp_output_60_0 , temp_output_87_0));
				float3 PackedMasks211 = appendResult210;
				float3 break278 = PackedMasks211;
				float smoothstepResult293 = smoothstep( _Float0 , 1.0 , break278.x);
				float4 appendResult140 = (float4(( ( 1.0 - temp_output_83_0 ) * texCoord53.y ) , ( ( 1.0 - temp_output_92_0 ) * texCoord53.y ) , ( ( 1.0 - temp_output_88_0 ) * texCoord53.y ) , 1.0));
				float4 break186 = appendResult140;
				float smoothstepResult299 = smoothstep( _Cutoff , 1.0 , ( 1.0 - min( min( temp_output_71_0 , temp_output_60_0 ) , temp_output_87_0 ) ));
				float Mask101 = smoothstepResult299;
				float3 appendResult252 = (float3(( break186.x * Mask101 ) , 0.0 , ( break186.z * Mask101 )));
				float3 PackedMaskedMasks251 = appendResult252;
				float3 break282 = PackedMaskedMasks251;
				float4 lerpResult283 = lerp( color285 , color286 , ( step( smoothstepResult293 , 0.0 ) * break282.x ));
				float4 color291 = IsGammaSpace() ? float4(0.7924528,0,0,1) : float4(0.5911142,0,0,1);
				float4 color292 = IsGammaSpace() ? float4(0.404,0,0,1) : float4(0.135689,0,0,1);
				float smoothstepResult294 = smoothstep( _Float0 , 1.0 , break278.z);
				float4 lerpResult290 = lerp( color291 , color292 , ( step( smoothstepResult294 , 0.0 ) * break282.z ));
				float lerpResult108 = lerp( _SliderMin , ( _SliderMax + ( _Float1 * 0.5 ) ) , _SliderValue);
				float2 appendResult50 = (float2(0.0 , _PanSpeed));
				float2 texCoord19 = IN.texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 panner49 = ( 1.0 * _Time.y * appendResult50 + texCoord19);
				float simplePerlin2D44 = snoise( panner49*2.0 );
				simplePerlin2D44 = simplePerlin2D44*0.5 + 0.5;
				float smoothstepResult42 = smoothstep( lerpResult108 , ( lerpResult108 + ( _Float1 * simplePerlin2D44 ) ) , texCoord19.x);
				float4 lerpResult190 = lerp( lerpResult283 , lerpResult290 , smoothstepResult42);
				float4 Color100 = lerpResult190;
				
				half4 color = ( Color100 * Mask101 );
				
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
1920;0;1920;1059;2691.727;-309.9988;1;True;False
Node;AmplifyShaderEditor.RangedFloatNode;58;-3715.417,-1255.628;Inherit;False;Constant;_Float2;Float 2;2;0;Create;True;0;0;0;False;0;False;26;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;53;-3947.362,-1510.005;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;115;-2660.836,-1908.428;Inherit;False;754.5939;386.0146;Defense;7;118;68;67;71;83;76;73;;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;56;-3542.556,-1256.22;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;132;-2995.741,-1248.944;Inherit;False;Property;_BarWidthMin;Bar Width Min;10;0;Create;True;0;0;0;False;0;False;0;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;133;-2987.741,-1173.944;Inherit;False;Property;_BarWidthMax;Bar Width Max;11;0;Create;True;0;0;0;False;0;False;0;24;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;116;-2728.538,-1406.649;Inherit;False;872.2551;349.4607;Bar;6;64;63;92;60;93;61;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;118;-2647.822,-1829.087;Inherit;False;Property;_DefenseOffset;Defense Offset;5;0;Create;True;0;0;0;False;0;False;1;1.4;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;114;-2684.857,-1026.695;Inherit;False;718.6239;380.481;Threat;7;84;85;86;89;88;87;119;;1,1,1,1;0;0
Node;AmplifyShaderEditor.ClampOpNode;61;-2678.538,-1317.557;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;1;False;2;FLOAT;25;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;91;-3407.953,-1579.714;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;4;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;68;-2485.863,-1803.091;Inherit;False;FLOAT2;4;0;FLOAT;1;False;1;FLOAT;2;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;119;-2668.849,-882.3743;Inherit;False;Property;_ThreatOffset;Threat Offset;7;0;Create;True;0;0;0;False;0;False;25;24.6;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;64;-2447.027,-1259.611;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;2;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;67;-2574.866,-1675.808;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;93;-2299.273,-1173.189;Inherit;False;Property;_BarWidth;Bar Width;8;0;Create;True;0;0;0;False;0;False;0;0.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DistanceOpNode;73;-2356.345,-1765.574;Inherit;False;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DistanceOpNode;63;-2273.535,-1306.888;Inherit;False;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;84;-2592.891,-976.695;Inherit;False;FLOAT2;4;0;FLOAT;1;False;1;FLOAT;0.5;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;85;-2570.534,-805.1419;Inherit;False;FLOAT2;4;0;FLOAT;25;False;1;FLOAT;2;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;76;-2412.795,-1653.864;Inherit;False;Property;_DefenseSize;Defense Size;4;0;Create;True;0;0;0;False;0;False;0;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;89;-2435.953,-762.2139;Inherit;False;Property;_ThreatSize;Threat Size;6;0;Create;True;0;0;0;False;0;False;0;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DistanceOpNode;86;-2416.336,-883.8401;Inherit;False;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;92;-2148.843,-1303.894;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;83;-2228.18,-1689.721;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;71;-2068.488,-1661.573;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;60;-2012.825,-1339.734;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;88;-2288.17,-807.9872;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;87;-2131.233,-767.445;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMinOpNode;82;-1779.183,-1354.261;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMinOpNode;94;-1662.54,-1345.604;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;147;-1700.865,-1508.666;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;144;-1672.219,-1658.949;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;95;-1662.976,-1212.945;Inherit;False;Property;_Cutoff;Cutoff;9;0;Create;True;0;0;0;False;0;False;0;0.9;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;149;-1737.553,-850.1884;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;300;-1552.449,-1331.141;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;141;-1485.378,-1649.216;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;150;-1479.123,-896.5204;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;148;-1500.746,-1474.089;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;299;-1457.272,-1323.661;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;101;-1280.505,-1336.656;Inherit;False;Mask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;140;-1342.08,-1523.686;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;1;False;1;FLOAT4;0
Node;AmplifyShaderEditor.GetLocalVarNode;173;-1231.023,-1668.533;Inherit;False;101;Mask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;210;-1761.719,-1077.904;Inherit;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.BreakToComponentsNode;186;-1188.061,-1538.231;Inherit;False;FLOAT4;1;0;FLOAT4;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;175;-930.435,-1664.604;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;301;-2367.727,1013.999;Inherit;False;Property;_PanSpeed;Pan Speed;13;0;Create;True;0;0;0;False;0;False;0;0.5;0;3;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;177;-916.3076,-1445.054;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;211;-1593.709,-1075.15;Inherit;False;PackedMasks;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.DynamicAppendNode;50;-1988.933,995.326;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0.5;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;213;-1103.222,-373.8159;Inherit;False;211;PackedMasks;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;19;-2094.503,789.2147;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;252;-398.2357,-1580.099;Inherit;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;41;-1512.464,923.8068;Inherit;False;Property;_Float1;Float 1;3;0;Create;True;0;0;0;False;0;False;0.1;0.05;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;113;-2066.367,570.1153;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;107;-2034.655,426.0905;Inherit;False;Property;_SliderMax;Slider Max;2;0;Create;True;0;0;0;False;0;False;0.925;0.875;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;251;-224.5254,-1546.905;Inherit;False;PackedMaskedMasks;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;297;-771.9323,-450.0315;Inherit;False;Property;_Float0;Float 0;12;0;Create;True;0;0;0;False;0;False;0;0.99;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;278;-778.5376,-297.8552;Inherit;False;FLOAT3;1;0;FLOAT3;0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.PannerNode;49;-1789.234,977.8255;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;112;-1814.106,445.6816;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;281;-894.5842,-111.3235;Inherit;False;251;PackedMaskedMasks;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;44;-1569.434,1099.026;Inherit;True;Simplex2D;True;False;2;0;FLOAT2;0,0;False;1;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;294;-564.7209,-82.43384;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;20;-1880.631,592.0428;Inherit;False;Property;_SliderValue;Slider Value;0;0;Create;True;0;0;0;False;0;False;0;0.3;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;293;-606.7209,-333.4338;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0.5;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;106;-1886.767,281.577;Inherit;False;Property;_SliderMin;Slider Min;1;0;Create;True;0;0;0;False;0;False;0.075;0.075;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;287;-344.6232,-61.06689;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;108;-1442.514,541.6542;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;279;-441.8491,-338.9123;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;282;-234.9018,-170.3535;Inherit;False;FLOAT3;1;0;FLOAT3;0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;45;-1295.993,953.4172;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;291;-61.79517,63.29358;Inherit;False;Constant;_Color4;Color 4;12;0;Create;True;0;0;0;False;0;False;0.7924528,0,0,1;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;285;-144.9173,-838.5121;Inherit;False;Constant;_Color2;Color 2;12;0;Create;True;0;0;0;False;0;False;0,0.4998641,1,1;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;292;-76.79517,254.2936;Inherit;False;Constant;_Color5;Color 5;12;0;Create;True;0;0;0;False;0;False;0.404,0,0,1;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;280;-150.3055,-351.7923;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;288;-67.79517,-91.70642;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;43;-1052.133,961.3259;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;286;-158.9173,-624.5121;Inherit;False;Constant;_Color3;Color 3;12;0;Create;True;0;0;0;False;0;False;0,0.2221255,0.446,1;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;290;119.0683,-130.9795;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SmoothstepOpNode;42;-919.3307,785.843;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;283;116.7796,-468.7107;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;190;-425.5276,734.3361;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;1,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;100;-49.26752,734.1537;Inherit;False;Color;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;103;1060.966,-105.8525;Inherit;False;101;Mask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;218;1057.665,-181.3562;Inherit;False;100;Color;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;126;977.2161,16.88071;Inherit;False;125;DebugUV;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;120;1448.578,2.028503;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;1;False;1;FLOAT4;0
Node;AmplifyShaderEditor.FractNode;130;-3524.132,-1728.579;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;124;-3333.857,-1819.783;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SaturateNode;295;-359.9323,112.9685;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FractNode;129;-3625.869,-1806.379;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;104;1227.12,-168.1612;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;125;-3005.599,-1805.42;Inherit;False;DebugUV;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SaturateNode;195;-157.408,604.7363;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.BreakToComponentsNode;272;-1229.205,125.5453;Inherit;False;FLOAT3;1;0;FLOAT3;0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.ComponentMaskNode;131;1197.493,85.58746;Inherit;False;True;True;True;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.GetLocalVarNode;248;-1503.636,77.68979;Inherit;False;251;PackedMaskedMasks;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;3;1726.31,-19.11879;Float;False;True;-1;2;ASEMaterialInspector;0;6;UIAnimations;5056123faa0c79b47ab6ad7e8bf059a4;True;Default;0;0;Default;2;False;True;2;5;False;-1;10;False;-1;0;1;False;-1;0;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;-1;False;True;True;True;True;True;0;True;-9;False;False;False;False;False;False;False;True;True;0;True;-5;255;True;-8;255;True;-7;0;True;-4;0;True;-6;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;False;True;2;False;-1;True;0;True;-11;False;True;5;Queue=Transparent=Queue=0;IgnoreProjector=True;RenderType=Transparent=RenderType;PreviewType=Plane;CanUseSpriteAtlas=True;False;False;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;0;;0;0;Standard;0;0;1;True;False;;False;0
WireConnection;56;0;53;1
WireConnection;56;1;58;0
WireConnection;61;0;56;0
WireConnection;61;1;132;0
WireConnection;61;2;133;0
WireConnection;91;0;53;2
WireConnection;68;0;118;0
WireConnection;64;0;61;0
WireConnection;67;0;56;0
WireConnection;67;1;91;0
WireConnection;73;0;68;0
WireConnection;73;1;67;0
WireConnection;63;0;67;0
WireConnection;63;1;64;0
WireConnection;84;0;56;0
WireConnection;84;1;91;0
WireConnection;85;0;119;0
WireConnection;86;0;84;0
WireConnection;86;1;85;0
WireConnection;92;0;63;0
WireConnection;92;1;93;0
WireConnection;83;0;73;0
WireConnection;83;1;76;0
WireConnection;71;0;83;0
WireConnection;60;0;92;0
WireConnection;88;0;86;0
WireConnection;88;1;89;0
WireConnection;87;0;88;0
WireConnection;82;0;71;0
WireConnection;82;1;60;0
WireConnection;94;0;82;0
WireConnection;94;1;87;0
WireConnection;147;0;92;0
WireConnection;144;0;83;0
WireConnection;149;0;88;0
WireConnection;300;0;94;0
WireConnection;141;0;144;0
WireConnection;141;1;53;2
WireConnection;150;0;149;0
WireConnection;150;1;53;2
WireConnection;148;0;147;0
WireConnection;148;1;53;2
WireConnection;299;0;300;0
WireConnection;299;1;95;0
WireConnection;101;0;299;0
WireConnection;140;0;141;0
WireConnection;140;1;148;0
WireConnection;140;2;150;0
WireConnection;210;0;71;0
WireConnection;210;1;60;0
WireConnection;210;2;87;0
WireConnection;186;0;140;0
WireConnection;175;0;186;0
WireConnection;175;1;173;0
WireConnection;177;0;186;2
WireConnection;177;1;173;0
WireConnection;211;0;210;0
WireConnection;50;1;301;0
WireConnection;252;0;175;0
WireConnection;252;2;177;0
WireConnection;113;0;41;0
WireConnection;251;0;252;0
WireConnection;278;0;213;0
WireConnection;49;0;19;0
WireConnection;49;2;50;0
WireConnection;112;0;107;0
WireConnection;112;1;113;0
WireConnection;44;0;49;0
WireConnection;294;0;278;2
WireConnection;294;1;297;0
WireConnection;293;0;278;0
WireConnection;293;1;297;0
WireConnection;287;0;294;0
WireConnection;108;0;106;0
WireConnection;108;1;112;0
WireConnection;108;2;20;0
WireConnection;279;0;293;0
WireConnection;282;0;281;0
WireConnection;45;0;41;0
WireConnection;45;1;44;0
WireConnection;280;0;279;0
WireConnection;280;1;282;0
WireConnection;288;0;287;0
WireConnection;288;1;282;2
WireConnection;43;0;108;0
WireConnection;43;1;45;0
WireConnection;290;0;291;0
WireConnection;290;1;292;0
WireConnection;290;2;288;0
WireConnection;42;0;19;1
WireConnection;42;1;108;0
WireConnection;42;2;43;0
WireConnection;283;0;285;0
WireConnection;283;1;286;0
WireConnection;283;2;280;0
WireConnection;190;0;283;0
WireConnection;190;1;290;0
WireConnection;190;2;42;0
WireConnection;100;0;190;0
WireConnection;130;0;91;0
WireConnection;124;0;129;0
WireConnection;124;1;130;0
WireConnection;129;0;56;0
WireConnection;104;0;218;0
WireConnection;104;1;103;0
WireConnection;125;0;124;0
WireConnection;195;0;190;0
WireConnection;272;0;248;0
WireConnection;3;0;104;0
ASEEND*/
//CHKSM=B420B04537C49F73A22C46566591D4916929E4A5