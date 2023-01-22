// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "UIAnimationsBaked"
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
		_LineBlur("Line Blur", Range( 0 , 0.1)) = 0.01
		_YPanSpeed("Y Pan Speed", Range( 0 , 3)) = 0
		_XPanSpeed("X Pan Speed", Range( 0 , 3)) = 0
		_DefenseColor("Defense Color", Color) = (0,0.3496235,0.702,1)
		_ThreatColor("Threat Color", Color) = (0.7924528,0,0,1)
		_ShadingMultiplier("Shading Multiplier", Float) = 0.2
		_TextureSample0("Texture Sample 0", 2D) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}

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
			uniform half4 _DefenseColor;
			uniform half4 _ThreatColor;
			uniform half _SliderMin;
			uniform half _SliderMax;
			uniform half Stability;
			uniform half StabilityIntensity;
			uniform half _XPanSpeed;
			uniform half _YPanSpeed;
			uniform half _LineBlur;
			uniform half _ShadingMultiplier;
			uniform sampler2D _TextureSample0;
			uniform half4 _TextureSample0_ST;
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

				half4 DefenceColors302 = _DefenseColor;
				half4 ThreatColors303 = _ThreatColor;
				half lerpResult108 = lerp( _SliderMin , _SliderMax , Stability);
				half temp_output_584_0 = saturate( StabilityIntensity );
				half lerpResult578 = lerp( 0.01 , 0.03 , temp_output_584_0);
				half lerpResult579 = lerp( 1.0 , 2.0 , temp_output_584_0);
				half2 appendResult50 = (half2(_XPanSpeed , ( _YPanSpeed * lerpResult579 )));
				half2 texCoord19 = IN.texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				half2 panner49 = ( 1.0 * _Time.y * appendResult50 + texCoord19);
				half simplePerlin2D44 = snoise( panner49*2.0 );
				half temp_output_556_0 = ( lerpResult108 + ( lerpResult578 * simplePerlin2D44 ) );
				half smoothstepResult42 = smoothstep( ( temp_output_556_0 - ( _LineBlur * 0.5 ) ) , ( temp_output_556_0 + _LineBlur ) , (0.0 + (texCoord19.x - 0.087) * (1.0 - 0.0) / (0.91 - 0.087)));
				half4 lerpResult190 = lerp( DefenceColors302 , ThreatColors303 , smoothstepResult42);
				half3 temp_output_316_0 = (lerpResult190).rgb;
				half3 temp_output_486_0 = ( temp_output_316_0 * _ShadingMultiplier );
				float2 uv_TextureSample0 = IN.texcoord.xy * _TextureSample0_ST.xy + _TextureSample0_ST.zw;
				half4 tex2DNode511 = tex2D( _TextureSample0, uv_TextureSample0 );
				half temp_output_508_0 = max( tex2DNode511.r , tex2DNode511.g );
				half3 lerpResult459 = lerp( temp_output_316_0 , temp_output_486_0 , temp_output_508_0);
				half3 lerpResult506 = lerp( temp_output_316_0 , temp_output_486_0 , temp_output_508_0);
				half3 lerpResult354 = lerp( max( lerpResult459 , lerpResult506 ) , half3(0,0,0) , tex2DNode511.b);
				half3 Color100 = saturate( lerpResult354 );
				half4 appendResult531 = (half4(Color100 , tex2DNode511.a));
				
				half4 color = appendResult531;
				
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
1913;36;1920;1017;3144.479;-619.7999;1;True;False
Node;AmplifyShaderEditor.RangedFloatNode;577;-2764.479,1272.8;Inherit;False;Global;StabilityIntensity;StabilityIntensity;14;0;Create;True;0;0;0;False;0;False;0;1.2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;584;-2552.479,1333.8;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;301;-2650.727,1177.999;Inherit;False;Property;_YPanSpeed;Y Pan Speed;5;0;Create;True;0;0;0;False;0;False;0;0.1;0;3;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;579;-2337.479,1251.8;Inherit;False;3;0;FLOAT;1;False;1;FLOAT;2;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;537;-2348.095,1077.412;Inherit;False;Property;_XPanSpeed;X Pan Speed;6;0;Create;True;0;0;0;False;0;False;0;0.15;0;3;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;583;-2172.479,1166.8;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;50;-2017.933,1042.326;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0.5;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;19;-2503.906,835.3554;Inherit;True;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PannerNode;49;-1815.234,1016.825;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;528;-1854.144,695.8151;Inherit;False;Global;Stability;Stability;16;0;Create;True;0;0;0;False;0;False;0;0.5;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;106;-1870.24,342.7099;Inherit;False;Property;_SliderMin;Slider Min;0;0;Create;True;0;0;0;False;0;False;0.075;0.075;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;44;-1579.434,1029.026;Inherit;True;Simplex2D;False;False;2;0;FLOAT2;0,0;False;1;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;527;-1951.095,440.1252;Inherit;False;Property;_SliderMax;Slider Max;1;0;Create;True;0;0;0;False;0;False;0.925;0.9;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;578;-1584.479,884.7999;Inherit;False;3;0;FLOAT;0.01;False;1;FLOAT;0.03;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;108;-1452.914,501.3542;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;559;-1339.085,1208.978;Inherit;False;Property;_LineBlur;Line Blur;3;0;Create;True;0;0;0;False;0;False;0.01;0.01;0;0.1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;45;-1326.474,886.3186;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;291;-135.1198,12.37366;Inherit;False;Property;_ThreatColor;Threat Color;8;0;Create;True;0;0;0;False;0;False;0.7924528,0,0,1;0.482,0,0,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;570;-2225.912,828.2944;Inherit;False;Constant;_Float6;Float 6;15;0;Create;True;0;0;0;False;0;False;0.91;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;556;-1098.852,754.946;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;569;-2222.502,756.0936;Inherit;False;Constant;_Float4;Float 4;14;0;Create;True;0;0;0;False;0;False;0.087;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;575;-1009.903,1177.667;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;285;-144.9173,-838.5121;Inherit;False;Property;_DefenseColor;Defense Color;7;0;Create;True;0;0;0;False;0;False;0,0.3496235,0.702,1;0,0.2649998,0.53,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;303;342.9445,-99.05182;Inherit;False;ThreatColors;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;558;-991.7518,985.9458;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;568;-1942.187,807.4594;Inherit;True;5;0;FLOAT;0;False;1;FLOAT;0.1;False;2;FLOAT;0.89;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;574;-928.884,791.166;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;302;294.9445,-459.0518;Inherit;False;DefenceColors;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SmoothstepOpNode;42;-737.5247,820.3702;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;305;-893.6731,463.342;Inherit;False;302;DefenceColors;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;304;-887.6731,550.342;Inherit;False;303;ThreatColors;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;190;-425.5276,734.3361;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;1,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;511;-42.32718,419.8845;Inherit;True;Property;_TextureSample0;Texture Sample 0;13;0;Create;True;0;0;0;False;0;False;-1;752f79075bc6f604482715dae4a26663;3e31335d5c9761d4284ffb0abaebfc96;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ComponentMaskNode;316;-258.4873,717.9601;Inherit;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;485;-275.0305,846.2801;Inherit;False;Property;_ShadingMultiplier;Shading Multiplier;12;0;Create;True;0;0;0;False;0;False;0.2;1.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMaxOpNode;508;304.9182,923.0835;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;486;-40.03052,793.2801;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LerpOp;506;302.9949,761.7993;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LerpOp;459;304.3743,645.1786;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;1,1,1;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.Vector3Node;356;436.6736,441.2546;Inherit;False;Constant;_Vector0;Vector 0;15;0;Create;True;0;0;0;False;0;False;0,0,0;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleMaxOpNode;529;479.9188,739.2632;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LerpOp;354;628.7952,633.4672;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SaturateNode;195;802.592,666.6582;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;100;1016.072,669.6618;Inherit;False;Color;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.CommentaryNode;115;-2660.836,-1908.428;Inherit;False;754.5939;386.0146;Defense;1;71;;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;116;-2728.538,-1406.649;Inherit;False;872.2551;349.4607;Bar;1;60;;1,1,1,1;0;0
Node;AmplifyShaderEditor.GetLocalVarNode;530;1289.011,-559.4915;Inherit;False;100;Color;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.CommentaryNode;114;-2684.857,-1026.695;Inherit;False;718.6239;380.481;Threat;1;87;;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;352;-3258.282,37.28274;Inherit;False;907.8655;574.8445;Shadow Mask;2;343;351;;1,1,1,1;0;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;540;1234.582,-435.8628;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;397;616.7174,85.03387;Inherit;False;211;PackedMasks;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;297;-1062.717,-247.8733;Inherit;False;Property;_BarSmoothing;Bar Smoothing;4;0;Create;True;0;0;0;False;0;False;0;0.9;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;452;-3168.555,819.0541;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;539;1775.52,-408.2599;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;1;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SaturateNode;411;-999.1029,-402.3314;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;516;1150.836,-1118.622;Inherit;False;302;DefenceColors;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;41;-1649.744,762.2764;Inherit;False;Property;_NoiseAmplitude;Noise Amplitude;2;0;Create;True;0;0;0;False;0;False;0.025;0.05;0;0.1;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;53;-3909.661,-1493.105;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SmoothstepOpNode;294;-613.3519,-88.24643;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;427;1717.559,-29.05096;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;504;-3487.942,1238.195;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;330;-827.4614,-1786.274;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;531;1519.966,-547.3936;Inherit;False;FLOAT4;4;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SaturateNode;349;-688.3105,-1300.809;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;293;-606.7209,-333.4338;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0.5;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;359;896.5851,338.3596;Inherit;False;357;ShadowMaskDebug;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;295;-359.9323,112.9685;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;418;1306.559,97.94904;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.StepOpNode;279;-441.8491,-338.9123;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;283;116.7796,-468.7107;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;280;-150.3055,-351.7923;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;299;-1349.272,-1320.661;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;343;-2917.717,106.2827;Inherit;False;FLOAT3;1;0;FLOAT3;0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.SaturateNode;351;-2784.033,154.8387;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RadiansOpNode;443;-4621.366,1397.739;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;120;1675.613,-126.567;Inherit;False;FLOAT4;4;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;1;False;1;FLOAT4;0
Node;AmplifyShaderEditor.LerpOp;290;178.1355,-92.28038;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;432;1538.188,278.0194;Inherit;False;-1;;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LerpOp;422;1361.559,-222.051;Inherit;False;3;0;FLOAT3;0.01,0.01,0.01;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.StepOpNode;287;-390.3845,-50.8977;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FractNode;130;-3524.132,-1728.579;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;430;1950.559,24.94904;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;71;-2036.488,-1749.573;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;147;-1720.865,-1502.666;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;292;-133.8255,183.0058;Inherit;False;Constant;_Color5;Color 5;12;0;Create;True;0;0;0;False;0;False;0.404,0,0,1;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PowerNode;429;1830.559,118.949;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.DistanceOpNode;365;1334.002,314.6362;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;420;1433.559,90.94904;Inherit;False;FLOAT3;1;0;FLOAT3;0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.SaturateNode;421;1702.559,103.949;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;518;1156.836,-1031.622;Inherit;False;303;ThreatColors;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.ComponentMaskNode;131;1515.279,407.6309;Inherit;False;True;True;True;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.GetLocalVarNode;218;1082.665,-228.3562;Inherit;False;100;Color;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SmoothstepOpNode;416;1154.559,97.94904;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.BreakToComponentsNode;278;-848.3152,-401.2;Inherit;False;FLOAT3;1;0;FLOAT3;0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.SimpleAddOpNode;460;-3436.672,685.6075;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;124;-3333.857,-1819.783;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;426;1615.559,33.94904;Inherit;False;Property;_Shadow1;Shadow 1;10;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;126;880.2161,-224.1193;Inherit;False;125;DebugUV;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;210;-1739.619,-1111.704;Inherit;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SaturateNode;60;-2031.815,-1107.884;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FractNode;129;-3625.869,-1806.379;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;281;-1102.675,-124.7487;Inherit;False;-1;;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;419;1585.559,111.949;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;300;-1523.449,-1318.141;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;282;-234.9018,-170.3535;Inherit;False;FLOAT3;1;0;FLOAT3;0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.RegisterLocalVarNode;211;-1544.409,-1109.75;Inherit;False;PackedMasks;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LerpOp;517;1691.215,-901.238;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;1,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;288;-67.79517,-91.70642;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;489;2095.384,359.9557;Inherit;False;FLOAT2;1;0;FLOAT2;0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.ComponentMaskNode;413;907.5056,84.72626;Inherit;False;True;True;True;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SaturateNode;433;1815.904,290.742;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;213;-1218.922,-428.4159;Inherit;False;211;PackedMasks;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;410;803.9075,249.0453;Inherit;False;Property;_ShadowMask;Shadow Mask;9;0;Create;True;0;0;0;False;0;False;0;-1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;272;-1229.205,125.5453;Inherit;False;FLOAT3;1;0;FLOAT3;0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.RegisterLocalVarNode;466;-3821.251,870.1798;Inherit;False;Test;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;357;-2287.706,385.1952;Inherit;False;ShadowMaskDebug;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;544;1475.312,-406.4484;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;-0.9;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;306;-1089.911,-1007.1;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;87;-2118.641,-687.6981;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;248;-1503.636,77.68979;Inherit;False;-1;;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;520;1047.868,-898.4193;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;286;-158.9173,-624.5121;Inherit;False;Constant;_Color3;Color 3;12;0;Create;True;0;0;0;False;0;False;0,0.2221255,0.446,1;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;412;795.5056,173.7263;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;101;-1176.386,-1319.769;Inherit;False;Mask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;431;1773.559,52.94904;Inherit;False;Property;_Shadow2;Shadow 2;11;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;424;2146.938,116.5695;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;1;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;125;-3005.599,-1805.42;Inherit;False;DebugUV;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;3;1850.92,-541.4042;Half;False;True;-1;2;ASEMaterialInspector;0;6;UIAnimationsBaked;5056123faa0c79b47ab6ad7e8bf059a4;True;Default;0;0;Default;2;False;True;2;5;False;-1;10;False;-1;0;1;False;-1;0;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;-1;False;True;True;True;True;True;0;True;-9;False;False;False;False;False;False;False;True;True;0;True;-5;255;True;-8;255;True;-7;0;True;-4;0;True;-6;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;False;True;2;False;-1;True;0;True;-11;False;True;5;Queue=Transparent=Queue=0;IgnoreProjector=True;RenderType=Transparent=RenderType;PreviewType=Plane;CanUseSpriteAtlas=True;False;False;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;0;;0;0;Standard;0;0;1;True;False;;False;0
WireConnection;584;0;577;0
WireConnection;579;2;584;0
WireConnection;583;0;301;0
WireConnection;583;1;579;0
WireConnection;50;0;537;0
WireConnection;50;1;583;0
WireConnection;49;0;19;0
WireConnection;49;2;50;0
WireConnection;44;0;49;0
WireConnection;578;2;584;0
WireConnection;108;0;106;0
WireConnection;108;1;527;0
WireConnection;108;2;528;0
WireConnection;45;0;578;0
WireConnection;45;1;44;0
WireConnection;556;0;108;0
WireConnection;556;1;45;0
WireConnection;575;0;559;0
WireConnection;303;0;291;0
WireConnection;558;0;556;0
WireConnection;558;1;559;0
WireConnection;568;0;19;1
WireConnection;568;1;569;0
WireConnection;568;2;570;0
WireConnection;574;0;556;0
WireConnection;574;1;575;0
WireConnection;302;0;285;0
WireConnection;42;0;568;0
WireConnection;42;1;574;0
WireConnection;42;2;558;0
WireConnection;190;0;305;0
WireConnection;190;1;304;0
WireConnection;190;2;42;0
WireConnection;316;0;190;0
WireConnection;508;0;511;1
WireConnection;508;1;511;2
WireConnection;486;0;316;0
WireConnection;486;1;485;0
WireConnection;506;0;316;0
WireConnection;506;1;486;0
WireConnection;506;2;508;0
WireConnection;459;0;316;0
WireConnection;459;1;486;0
WireConnection;459;2;508;0
WireConnection;529;0;459;0
WireConnection;529;1;506;0
WireConnection;354;0;529;0
WireConnection;354;1;356;0
WireConnection;354;2;511;3
WireConnection;195;0;354;0
WireConnection;100;0;195;0
WireConnection;539;0;544;0
WireConnection;411;0;213;0
WireConnection;294;0;278;2
WireConnection;294;1;297;0
WireConnection;427;0;430;0
WireConnection;531;0;530;0
WireConnection;531;3;511;4
WireConnection;293;0;278;0
WireConnection;293;1;297;0
WireConnection;418;0;416;0
WireConnection;279;0;293;0
WireConnection;283;1;286;0
WireConnection;283;2;280;0
WireConnection;280;0;279;0
WireConnection;280;1;282;0
WireConnection;299;0;300;0
WireConnection;351;0;343;2
WireConnection;120;0;422;0
WireConnection;290;1;292;0
WireConnection;290;2;288;0
WireConnection;422;1;218;0
WireConnection;422;2;427;0
WireConnection;287;0;294;0
WireConnection;430;0;429;0
WireConnection;430;1;431;0
WireConnection;429;0;421;0
WireConnection;429;1;426;0
WireConnection;420;0;418;0
WireConnection;421;0;419;0
WireConnection;416;0;413;0
WireConnection;416;2;410;0
WireConnection;278;0;411;0
WireConnection;124;0;129;0
WireConnection;124;1;130;0
WireConnection;419;0;420;0
WireConnection;419;1;420;1
WireConnection;419;2;420;2
WireConnection;282;0;281;0
WireConnection;211;0;210;0
WireConnection;517;0;516;0
WireConnection;517;1;518;0
WireConnection;288;0;287;0
WireConnection;288;1;282;2
WireConnection;413;0;397;0
WireConnection;433;0;432;0
WireConnection;272;0;248;0
WireConnection;544;0;540;1
WireConnection;101;0;299;0
WireConnection;125;0;124;0
WireConnection;3;0;531;0
ASEEND*/
//CHKSM=C0B14469FD39ED2A810498C609DD1D364BFA1B2B