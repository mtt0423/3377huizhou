Shader "Custom/InkWashShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _InkColor ("Ink Color", Color) = (0.1, 0.1, 0.3, 1)
        _WashColor ("Wash Color", Color) = (0.7, 0.7, 0.8, 1)
        _PaperTex ("Paper Texture", 2D) = "white" {}
        _OutlineColor ("Outline Color", Color) = (0, 0, 0, 1)
        _OutlineWidth ("Outline Width", Range(0, 0.1)) = 0.02
        _ColorSteps ("Color Steps", Range(1, 5)) = 3
        _InkThreshold ("Ink Threshold", Range(0, 1)) = 0.7
    }
    
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" }
        LOD 100
        
        // 第一道轮廓线 - 主轮廓
        Pass
        {
            Name "OUTLINE1"
            Cull Front
            ZWrite On
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            
            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };
            
            struct v2f
            {
                float4 pos : SV_POSITION;
            };
            
            float _OutlineWidth;
            float4 _OutlineColor;
            
            v2f vert (appdata v)
            {
                v2f o;
                float4 pos = float4(v.vertex.xyz + v.normal * _OutlineWidth, 1.0);
                o.pos = UnityObjectToClipPos(pos);
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                return _OutlineColor;
            }
            ENDCG
        }
        
        // 第二道轮廓线 - 细边（模拟毛笔飞白）
        Pass
        {
            Name "OUTLINE2"
            Cull Front
            ZWrite On
            Offset 1, 1
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            
            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };
            
            struct v2f
            {
                float4 pos : SV_POSITION;
            };
            
            float _OutlineWidth;
            float4 _OutlineColor;
            
            v2f vert (appdata v)
            {
                v2f o;
                float4 pos = float4(v.vertex.xyz + v.normal * (_OutlineWidth * 0.7), 1.0);
                o.pos = UnityObjectToClipPos(pos);
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                // 添加一些不规则性模拟毛笔痕迹
                float variation = sin(i.pos.x * 100 + i.pos.y * 150) * 0.1 + 0.9;
                return _OutlineColor * fixed4(1,1,1, variation);
            }
            ENDCG
        }
        
        // 主色Pass
        Pass
        {
            Name "MAIN"
            Cull Back
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            
            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };
            
            struct v2f
            {
                float2 uv : TEXCOORD0;
                float2 paperUV : TEXCOORD1;
                float3 worldNormal : TEXCOORD2;
                float3 worldPos : TEXCOORD3;
                float4 pos : SV_POSITION;
            };
            
            sampler2D _MainTex;
            sampler2D _PaperTex;
            float4 _MainTex_ST;
            float4 _PaperTex_ST;
            float4 _InkColor;
            float4 _WashColor;
            float _ColorSteps;
            float _InkThreshold;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.paperUV = TRANSFORM_TEX(v.uv, _PaperTex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }
            
            // 色彩量化函数
            float quantize(float value, float steps)
            {
                return floor(value * steps) / steps;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                // 基础纹理
                fixed4 texColor = tex2D(_MainTex, i.uv);
                
                // 光照计算
                float3 worldNormal = normalize(i.worldNormal);
                float3 worldLightDir = normalize(_WorldSpaceLightPos0.xyz);
                float NdotL = max(0, dot(worldNormal, worldLightDir));
                
                // 色彩量化 - 模拟水墨的有限色阶
                float steppedLight = quantize(NdotL, _ColorSteps);
                
                // 主色调选择
                float4 baseColor = lerp(_WashColor, _InkColor, steppedLight);
                
                // 叠加纸纹
                float4 paper = tex2D(_PaperTex, i.paperUV);
                baseColor.rgb *= paper.rgb * 1.2;
                
                // 边缘留白效果
                float edge = dot(worldNormal, float3(0, 1, 0));
                edge = abs(edge);
                if (edge > 0.7)
                {
                    baseColor.rgb = lerp(baseColor.rgb, _WashColor.rgb * 1.3, 0.3);
                }
                
                // 最终颜色混合
                fixed4 finalColor = baseColor;
                finalColor.a = texColor.a;
                
                return finalColor;
            }
            ENDCG
        }
    }
}