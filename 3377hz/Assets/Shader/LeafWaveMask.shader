Shader "Unlit/LeafWaveMask"
{
    Properties
    {
        _MainTex ("Sprite", 2D) = "white" {}
        _MaskTex ("Speed Mask (Grayscale)", 2D) = "white" {}
        _WaveSpeed ("Wave Speed", Range(0,8)) = 3
        _WaveAmount ("Wave Amount", Range(0,0.3)) = 0.08
        _WaveFreq ("Wave Freq", Range(0,30)) = 15
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True"}
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };
            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex, _MaskTex;
            float _WaveSpeed, _WaveAmount, _WaveFreq;

            v2f vert (appdata v)
            {
                v2f o;
                float speedMask = tex2Dlod(_MaskTex, float4(v.uv,0,0)).r; // 读取灰度
                float wave = sin(v.uv.y * _WaveFreq + _Time.y * _WaveSpeed * speedMask)
                             * _WaveAmount * speedMask;
                v.vertex.x += wave * v.uv.y;   // 叶尖幅度更大
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return tex2D(_MainTex, i.uv);
            }
            ENDCG
        }
    }
}