// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "ASETut/Dissolve"
{
	Properties
	{
		_Cutoff( "Mask Clip Value", Float ) = 0.5
		_Albedo("Albedo", 2D) = "white" {}
		_Normal("Normal", 2D) = "bump" {}
		_Metallic("Metallic", Range( 0 , 1)) = 0
		_Smoothness("Smoothness", Range( 0 , 1)) = 0
		_NormalScale("Normal Scale", Range( 0 , 5)) = 1
		_Ramp("Ramp", 2D) = "white" {}
		_EmissionIntensity("Emission Intensity", Range( 0 , 10)) = 0
		_OpacityMask("Opacity Mask", Range( -0.6 , 0.5)) = 0
		_RampUVScale("Ramp UV Scale", Float) = 0
		_Mask("Mask", 2D) = "white" {}
		_UVLerp("UV Lerp", Range( 0 , 1)) = 0
		_PannerSpeed("Panner Speed", Vector) = (0,0,0,0)
		[Toggle]_Animate("Animate", Float) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "AlphaTest+0" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#include "UnityStandardUtils.cginc"
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform float _NormalScale;
		uniform sampler2D _Normal;
		uniform float4 _Normal_ST;
		uniform sampler2D _Albedo;
		uniform float4 _Albedo_ST;
		uniform sampler2D _Ramp;
		uniform sampler2D _Mask;
		uniform float2 _PannerSpeed;
		uniform float _UVLerp;
		uniform float _Animate;
		uniform float _OpacityMask;
		uniform float _RampUVScale;
		uniform float _EmissionIntensity;
		uniform float _Metallic;
		uniform float _Smoothness;
		uniform float _Cutoff = 0.5;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_Normal = i.uv_texcoord * _Normal_ST.xy + _Normal_ST.zw;
			o.Normal = UnpackScaleNormal( tex2D( _Normal, uv_Normal ), _NormalScale );
			float2 uv_Albedo = i.uv_texcoord * _Albedo_ST.xy + _Albedo_ST.zw;
			o.Albedo = tex2D( _Albedo, uv_Albedo ).rgb;
			float2 panner25 = ( _Time.y * _PannerSpeed + i.uv_texcoord);
			float2 temp_cast_1 = (tex2D( _Mask, panner25 ).r).xx;
			float2 lerpResult23 = lerp( i.uv_texcoord , temp_cast_1 , _UVLerp);
			float temp_output_8_0 = ( tex2D( _Mask, lerpResult23 ).r - (( _Animate )?( (-0.6 + (_SinTime.z - 0.0) * (0.5 - -0.6) / (1.0 - 0.0)) ):( _OpacityMask )) );
			float2 temp_cast_2 = (( 1.0 - saturate( (( _RampUVScale * -1.0 ) + (temp_output_8_0 - 0.0) * (_RampUVScale - ( _RampUVScale * -1.0 )) / (1.0 - 0.0)) ) )).xx;
			o.Emission = ( tex2D( _Ramp, temp_cast_2 ) * _EmissionIntensity ).rgb;
			o.Metallic = _Metallic;
			o.Smoothness = _Smoothness;
			o.Alpha = 1;
			clip( temp_output_8_0 - _Cutoff );
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18000
435;73;1072;655;3643.068;424.6617;4.044302;True;False
Node;AmplifyShaderEditor.SimpleTimeNode;26;-2863,1255.062;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;22;-3072.314,853.9816;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;28;-2897.862,1100.052;Inherit;False;Property;_PannerSpeed;Panner Speed;12;0;Create;True;0;0;False;0;0,0;0.05,-0.05;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.TexturePropertyNode;20;-2499.303,500.484;Inherit;True;Property;_Mask;Mask;10;0;Create;True;0;0;False;0;None;e28dc97a9541e3642a48c0e3886688c5;False;white;Auto;Texture2D;-1;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.PannerNode;25;-2652,1114.062;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;21;-2363.599,1089.394;Inherit;True;Property;_TextureSample0;Texture Sample 0;11;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;24;-2365.884,1318.477;Inherit;False;Property;_UVLerp;UV Lerp;11;0;Create;True;0;0;False;0;0;0.1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SinTimeNode;29;-1788.614,880.5681;Inherit;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;23;-2046.13,1084.095;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;6;-1751.748,719.4156;Inherit;False;Property;_OpacityMask;Opacity Mask;8;0;Create;True;0;0;False;0;0;-0.6;-0.6;0.5;0;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;30;-1582.295,880.1373;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;-0.6;False;4;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;15;-1421.002,809.022;Inherit;False;Property;_RampUVScale;Ramp UV Scale;9;0;Create;True;0;0;False;0;0;3;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;7;-1806.435,508.653;Inherit;True;Property;_TextureMask;Texture Mask;6;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ToggleSwitchNode;31;-1318.111,1066.593;Inherit;False;Property;_Animate;Animate;13;0;Create;True;0;0;False;0;0;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;8;-1451.513,588.2398;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;16;-1180.107,622.4204;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;-1;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;14;-1060.83,742.5794;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;18;-859.4575,807.1891;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;19;-682.8741,812.8977;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;9;-409.5695,791.4617;Inherit;True;Property;_Ramp;Ramp;6;0;Create;True;0;0;False;0;-1;None;64e7766099ad46747a07014e44d0aea1;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;5;-1392.836,146.3434;Inherit;False;Property;_NormalScale;Normal Scale;5;0;Create;True;0;0;False;0;1;2.45;0;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;12;-389.5702,1056.277;Inherit;False;Property;_EmissionIntensity;Emission Intensity;7;0;Create;True;0;0;False;0;0;6.563017;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;2;-1023.144,167.6736;Inherit;True;Property;_Normal;Normal;2;0;Create;True;0;0;False;0;-1;None;77fdad851e93f394c9f8a1b1a63b56f3;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;3.35;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;4;-1016.011,463.2134;Inherit;False;Property;_Smoothness;Smoothness;4;0;Create;True;0;0;False;0;0;0.487;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;13;-19.83896,756.6808;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;1;-1036.912,-64.78225;Inherit;True;Property;_Albedo;Albedo;1;0;Create;True;0;0;False;0;-1;138df4511c079324cabae1f7f865c1c1;138df4511c079324cabae1f7f865c1c1;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;3;-1020.632,381.2431;Inherit;False;Property;_Metallic;Metallic;3;0;Create;True;0;0;False;0;0;0.457;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;0,0;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;ASETut/Dissolve;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Masked;0.5;True;True;0;False;TransparentCutout;;AlphaTest;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;0;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;25;0;22;0
WireConnection;25;2;28;0
WireConnection;25;1;26;0
WireConnection;21;0;20;0
WireConnection;21;1;25;0
WireConnection;23;0;22;0
WireConnection;23;1;21;1
WireConnection;23;2;24;0
WireConnection;30;0;29;3
WireConnection;7;0;20;0
WireConnection;7;1;23;0
WireConnection;31;0;6;0
WireConnection;31;1;30;0
WireConnection;8;0;7;1
WireConnection;8;1;31;0
WireConnection;16;0;15;0
WireConnection;14;0;8;0
WireConnection;14;3;16;0
WireConnection;14;4;15;0
WireConnection;18;0;14;0
WireConnection;19;0;18;0
WireConnection;9;1;19;0
WireConnection;2;5;5;0
WireConnection;13;0;9;0
WireConnection;13;1;12;0
WireConnection;0;0;1;0
WireConnection;0;1;2;0
WireConnection;0;2;13;0
WireConnection;0;3;3;0
WireConnection;0;4;4;0
WireConnection;0;10;8;0
ASEEND*/
//CHKSM=3A26F89142A2D06DA0F8D84DF14196AEA181D2A4