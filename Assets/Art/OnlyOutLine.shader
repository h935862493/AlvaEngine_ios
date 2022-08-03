// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "CYF/Dissolve_old"
{
	Properties
	{
		_MaxZ("MaxZ", Float) = 0.5
		_Cutoff( "Mask Clip Value", Float ) = 0
		[KeywordEnum(x,y,z)] _AxisChoose("AxisChoose", Float) = 0
		_Nosie("Nosie", 2D) = "white" {}
		_MinZ("MinZ", Float) = -0.5
		_DissolveThreshold("DissolveThreshold", Range( 0 , 1)) = 0.5311602
		_WhiteNoise("WhiteNoise", 2D) = "white" {}
		_MeshTexture("MeshTexture", 2D) = "white" {}
		_WhiteNoiseEffect("WhiteNoiseEffect", Float) = 0.1
		_DissolveEdge("DissolveEdge", Float) = 0.1
		_Albedo("Albedo", Color) = (0,0,0,0)
		_LinearColor("LinearColor", 2D) = "white" {}
		_Threshold("Threshold", Range( -0.2 , 1.2)) = -0.2
		_Edge("Edge", Float) = 0.2
		_tiling("tiling", Vector) = (1,1,0,0)
		_Metallic("Metallic", Float) = 0
		_Smoothness("Smoothness", Float) = 0
		_NormalMap("NormalMap", 2D) = "bump" {}
		_Tiling(" Tiling", Vector) = (0,0,0,0)
		[Toggle(_USEDISSOLVEEFFECT_ON)] _UseDissolveEffect("UseDissolveEffect", Float) = 1
		_UseDissolve("UseDissolve", Float) = 1
		_EmissionStrenth("EmissionStrenth", Float) = 1
		_UseOpacity("UseOpacity", Float) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Custom"  "Queue" = "Transparent+0" "IsEmissive" = "true"  }
		Cull Back
		Blend SrcAlpha OneMinusSrcAlpha
		
		CGINCLUDE
		#include "UnityShaderVariables.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		#pragma shader_feature_local _USEDISSOLVEEFFECT_ON
		#pragma multi_compile _AXISCHOOSE_X _AXISCHOOSE_Y _AXISCHOOSE_Z
		struct Input
		{
			float2 uv_texcoord;
			float3 worldPos;
		};

		uniform sampler2D _NormalMap;
		uniform float4 _NormalMap_ST;
		uniform sampler2D _MeshTexture;
		uniform float2 _Tiling;
		uniform float4 _Albedo;
		uniform float _Threshold;
		uniform float _MaxZ;
		uniform float4x4 SelfMatrix;
		uniform float _MinZ;
		uniform float _Edge;
		uniform sampler2D _LinearColor;
		uniform sampler2D _WhiteNoise;
		uniform float2 _tiling;
		uniform float _WhiteNoiseEffect;
		uniform sampler2D _Nosie;
		uniform float _DissolveThreshold;
		uniform float _DissolveEdge;
		uniform float _EmissionStrenth;
		uniform float _Metallic;
		uniform float _Smoothness;
		uniform float _UseDissolve;
		uniform float _UseOpacity;
		uniform float _Cutoff = 0;


		float4x4 Inverse4x4(float4x4 input)
		{
			#define minor(a,b,c) determinant(float3x3(input.a, input.b, input.c))
			float4x4 cofactors = float4x4(
			minor( _22_23_24, _32_33_34, _42_43_44 ),
			-minor( _21_23_24, _31_33_34, _41_43_44 ),
			minor( _21_22_24, _31_32_34, _41_42_44 ),
			-minor( _21_22_23, _31_32_33, _41_42_43 ),
		
			-minor( _12_13_14, _32_33_34, _42_43_44 ),
			minor( _11_13_14, _31_33_34, _41_43_44 ),
			-minor( _11_12_14, _31_32_34, _41_42_44 ),
			minor( _11_12_13, _31_32_33, _41_42_43 ),
		
			minor( _12_13_14, _22_23_24, _42_43_44 ),
			-minor( _11_13_14, _21_23_24, _41_43_44 ),
			minor( _11_12_14, _21_22_24, _41_42_44 ),
			-minor( _11_12_13, _21_22_23, _41_42_43 ),
		
			-minor( _12_13_14, _22_23_24, _32_33_34 ),
			minor( _11_13_14, _21_23_24, _31_33_34 ),
			-minor( _11_12_14, _21_22_24, _31_32_34 ),
			minor( _11_12_13, _21_22_23, _31_32_33 ));
			#undef minor
			return transpose( cofactors ) / determinant( input );
		}


		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_NormalMap = i.uv_texcoord * _NormalMap_ST.xy + _NormalMap_ST.zw;
			o.Normal = tex2D( _NormalMap, uv_NormalMap ).rgb;
			float4 color21 = IsGammaSpace() ? float4(1,1,1,0) : float4(1,1,1,0);
			float4 color22 = IsGammaSpace() ? float4(0,0.9301295,1,0) : float4(0,0.8483561,1,0);
			float3 ase_worldPos = i.worldPos;
			float2 appendResult47 = (float2(ase_worldPos.x , ase_worldPos.z));
			float4 lerpResult20 = lerp( color21 , color22 , tex2D( _MeshTexture, ( appendResult47 * _Tiling ) ).g);
			float3 temp_cast_1 = (_MaxZ).xxx;
			float3 temp_output_23_0_g1 = temp_cast_1;
			float temp_output_42_0_g1 = (temp_output_23_0_g1).x;
			float3 ase_vertex3Pos = mul( unity_WorldToObject, float4( i.worldPos , 1 ) );
			float3 objToWorld5_g1 = mul( unity_ObjectToWorld, float4( ase_vertex3Pos, 1 ) ).xyz;
			float4x4 invertVal50_g1 = Inverse4x4( SelfMatrix );
			float3 break52_g1 = mul( float4( objToWorld5_g1 , 0.0 ), invertVal50_g1 ).xyz;
			float3 temp_cast_4 = (_MinZ).xxx;
			float3 temp_output_22_0_g1 = temp_cast_4;
			float temp_output_44_0_g1 = (temp_output_23_0_g1).y;
			float temp_output_46_0_g1 = (temp_output_23_0_g1).z;
			#if defined(_AXISCHOOSE_X)
				float staticSwitch25_g1 = ( abs( ( temp_output_42_0_g1 - break52_g1.x ) ) / ( temp_output_42_0_g1 - (temp_output_22_0_g1).x ) );
			#elif defined(_AXISCHOOSE_Y)
				float staticSwitch25_g1 = ( abs( ( temp_output_44_0_g1 - break52_g1.y ) ) / ( temp_output_44_0_g1 - (temp_output_22_0_g1).y ) );
			#elif defined(_AXISCHOOSE_Z)
				float staticSwitch25_g1 = ( abs( ( temp_output_46_0_g1 - break52_g1.z ) ) / ( temp_output_46_0_g1 - (temp_output_22_0_g1).z ) );
			#else
				float staticSwitch25_g1 = ( abs( ( temp_output_42_0_g1 - break52_g1.x ) ) / ( temp_output_42_0_g1 - (temp_output_22_0_g1).x ) );
			#endif
			float temp_output_28_0 = saturate( ( abs( ( _Threshold - staticSwitch25_g1 ) ) / _Edge ) );
			float4 lerpResult29 = lerp( lerpResult20 , _Albedo , temp_output_28_0);
			float4 MeshMaskColor72 = lerpResult29;
			float2 uv_TexCoord51 = i.uv_texcoord * _tiling;
			float temp_output_58_0 = ( ( saturate( tex2D( _WhiteNoise, uv_TexCoord51 ).r ) * _WhiteNoiseEffect ) + ( tex2D( _Nosie, uv_TexCoord51 ).r - _DissolveThreshold ) );
			float temp_output_61_0 = saturate( ( temp_output_58_0 / _DissolveEdge ) );
			float2 appendResult94 = (float2(temp_output_61_0 , temp_output_61_0));
			float4 BaseAlbedo77 = _Albedo;
			float4 lerpResult65 = lerp( tex2D( _LinearColor, appendResult94 ) , BaseAlbedo77 , temp_output_61_0);
			#ifdef _USEDISSOLVEEFFECT_ON
				float4 staticSwitch76 = lerpResult65;
			#else
				float4 staticSwitch76 = MeshMaskColor72;
			#endif
			float4 FinalColor79 = staticSwitch76;
			o.Albedo = FinalColor79.rgb;
			float4 EmmissionColor95 = lerpResult65;
			o.Emission = ( EmmissionColor95 * _EmissionStrenth ).rgb;
			o.Metallic = _Metallic;
			o.Smoothness = _Smoothness;
			float lerpResult103 = lerp( 1.0 , 0.0 , temp_output_28_0);
			float UseDissolve115 = _UseDissolve;
			float lerpResult113 = lerp( lerpResult103 , 1.0 , UseDissolve115);
			float Opacity108 = lerpResult113;
			float lerpResult123 = lerp( 1.0 , Opacity108 , _UseOpacity);
			o.Alpha = lerpResult123;
			float lerpResult83 = lerp( 1.0 , temp_output_58_0 , _UseDissolve);
			float AlphaMask80 = lerpResult83;
			clip( AlphaMask80 - _Cutoff );
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard keepalpha fullforwardshadows exclude_path:deferred 

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
			sampler3D _DitherMaskLOD;
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float2 customPack1 : TEXCOORD1;
				float3 worldPos : TEXCOORD2;
				float4 tSpace0 : TEXCOORD3;
				float4 tSpace1 : TEXCOORD4;
				float4 tSpace2 : TEXCOORD5;
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
				o.worldPos = worldPos;
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
				float3 worldPos = IN.worldPos;
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				SurfaceOutputStandard o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputStandard, o )
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
Version=18800
0;142;1920;1080;5032.379;1048.894;2.917271;True;False
Node;AmplifyShaderEditor.Vector2Node;50;-3270.631,1761.283;Inherit;False;Property;_tiling;tiling;16;0;Create;True;0;0;0;False;0;False;1,1;8,8;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.TextureCoordinatesNode;51;-3003.824,1742.883;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;66;-2750.882,1410.981;Inherit;True;Property;_WhiteNoise;WhiteNoise;8;0;Create;True;0;0;0;False;0;False;-1;dcc71921b79e05547894f875b8b0beea;dcc71921b79e05547894f875b8b0beea;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Matrix4X4Node;101;-2794.022,334.7117;Inherit;False;Global;SelfMatrix;SelfMatrix;23;0;Create;True;0;0;0;False;0;False;1,0,0,0,0,1,0,0,0,0,1,0,0,0,0,1;0;1;FLOAT4x4;0
Node;AmplifyShaderEditor.RangedFloatNode;17;-2909.813,756.3411;Inherit;False;Property;_MinZ;MinZ;5;0;Create;True;0;0;0;False;0;False;-0.5;-2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;13;-2990.813,397.7414;Inherit;False;Property;_MaxZ;MaxZ;0;0;Create;True;0;0;0;False;0;False;0.5;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.WorldPosInputsNode;46;-2966.146,-294.8546;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;52;-2415.029,1996.464;Inherit;False;Property;_DissolveThreshold;DissolveThreshold;6;0;Create;True;0;0;0;False;0;False;0.5311602;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;55;-2411.083,1364.981;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;24;-2428.913,392.664;Inherit;False;Property;_Threshold;Threshold;14;0;Create;True;0;0;0;False;0;False;-0.2;1.2;-0.2;1.2;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;54;-2598.696,1744.064;Inherit;True;Property;_Nosie;Nosie;4;0;Create;True;0;0;0;False;0;False;-1;a251f944d4b59b6429c71d1eb4d50dde;a251f944d4b59b6429c71d1eb4d50dde;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;53;-2542.882,1606.981;Inherit;False;Property;_WhiteNoiseEffect;WhiteNoiseEffect;10;0;Create;True;0;0;0;False;0;False;0.1;0.1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;102;-2398.079,526.0095;Inherit;False;NormalizeVertexLenth;2;;1;bf53d4c9039ba4d25a8113bb35bdaa4a;0;3;49;FLOAT4x4;0,0,0,0,0,1,0,0,0,0,1,0,0,0,0,1;False;23;FLOAT3;0,0,0;False;22;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;25;-2088.914,503.6638;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;56;-2071.523,1842.552;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;45;-2781.026,-463.9377;Inherit;False;Property;_Tiling; Tiling;20;0;Create;True;0;0;0;False;0;False;0,0;1,1;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.DynamicAppendNode;47;-2712.146,-295.8546;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;57;-2172.114,1424.814;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;27;-1872.115,672.9355;Inherit;False;Property;_Edge;Edge;15;0;Create;True;0;0;0;False;0;False;0.2;0.1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;48;-2508.146,-279.8546;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;59;-1912.468,1393.387;Inherit;False;Property;_DissolveEdge;DissolveEdge;11;0;Create;True;0;0;0;False;0;False;0.1;0.2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;58;-1833.892,1613.571;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.AbsOpNode;31;-1903.836,509.0917;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;22;-2111.813,-499.5506;Inherit;False;Constant;_Color1;Color 1;5;0;Create;True;0;0;0;False;0;False;0,0.9301295,1,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleDivideOpNode;26;-1726.913,510.6639;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;19;-2190.165,-340.4846;Inherit;True;Property;_MeshTexture;MeshTexture;9;0;Create;True;0;0;0;False;0;False;-1;None;885e9893a50f456448746c2676b54157;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;21;-2101.814,-656.5505;Inherit;False;Constant;_Color0;Color 0;5;0;Create;True;0;0;0;False;0;False;1,1,1,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleDivideOpNode;60;-1703.471,1381.572;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;84;-1735.596,1965.708;Inherit;False;Property;_UseDissolve;UseDissolve;22;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;20;-1741.848,-308.2916;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;37;-1822.854,92.49645;Inherit;False;Property;_Albedo;Albedo;12;0;Create;True;0;0;0;False;0;False;0,0,0,0;1,1,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SaturateNode;61;-1542.623,1382.674;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;28;-1549.536,507.4607;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;115;-1487.462,1952.089;Inherit;False;UseDissolve;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;104;-1256.485,303.7954;Inherit;False;Constant;_Float1;Float 1;23;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;77;-1295.144,90.82806;Inherit;False;BaseAlbedo;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;29;-1304.299,-103.4406;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.DynamicAppendNode;94;-1304.31,1296.999;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;105;-1251.825,390.4892;Inherit;False;Constant;_Float2;Float 2;23;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;103;-1074.165,414.0823;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;114;-1048.924,601.3501;Inherit;False;Constant;_Float4;Float 4;25;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;116;-1066.106,743.8252;Inherit;False;115;UseDissolve;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;63;-1078.973,1246.167;Inherit;True;Property;_LinearColor;LinearColor;13;0;Create;True;0;0;0;False;0;False;-1;902aae3ba10e5894c81f881e62e88390;902aae3ba10e5894c81f881e62e88390;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;78;-958.1403,1459.855;Inherit;False;77;BaseAlbedo;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;72;-1028.485,-144.0472;Inherit;False;MeshMaskColor;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;73;-505.05,1132.899;Inherit;False;72;MeshMaskColor;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;81;-1765.032,1856.818;Inherit;False;Constant;_Float0;Float 0;22;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;65;-567.7252,1298.83;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;113;-826.2238,557.1501;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;83;-1500.041,1660.643;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;108;-602.9945,539.3458;Inherit;False;Opacity;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;76;-165.7649,1157.083;Inherit;True;Property;_UseDissolveEffect;UseDissolveEffect;21;0;Create;True;0;0;0;False;0;False;0;1;1;True;;Toggle;2;Key0;Key1;Create;True;True;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;95;-165.0675,1430.894;Inherit;False;EmmissionColor;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;98;-300.1602,479.5659;Inherit;False;Property;_EmissionStrenth;EmissionStrenth;23;0;Create;True;0;0;0;False;0;False;1;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;80;-1311.121,1658.357;Inherit;False;AlphaMask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;96;-294.0774,382.8426;Inherit;False;95;EmmissionColor;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;124;-295.5513,566.2893;Inherit;False;Constant;_Float5;Float 5;25;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;79;165.564,1159.083;Inherit;False;FinalColor;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;110;-295.2898,642.9232;Inherit;False;108;Opacity;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;121;-294.479,716.4349;Inherit;False;Property;_UseOpacity;UseOpacity;25;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector4Node;14;-3064.912,519.7415;Inherit;False;Property;_CaptureLocation;CaptureLocation;7;0;Create;True;0;0;0;False;0;False;0,0,0,0;0,0,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;123;-58.5352,608.1155;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;111;-71.78025,841.5775;Inherit;False;Property;_Float3;Float 3;24;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;39;-274.8374,231.8637;Inherit;False;Property;_Smoothness;Smoothness;18;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;86;-178.6789,57.12002;Inherit;False;79;FinalColor;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;85;-78.28802,261.2024;Inherit;False;80;AlphaMask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;43;-257.8374,-189.1363;Inherit;True;Property;_NormalMap;NormalMap;19;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;bump;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;38;-270.8374,152.8637;Inherit;False;Property;_Metallic;Metallic;17;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;33;-2772.635,490.7938;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;97;-69.14585,407.3741;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;34;-2753.635,617.2941;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;49;304.727,22.22238;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;CYF/Dissolve_old;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0;True;True;0;True;Custom;;Transparent;ForwardOnly;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0.24;1,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;51;0;50;0
WireConnection;66;1;51;0
WireConnection;55;0;66;1
WireConnection;54;1;51;0
WireConnection;102;49;101;0
WireConnection;102;23;13;0
WireConnection;102;22;17;0
WireConnection;25;0;24;0
WireConnection;25;1;102;0
WireConnection;56;0;54;1
WireConnection;56;1;52;0
WireConnection;47;0;46;1
WireConnection;47;1;46;3
WireConnection;57;0;55;0
WireConnection;57;1;53;0
WireConnection;48;0;47;0
WireConnection;48;1;45;0
WireConnection;58;0;57;0
WireConnection;58;1;56;0
WireConnection;31;0;25;0
WireConnection;26;0;31;0
WireConnection;26;1;27;0
WireConnection;19;1;48;0
WireConnection;60;0;58;0
WireConnection;60;1;59;0
WireConnection;20;0;21;0
WireConnection;20;1;22;0
WireConnection;20;2;19;2
WireConnection;61;0;60;0
WireConnection;28;0;26;0
WireConnection;115;0;84;0
WireConnection;77;0;37;0
WireConnection;29;0;20;0
WireConnection;29;1;37;0
WireConnection;29;2;28;0
WireConnection;94;0;61;0
WireConnection;94;1;61;0
WireConnection;103;0;105;0
WireConnection;103;1;104;0
WireConnection;103;2;28;0
WireConnection;63;1;94;0
WireConnection;72;0;29;0
WireConnection;65;0;63;0
WireConnection;65;1;78;0
WireConnection;65;2;61;0
WireConnection;113;0;103;0
WireConnection;113;1;114;0
WireConnection;113;2;116;0
WireConnection;83;0;81;0
WireConnection;83;1;58;0
WireConnection;83;2;84;0
WireConnection;108;0;113;0
WireConnection;76;1;73;0
WireConnection;76;0;65;0
WireConnection;95;0;65;0
WireConnection;80;0;83;0
WireConnection;79;0;76;0
WireConnection;123;0;124;0
WireConnection;123;1;110;0
WireConnection;123;2;121;0
WireConnection;33;0;13;0
WireConnection;33;1;14;1
WireConnection;97;0;96;0
WireConnection;97;1;98;0
WireConnection;34;0;14;1
WireConnection;34;1;17;0
WireConnection;49;0;86;0
WireConnection;49;1;43;0
WireConnection;49;2;97;0
WireConnection;49;3;38;0
WireConnection;49;4;39;0
WireConnection;49;9;123;0
WireConnection;49;10;85;0
ASEEND*/
//CHKSM=E2CAEB313206D5BF8FC526F0BF7AD4CA52FCB44C