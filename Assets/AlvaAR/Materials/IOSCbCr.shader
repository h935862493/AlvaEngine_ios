Shader "Alva/IOSCbCr"
{
	Properties
	{
    	_textureY ("TextureY", 2D) = "white" {}
        _textureCbCr ("TextureCbCr", 2D) = "black" {}
	}
	SubShader
	{
		Cull Off
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
            ZWrite Off
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			//float4x4 _DisplayTransform;
			uniform float _ScaleX;
			uniform float _ScaleY;
			uniform float _OffsetX;
			uniform float _OffsetY;
			float4 _textureY_ST;
			uniform int  rotation;

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

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				
				float a = v.uv.x;
				switch (rotation)
					{
					case 0:
						v.uv.x = 1 - v.uv.y;
						v.uv.y = 1 - a;
						break;
					case 1:		
						v.uv.y = 1 - v.uv.y;
						break;
					case 2:
						break;
					case 3:
						break;
					}

				o.uv = TRANSFORM_TEX(v.uv, _textureY);

				UNITY_TRANSFER_FOG(o, o.vertex);
				return o;

			}
			
            // samplers
            sampler2D _textureY;
            sampler2D _textureCbCr;

			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
               // float2 texcoord = i.texcoord;

				i.uv.x = i.uv.x * _ScaleX + _OffsetX;
				i.uv.y = i.uv.y * _ScaleY + _OffsetY;

                float y = tex2D(_textureY, i.uv).r;
                float4 ycbcr = float4(y, tex2D(_textureCbCr, i.uv).rg, 1.0);

				const float4x4 ycbcrToRGBTransform = float4x4(
						float4(1.0, +0.0000, +1.4020, -0.7010),
						float4(1.0, -0.3441, -0.7141, +0.5291),
						float4(1.0, +1.7720, +0.0000, -0.8860),
						float4(0.0, +0.0000, +0.0000, +1.0000)
					);

                return mul(ycbcrToRGBTransform, ycbcr);
			}
			ENDCG
		}
	}
}
