Shader "Alva/AlvaBackground"
{
	Properties
	{
		 _Color("Color", Color) = (1,1,1,1)
		_MainTex("Texture", 2D) = "white" {}
		_UTex("U", 2D) = "white" {}
		_VTex("V", 2D) = "white" {}
	}
		SubShader
		{
			
			Pass
			{
				ZWrite Off
				Cull Off

				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				// make fog work
				#pragma multi_compile_fog

				#include "UnityCG.cginc"

				struct appdata
				{
					float4 vertex : POSITION;
					float2 uv : TEXCOORD0;
				};

				struct v2f
				{
					float2 uv : TEXCOORD0;
					UNITY_FOG_COORDS(1)
					float4 vertex : SV_POSITION;
				};

				sampler2D _MainTex;
				sampler2D _UTex;
				sampler2D _VTex;
				sampler2D _UVTex;
				float4 _MainTex_ST;
				uniform float _ScaleX;
				uniform float _ScaleY;
				uniform float _OffsetX;
				uniform float _OffsetY;
				v2f vert(appdata v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					//uv旋转
					float a = v.uv.x;
					v.uv.x = 1-v.uv.y;
					v.uv.y = 1-a;
					o.uv = TRANSFORM_TEX(v.uv, _MainTex);

					UNITY_TRANSFER_FOG(o, o.vertex);
					return o;
				}

				fixed4 frag(v2f i) : SV_Target
				{
					i.uv.x = i.uv.x * _ScaleX + _OffsetX;
					i.uv.y = i.uv.y * _ScaleY + _OffsetY;
					fixed4 ycol = tex2D(_MainTex, i.uv);
					fixed4 ucol = tex2D(_UTex, i.uv);
					fixed4 vcol = tex2D(_VTex, i.uv);
					//fixed4 uvcol = tex2D(_UVTex,uv);

					//如果是使用 Alpha8 的纹理格式写入各分量的值，各分量的值就可以直接取a通道的值
					float r = ycol.a + 1.4022 * vcol.a - 0.7011;
					float g = ycol.a - 0.3456 * ucol.a - 0.7145 * vcol.a + 0.53005;
					float b = ycol.a + 1.771 * ucol.a - 0.8855;

					return fixed4(r,g,b,1);
				}
				ENDCG
			}
		}

}