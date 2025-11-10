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
        Tags { 
            "Queue"="Transparent" 
            "RenderType"="Transparent" 
            "IgnoreProjector"="True"
        }
        
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

            sampler2D _MainTex;
            sampler2D _MaskTex;
            float4 _MaskTex_ST; // 添加纹理缩放偏移
            float _WaveSpeed, _WaveAmount, _WaveFreq;

            v2f vert (appdata v)
            {
                v2f o;
                
                // 修正：在顶点着色器中使用 tex2Dlod 需要特殊处理
                // 或者改用简单的 UV 动画测试
                
                // 方法1：使用简单的基于 UV Y 的遮罩（先测试基础功能）
                float speedMask = v.uv.y; // 临时：用 UV Y 代替遮罩纹理
                
                // 计算波浪
                float wave = sin(v.uv.y * _WaveFreq + _Time.y * _WaveSpeed) 
                           * _WaveAmount * speedMask;
                
                // 应用顶点偏移
                v.vertex.x += wave;
                
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                return col;
            }
            ENDCG
        }
    }
}