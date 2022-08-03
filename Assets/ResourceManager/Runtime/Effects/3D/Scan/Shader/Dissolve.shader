// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "CYF/Dissolve"
{
	Properties
	{
		_MinZ("MinZ", Float) = -0.5
		_MaxZ("MaxZ", Float) = 0.5
		[Toggle(_USEDISSOLVEEFFECT_ON)] _UseDissolveEffect("UseDissolveEffect", Float) = 1
		_UseDissolve("UseDissolve", Float) = 1
		_Cutoff( "Mask Clip Value", Float ) = 0
		[KeywordEnum(x,y,z)] _AxisChoose("AxisChoose", Float) = 0
		_Nosie("Nosie", 2D) = "white" {}
		_DissolveThreshold("DissolveThreshold", Range( 0 , 1)) = 0.5311602
		_DissolveNoiseTiling("DissolveNoiseTiling", Vector) = (1,1,0,0)
		_CaptureLocation("CaptureLocation", Vector) = (0,0,0,0)
		_WhiteNoiseEffect("WhiteNoiseEffect", Float) = 0.1
		_WhiteNoise("WhiteNoise", 2D) = "white" {}
		_MeshTexture("MeshTexture", 2D) = "white" {}
		_DisslveEdge("DisslveEdge", Float) = 0.1
		_LinearColor("LinearColor", 2D) = "white" {}
		_Threshold("Threshold", Range( -0.2 , 1.2)) = -0.2
		_Edge("Edge", Float) = 0.2
		_NormalMap("NormalMap", 2D) = "bump" {}
		_MeshTextureTiling("MeshTexture Tiling", Vector) = (0,0,0,0)
		_EmissionStrenth("EmissionStrenth", Float) = 0
		_maintex("maintex", 2D) = "white" {}
		_usetexture("use texture", Float) = 0
		_Albedo("Albedo", Color) = (0,0,0,0)
		_Smooth("Smooth", Float) = 0
		_FlakesBumpMapScale("_FlakesBumpMapScale", Float) = 2441.5
		_FlakesBumpStrength("FlakesBumpStrength", Range( 0.001 , 8)) = 0.24
		_FresnelColor("FresnelColor", Color) = (0,0.8313726,0.5490196,0)
		_FresnelPower("FresnelPower", Range( 0 , 10)) = 1
		_FlakesBump("FlakesBump", 2D) = "bump" {}
		_FresnelScale("FresnelScale", Range( 0 , 1)) = 0
		_FresnelBias("FresnelBias", Range( 0 , 2)) = 0
		_occlusionMask("occlusionMask", 2D) = "white" {}
		_MatallicMask("MatallicMask", 2D) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Custom"  "Queue" = "Transparent+0" }
		Cull Off
		Blend SrcAlpha OneMinusSrcAlpha
		
		CGINCLUDE
		#include "UnityPBSLighting.cginc"
		#include "UnityShaderVariables.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		#pragma multi_compile _AXISCHOOSE_X _AXISCHOOSE_Y _AXISCHOOSE_Z
		#pragma shader_feature_local _USEDISSOLVEEFFECT_ON
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
			float2 uv_texcoord;
			float3 worldPos;
			float3 worldNormal;
			INTERNAL_DATA
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

		uniform float4 _Albedo;
		uniform sampler2D _maintex;
		uniform float4 _maintex_ST;
		uniform float _usetexture;
		uniform float _Threshold;
		uniform float _MaxZ;
		uniform float4 _CaptureLocation;
		uniform float4x4 SelfMatrix;
		uniform float _MinZ;
		uniform float _Edge;
		uniform float _UseDissolve;
		uniform sampler2D _WhiteNoise;
		uniform float2 _DissolveNoiseTiling;
		uniform float _WhiteNoiseEffect;
		uniform sampler2D _Nosie;
		uniform float _DissolveThreshold;
		uniform sampler2D _MeshTexture;
		uniform float2 _MeshTextureTiling;
		uniform float4 _FresnelColor;
		uniform sampler2D _NormalMap;
		uniform float4 _NormalMap_ST;
		uniform float _FresnelBias;
		uniform float _FresnelScale;
		uniform float _FresnelPower;
		uniform sampler2D _LinearColor;
		uniform float _DisslveEdge;
		uniform sampler2D _FlakesBump;
		uniform float _FlakesBumpMapScale;
		uniform float _FlakesBumpStrength;
		uniform float _EmissionStrenth;
		uniform sampler2D _MatallicMask;
		uniform float4 _MatallicMask_ST;
		uniform float _Smooth;
		uniform sampler2D _occlusionMask;
		uniform float4 _occlusionMask_ST;
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


		inline half4 LightingStandardCustomLighting( inout SurfaceOutputCustomLightingCustom s, half3 viewDir, UnityGI gi )
		{
			UnityGIInput data = s.GIData;
			Input i = s.SurfInput;
			half4 c = 0;
			float2 uv_maintex = i.uv_texcoord * _maintex_ST.xy + _maintex_ST.zw;
			float4 lerpResult164 = lerp( _Albedo , tex2D( _maintex, uv_maintex ) , _usetexture);
			float4 BaseAlbedo77 = lerpResult164;
			float3 temp_cast_0 = (( _MaxZ + _CaptureLocation.x )).xxx;
			float3 temp_output_23_0_g1 = temp_cast_0;
			float temp_output_42_0_g1 = (temp_output_23_0_g1).x;
			float3 ase_vertex3Pos = mul( unity_WorldToObject, float4( i.worldPos , 1 ) );
			float3 objToWorld5_g1 = mul( unity_ObjectToWorld, float4( ase_vertex3Pos, 1 ) ).xyz;
			float4x4 invertVal50_g1 = Inverse4x4( SelfMatrix );
			float3 break52_g1 = mul( float4( objToWorld5_g1 , 0.0 ), invertVal50_g1 ).xyz;
			float3 temp_cast_3 = (( _CaptureLocation.x - _MinZ )).xxx;
			float3 temp_output_22_0_g1 = temp_cast_3;
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
			float lerpResult103 = lerp( 1.0 , 0.0 , temp_output_28_0);
			float UseDissolve115 = _UseDissolve;
			float lerpResult113 = lerp( lerpResult103 , 1.0 , UseDissolve115);
			float Opacity108 = min( BaseAlbedo77.a , lerpResult113 );
			float2 uv_TexCoord51 = i.uv_texcoord * _DissolveNoiseTiling;
			float temp_output_58_0 = ( ( saturate( tex2D( _WhiteNoise, uv_TexCoord51 ).r ) * _WhiteNoiseEffect ) + ( tex2D( _Nosie, uv_TexCoord51 ).r - _DissolveThreshold ) );
			float lerpResult83 = lerp( 1.0 , temp_output_58_0 , _UseDissolve);
			float AlphaMask80 = min( lerpResult83 , ( Opacity108 - 0.01 ) );
			SurfaceOutputStandard s160 = (SurfaceOutputStandard ) 0;
			float4 color21 = IsGammaSpace() ? float4(1,1,1,0) : float4(1,1,1,0);
			float4 color22 = IsGammaSpace() ? float4(0,0.9301295,1,0) : float4(0,0.8483561,1,0);
			float3 ase_worldPos = i.worldPos;
			float2 appendResult47 = (float2(ase_worldPos.x , ase_worldPos.z));
			float4 lerpResult20 = lerp( color21 , color22 , tex2D( _MeshTexture, ( appendResult47 * _MeshTextureTiling ) ).g);
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float2 uv_NormalMap = i.uv_texcoord * _NormalMap_ST.xy + _NormalMap_ST.zw;
			float3 tex2DNode43 = UnpackNormal( tex2D( _NormalMap, uv_NormalMap ) );
			float3 appendResult129 = (float3(tex2DNode43.r , tex2DNode43.g , tex2DNode43.b));
			float3 BumpNormal134 = appendResult129;
			float fresnelNdotV133 = dot( BumpNormal134, ase_worldViewDir );
			float fresnelNode133 = ( _FresnelBias + _FresnelScale * pow( 1.0 - fresnelNdotV133, _FresnelPower ) );
			float4 lerpResult140 = lerp( lerpResult164 , _FresnelColor , fresnelNode133);
			float4 lerpResult29 = lerp( lerpResult20 , lerpResult140 , temp_output_28_0);
			float4 MeshMaskColor72 = lerpResult29;
			float temp_output_61_0 = saturate( ( temp_output_58_0 / _DisslveEdge ) );
			float2 appendResult94 = (float2(temp_output_61_0 , temp_output_61_0));
			float4 lerpResult65 = lerp( tex2D( _LinearColor, appendResult94 ) , BaseAlbedo77 , temp_output_61_0);
			#ifdef _USEDISSOLVEEFFECT_ON
				float4 staticSwitch76 = lerpResult65;
			#else
				float4 staticSwitch76 = MeshMaskColor72;
			#endif
			float4 FinalColor79 = staticSwitch76;
			s160.Albedo = FinalColor79.rgb;
			float3 tex2DNode117 = UnpackNormal( tex2D( _FlakesBump, ( i.uv_texcoord * _FlakesBumpMapScale ) ) );
			float2 appendResult122 = (float2(tex2DNode117.r , tex2DNode117.g));
			float3 appendResult125 = (float3(( appendResult122 * _FlakesBumpStrength ) , 0.0));
			float3 lerpResult2_g2 = lerp( appendResult129 , float3(0,0,1) , 1.0);
			float3 OutPutNormal131 = saturate( ( appendResult125 + lerpResult2_g2 ) );
			s160.Normal = WorldNormalVector( i , OutPutNormal131 );
			float4 EmmissionColor95 = lerpResult65;
			s160.Emission = ( EmmissionColor95 * _EmissionStrenth ).rgb;
			float2 uv_MatallicMask = i.uv_texcoord * _MatallicMask_ST.xy + _MatallicMask_ST.zw;
			s160.Metallic = tex2D( _MatallicMask, uv_MatallicMask ).r;
			s160.Smoothness = _Smooth;
			float2 uv_occlusionMask = i.uv_texcoord * _occlusionMask_ST.xy + _occlusionMask_ST.zw;
			s160.Occlusion = tex2D( _occlusionMask, uv_occlusionMask ).r;

			data.light = gi.light;

			UnityGI gi160 = gi;
			#ifdef UNITY_PASS_FORWARDBASE
			Unity_GlossyEnvironmentData g160 = UnityGlossyEnvironmentSetup( s160.Smoothness, data.worldViewDir, s160.Normal, float3(0,0,0));
			gi160 = UnityGlobalIllumination( data, s160.Occlusion, s160.Normal, g160 );
			#endif

			float3 surfResult160 = LightingStandard ( s160, viewDir, gi160 ).rgb;
			surfResult160 += s160.Emission;

			#ifdef UNITY_PASS_FORWARDADD//160
			surfResult160 -= s160.Emission;
			#endif//160
			c.rgb = surfResult160;
			c.a = Opacity108;
			clip( AlphaMask80 - _Cutoff );
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
		#pragma surface surf StandardCustomLighting keepalpha fullforwardshadows exclude_path:deferred 

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
				UnityGI gi;
				UNITY_INITIALIZE_OUTPUT( UnityGI, gi );
				o.Alpha = LightingStandardCustomLighting( o, worldViewDir, gi ).a;
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
	//CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18800
49;205;1920;1079;879.6034;113.9262;1.073239;True;False
Node;AmplifyShaderEditor.Vector4Node;14;-3064.912,519.7415;Inherit;False;Property;_CaptureLocation;CaptureLocation;10;0;Create;True;0;0;0;False;0;False;0,0,0,0;0,0,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;17;-3015.813,750.3411;Inherit;False;Property;_MinZ;MinZ;0;0;Create;True;0;0;0;False;0;False;-0.5;-0.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;13;-3016.813,390.7414;Inherit;False;Property;_MaxZ;MaxZ;1;0;Create;True;0;0;0;False;0;False;0.5;0.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;33;-2772.635,488.7938;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;34;-2768.635,617.2941;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;50;-3270.631,1761.283;Inherit;False;Property;_DissolveNoiseTiling;DissolveNoiseTiling;9;0;Create;True;0;0;0;False;0;False;1,1;1,1;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.Matrix4X4Node;101;-2794.022,334.7117;Inherit;False;Global;SelfMatrix;SelfMatrix;23;0;Create;True;0;0;0;False;0;False;1,0,0,0,0,1,0,0,0,0,1,0,0,0,0,1;0;1;FLOAT4x4;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;51;-3003.824,1742.883;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;24;-2428.913,392.664;Inherit;False;Property;_Threshold;Threshold;16;0;Create;True;0;0;0;False;0;False;-0.2;-0.2;-0.2;1.2;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;151;-2424.65,511.7958;Inherit;False;NormalizeVertexLenth;5;;1;b4165f3df9fbc4588ba2fcb26b599a0b;0;3;49;FLOAT4x4;0,0,0,0,0,1,0,0,0,0,1,0,0,0,0,1;False;23;FLOAT3;0,0,0;False;22;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;25;-2088.914,503.6638;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;43;-517.0394,-492.1563;Inherit;True;Property;_NormalMap;NormalMap;18;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;66;-2750.882,1410.981;Inherit;True;Property;_WhiteNoise;WhiteNoise;12;0;Create;True;0;0;0;False;0;False;-1;None;dcc71921b79e05547894f875b8b0beea;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.AbsOpNode;31;-1903.836,509.0917;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;55;-2411.083,1364.981;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;53;-2542.882,1606.981;Inherit;False;Property;_WhiteNoiseEffect;WhiteNoiseEffect;11;0;Create;True;0;0;0;False;0;False;0.1;0.1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;52;-2415.029,1996.464;Inherit;False;Property;_DissolveThreshold;DissolveThreshold;8;0;Create;True;0;0;0;False;0;False;0.5311602;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;129;-205.8293,-490.9238;Inherit;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SamplerNode;54;-2598.696,1744.064;Inherit;True;Property;_Nosie;Nosie;7;0;Create;True;0;0;0;False;0;False;-1;None;a251f944d4b59b6429c71d1eb4d50dde;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WorldPosInputsNode;46;-2966.146,-294.8546;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;27;-1907.669,657.8518;Inherit;False;Property;_Edge;Edge;17;0;Create;True;0;0;0;False;0;False;0.2;0.2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;166;-1741.259,196.7952;Inherit;False;Property;_usetexture;use texture;22;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;57;-2172.114,1424.814;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;26;-1768.932,509.5865;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;37;-1925.719,-110.0685;Inherit;False;Property;_Albedo;Albedo;23;0;Create;True;0;0;0;False;0;False;0,0,0,0;0.8584906,0.8584906,0.8584906,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;84;-1700.596,1958.708;Inherit;False;Property;_UseDissolve;UseDissolve;3;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;118;-949.7045,-1148.226;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;47;-2712.146,-295.8546;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;120;-1002.512,-956.0048;Inherit;False;Property;_FlakesBumpMapScale;_FlakesBumpMapScale;25;0;Create;True;0;0;0;False;0;False;2441.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;45;-2781.026,-463.9377;Inherit;False;Property;_MeshTextureTiling;MeshTexture Tiling;19;0;Create;True;0;0;0;False;0;False;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleSubtractOpNode;56;-2071.523,1842.552;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;134;184.0523,-504.8884;Inherit;False;BumpNormal;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SamplerNode;167;-2114.896,84.49524;Inherit;True;Property;_maintex;maintex;21;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;161;-2648.389,-597.7078;Inherit;False;Property;_FresnelScale;FresnelScale;30;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;119;-673.5118,-1132.005;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;104;-1438.207,234.2081;Inherit;False;Constant;_Float1;Float 1;23;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;28;-1549.536,507.4607;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;115;-1487.462,1952.089;Inherit;False;UseDissolve;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;162;-2653.389,-665.7078;Inherit;False;Property;_FresnelBias;FresnelBias;31;0;Create;True;0;0;0;False;0;False;0;0;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;136;-2662.12,-809.3984;Inherit;False;World;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;137;-2568.12,-516.3983;Inherit;False;Property;_FresnelPower;FresnelPower;28;0;Create;True;0;0;0;False;0;False;1;1;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;135;-2666.12,-909.3984;Inherit;False;134;BumpNormal;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;48;-2508.146,-279.8546;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;58;-1833.892,1613.571;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;164;-1608.052,-101.6341;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;59;-1912.468,1393.387;Inherit;False;Property;_DisslveEdge;DisslveEdge;14;0;Create;True;0;0;0;False;0;False;0.1;0.1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;105;-1451.665,354.3496;Inherit;False;Constant;_Float2;Float 2;23;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;77;-1224.721,75.98275;Inherit;False;BaseAlbedo;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;21;-2101.814,-656.5505;Inherit;False;Constant;_Color0;Color 0;5;0;Create;True;0;0;0;False;0;False;1,1,1,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;103;-1261.461,364.0062;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;117;-529.4092,-1159.155;Inherit;True;Property;_FlakesBump;FlakesBump;29;0;Create;True;0;0;0;False;0;False;-1;1169ae3514ec78447b915ec445961b32;1169ae3514ec78447b915ec445961b32;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;19;-2190.165,-340.4846;Inherit;True;Property;_MeshTexture;MeshTexture;13;0;Create;True;0;0;0;False;0;False;-1;885e9893a50f456448746c2676b54157;885e9893a50f456448746c2676b54157;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleDivideOpNode;60;-1703.471,1381.572;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;22;-2111.813,-499.5506;Inherit;False;Constant;_Color1;Color 1;5;0;Create;True;0;0;0;False;0;False;0,0.9301295,1,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;114;-1308.736,541.2858;Inherit;False;Constant;_Float4;Float 4;25;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;116;-1295.257,667.0369;Inherit;False;115;UseDissolve;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.FresnelNode;133;-2284.305,-802.8925;Inherit;False;Standard;WorldNormal;ViewDir;False;False;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;139;-2270.12,-1021.398;Inherit;False;Property;_FresnelColor;FresnelColor;27;0;Create;True;0;0;0;False;0;False;0,0.8313726,0.5490196,0;0,0.8313726,0.5490196,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;20;-1741.848,-308.2916;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.DynamicAppendNode;122;-226.4007,-1137.382;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;124;-475.3116,-936.9073;Inherit;False;Property;_FlakesBumpStrength;FlakesBumpStrength;26;0;Create;True;0;0;0;False;0;False;0.24;0.001;0.001;8;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;61;-1542.623,1382.674;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;168;-943.8605,81.35203;Inherit;False;COLOR;1;0;COLOR;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.LerpOp;113;-1086.036,497.0858;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;140;-1484.043,-425.8836;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.DynamicAppendNode;94;-1326.41,1257.999;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;153;-147.3141,-649.9595;Inherit;False;Constant;_Float7;Float 7;29;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMinOpNode;171;-864.8556,467.101;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;29;-1189.427,-152.264;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;123;-79.73074,-1136.961;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;63;-1078.973,1246.167;Inherit;True;Property;_LinearColor;LinearColor;15;0;Create;True;0;0;0;False;0;False;-1;902aae3ba10e5894c81f881e62e88390;902aae3ba10e5894c81f881e62e88390;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FunctionNode;152;28.68591,-749.9595;Inherit;False;FlattenNormal;-1;;2;5af39371d59ab5348a593c80f7f6f2f3;0;2;4;FLOAT3;0,0,0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;78;-1073.825,1508.691;Inherit;False;77;BaseAlbedo;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.DynamicAppendNode;125;94.02362,-1132.622;Inherit;False;FLOAT3;4;0;FLOAT2;0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;108;-637.5984,469.5966;Inherit;False;Opacity;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;72;-868.3237,-165.6151;Inherit;False;MeshMaskColor;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;65;-567.7252,1298.83;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;183;-1268.094,1965.216;Inherit;False;Constant;_Float3;Float 3;34;0;Create;True;0;0;0;False;0;False;0.01;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;81;-1765.032,1856.818;Inherit;False;Constant;_Float0;Float 0;22;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;73;-505.05,1132.899;Inherit;False;72;MeshMaskColor;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;177;-1326.952,1825.776;Inherit;False;108;Opacity;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;126;209.4288,-1025.514;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.StaticSwitch;76;-165.7649,1157.083;Inherit;True;Property;_UseDissolveEffect;UseDissolveEffect;2;0;Create;True;0;0;0;False;0;False;0;1;1;True;;Toggle;2;Key0;Key1;Create;True;True;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;182;-1135.094,1876.216;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;95;-203.779,1474.241;Inherit;False;EmmissionColor;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;83;-1500.041,1660.643;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;130;422.9359,-1018.652;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;96;-249.7766,470.2537;Inherit;False;95;EmmissionColor;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;79;163.4319,1156.951;Inherit;False;FinalColor;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMinOpNode;180;-1002.565,1790.464;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;131;627.866,-1021.329;Inherit;False;OutPutNormal;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;98;-255.8593,566.9771;Inherit;False;Property;_EmissionStrenth;EmissionStrenth;20;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;39;-203.1114,367.4571;Inherit;False;Property;_Smooth;Smooth;24;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;97;-7.845016,484.7852;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;185;-271.7317,167.1616;Inherit;True;Property;_MatallicMask;MatallicMask;33;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;86;47.52361,-219.3244;Inherit;False;79;FinalColor;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;80;-820.5527,1651.264;Inherit;False;AlphaMask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;132;-5.360363,78.54766;Inherit;False;131;OutPutNormal;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SamplerNode;184;-204.9104,657.3064;Inherit;True;Property;_occlusionMask;occlusionMask;32;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;138;-2273.12,-1202.398;Inherit;False;Constant;_FinalColor;FinalColor;25;0;Create;True;0;0;0;False;0;False;0,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;110;638.1505,583.8779;Inherit;False;108;Opacity;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;85;492.7518,425.3169;Inherit;False;80;AlphaMask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;163;-10.56073,-77.65814;Inherit;False;72;MeshMaskColor;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.CustomStandardSurface;160;378.729,53.27963;Inherit;False;Metallic;Tangent;6;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,1;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;1;False;1;FLOAT3;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;49;795.1935,-50.75976;Float;False;True;-1;2;ASEMaterialInspector;0;0;CustomLighting;CYF/Dissolve;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Off;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0;True;True;0;True;Custom;;Transparent;ForwardOnly;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0.24;1,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;4;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;33;0;13;0
WireConnection;33;1;14;1
WireConnection;34;0;14;1
WireConnection;34;1;17;0
WireConnection;51;0;50;0
WireConnection;151;49;101;0
WireConnection;151;23;33;0
WireConnection;151;22;34;0
WireConnection;25;0;24;0
WireConnection;25;1;151;0
WireConnection;66;1;51;0
WireConnection;31;0;25;0
WireConnection;55;0;66;1
WireConnection;129;0;43;1
WireConnection;129;1;43;2
WireConnection;129;2;43;3
WireConnection;54;1;51;0
WireConnection;57;0;55;0
WireConnection;57;1;53;0
WireConnection;26;0;31;0
WireConnection;26;1;27;0
WireConnection;47;0;46;1
WireConnection;47;1;46;3
WireConnection;56;0;54;1
WireConnection;56;1;52;0
WireConnection;134;0;129;0
WireConnection;119;0;118;0
WireConnection;119;1;120;0
WireConnection;28;0;26;0
WireConnection;115;0;84;0
WireConnection;48;0;47;0
WireConnection;48;1;45;0
WireConnection;58;0;57;0
WireConnection;58;1;56;0
WireConnection;164;0;37;0
WireConnection;164;1;167;0
WireConnection;164;2;166;0
WireConnection;77;0;164;0
WireConnection;103;0;105;0
WireConnection;103;1;104;0
WireConnection;103;2;28;0
WireConnection;117;1;119;0
WireConnection;19;1;48;0
WireConnection;60;0;58;0
WireConnection;60;1;59;0
WireConnection;133;0;135;0
WireConnection;133;4;136;0
WireConnection;133;1;162;0
WireConnection;133;2;161;0
WireConnection;133;3;137;0
WireConnection;20;0;21;0
WireConnection;20;1;22;0
WireConnection;20;2;19;2
WireConnection;122;0;117;1
WireConnection;122;1;117;2
WireConnection;61;0;60;0
WireConnection;168;0;77;0
WireConnection;113;0;103;0
WireConnection;113;1;114;0
WireConnection;113;2;116;0
WireConnection;140;0;164;0
WireConnection;140;1;139;0
WireConnection;140;2;133;0
WireConnection;94;0;61;0
WireConnection;94;1;61;0
WireConnection;171;0;168;3
WireConnection;171;1;113;0
WireConnection;29;0;20;0
WireConnection;29;1;140;0
WireConnection;29;2;28;0
WireConnection;123;0;122;0
WireConnection;123;1;124;0
WireConnection;63;1;94;0
WireConnection;152;4;129;0
WireConnection;152;3;153;0
WireConnection;125;0;123;0
WireConnection;108;0;171;0
WireConnection;72;0;29;0
WireConnection;65;0;63;0
WireConnection;65;1;78;0
WireConnection;65;2;61;0
WireConnection;126;0;125;0
WireConnection;126;1;152;0
WireConnection;76;1;73;0
WireConnection;76;0;65;0
WireConnection;182;0;177;0
WireConnection;182;1;183;0
WireConnection;95;0;65;0
WireConnection;83;0;81;0
WireConnection;83;1;58;0
WireConnection;83;2;84;0
WireConnection;130;0;126;0
WireConnection;79;0;76;0
WireConnection;180;0;83;0
WireConnection;180;1;182;0
WireConnection;131;0;130;0
WireConnection;97;0;96;0
WireConnection;97;1;98;0
WireConnection;80;0;180;0
WireConnection;160;0;86;0
WireConnection;160;1;132;0
WireConnection;160;2;97;0
WireConnection;160;3;185;1
WireConnection;160;4;39;0
WireConnection;160;5;184;1
WireConnection;49;9;110;0
WireConnection;49;10;85;0
WireConnection;49;13;160;0
ASEEND*/
//CHKSM=AF4FA72FBA247C663E02CFE7C5FC9EAE15D0123D