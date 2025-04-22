Shader "Hidden/CanvasMaskShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Offset ("Offset", Vector) = (0,0,0,0)
        _Scale ("Scale", Vector) = (1,1,0,0)
        _IsEllipse ("Is Ellipse", Float) = 0
        _Strength ("Strength", Range(0,1)) = 0.5
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "UnityCG.cginc"
            
            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };
            
            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };
            
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float2 _Offset;
            float2 _Scale;
            float _IsEllipse;
            float _Strength;
            
            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                float2 scaledUV = (i.uv - _Offset) / _Scale;
                scaledUV = scaledUV * 2.0 - 1.0;
                
                float mask;
                if (_IsEllipse > 0.5)
                {
                    mask = dot(scaledUV, scaledUV) <= 1.0 ? 0.0 : 1.0;
                }
                else
                {
                    mask = (abs(scaledUV.x) <= 1.0 && abs(scaledUV.y) <= 1.0) ? 0.0 : 1.0;
                }
                
                fixed4 col = tex2D(_MainTex, i.uv);
                col = lerp(col, float4(0.0, 0.0, 0.0, 1.0), mask * _Strength);
                return col;
            }
            ENDCG
        }
    }
}