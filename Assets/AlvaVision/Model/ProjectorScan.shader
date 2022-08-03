Shader "Projector/ray"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Cookie", 2D) = "white" {}
        _DirectVect("Directvect",Vector)= (1,1,1,1)
    }
    SubShader
    {
        Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" }
        Pass{
            ZWrite on
            ColorMask 0
            }
        Pass
        {
            ColorMask RGB
            ZWrite off Cull Off Lighting Off 
            
            Blend SrcAlpha OneMinusSrcAlpha    
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
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
                float4 texc:TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4x4 unity_Projector;
            fixed4 _Color;
            fixed4 _DirectVect;
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.texc = mul(unity_Projector, v.vertex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
              
                float4 c = tex2Dproj(_MainTex, i.texc) * _Color;
                float temp = saturate(dot(c, _DirectVect));
                int f = temp > 0 ? 1: 0;
                
                UNITY_APPLY_FOG(i.fogCoord, col);
                return c*f;
            }
            ENDCG
        }
    }
}