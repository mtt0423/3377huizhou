Shader "Custom/ToonShader" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _ShadowColor ("Shadow Color", Color) = (0.5,0.5,0.5,1)
        _Ramp ("Ramp Texture", 2D) = "white" {}
        _OutlineWidth ("Outline Width", Range(0,0.1)) = 0.02
        _OutlineColor ("Outline Color", Color) = (0,0,0,1)
    }
    SubShader {
        Tags { "RenderType"="Opaque" }
        LOD 200

        // ① 描边 Pass（背面膨胀）
        Pass {
            Name "OUTLINE"
            Cull Front
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            float _OutlineWidth;
            float4 _OutlineColor;
            struct appdata {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };
            struct v2f {
                float4 pos : SV_POSITION;
            };
            v2f vert (appdata v) {
                v2f o;
                float4 pos = float4(v.vertex.xyz + v.normal * _OutlineWidth, 1);
                o.pos = UnityObjectToClipPos(pos);
                return o;
            }
            fixed4 frag (v2f i) : SV_Target { return _OutlineColor; }
            ENDCG
        }

        // ② 主色 Pass（色块光照）
        Pass {
            Name "MAIN"
            Tags { "LightMode"="ForwardBase" }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdbase
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            sampler2D _MainTex, _Ramp;
            float4 _Color, _ShadowColor;
            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };
            struct v2f {
                float2 uv : TEXCOORD0;
                float4 pos : SV_POSITION;
                float3 worldNormal : TEXCOORD1;
                SHADOW_COORDS(2)
            };
            v2f vert (appdata v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                TRANSFER_SHADOW(o);
                return o;
            }
            fixed4 frag (v2f i) : SV_Target {
                fixed4 col = tex2D(_MainTex, i.uv) * _Color;
                float3 worldNormal = normalize(i.worldNormal);
                float3 worldLight = normalize(_WorldSpaceLightPos0.xyz);
                float NdotL = dot(worldNormal, worldLight);
                float ramp = tex2D(_Ramp, float2(NdotL * 0.5 + 0.5, 0.5)).r;
                float shadow = SHADOW_ATTENUATION(i);
                col.rgb = lerp(_ShadowColor, col.rgb, ramp * shadow);
                return col;
            }
            ENDCG
        }
    }
}