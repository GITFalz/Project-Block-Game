Shader "Custom/TextureArrayShaderWithDirectionalShading"
{
    Properties
    {
        _mainTexture("Albedo", 2DArray) = "white" {}
        _ShadeDirection("Shade Direction", Vector) = (0, 0, 1, 0)
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        
        Pass
        {
            Tags { "LightMode" = "ForwardBase" }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            UNITY_DECLARE_TEX2DARRAY(_mainTexture);
            
            float4 _ShadeDirection;

            struct appdata
            {
                float4 vertex : POSITION;
                float3 texcoord : TEXCOORD0; 
                float3 normal : NORMAL;
            };
            
            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv_MainTex : TEXCOORD0;
                float arrayIndex : TEXCOORD1;
                float3 worldNormal : TEXCOORD2;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv_MainTex = v.texcoord.xy; 
                o.arrayIndex = v.texcoord.z;

                // Transform the normal to world space
                o.worldNormal = normalize(mul((float3x3)unity_ObjectToWorld, v.normal));

                return o;
            }
            
            half4 frag(v2f i) : SV_Target
            {
                /*
                half4 color = UNITY_SAMPLE_TEX2DARRAY(_mainTexture, float3(i.uv_MainTex, i.arrayIndex));
                
                float3 shadeDir = normalize(_ShadeDirection.xyz);
                float alignment = saturate(dot(i.worldNormal, shadeDir));
                color.rgb *= lerp(1.0, 0.5, alignment);

                return color;
                */

                half4 color = UNITY_SAMPLE_TEX2DARRAY(_mainTexture, float3(i.uv_MainTex, i.arrayIndex));

                float3 shadeDir = normalize(_ShadeDirection.xyz);
                float alignment = saturate(dot(i.worldNormal, shadeDir));
                color.rgb *= lerp(1.0, 0.5, alignment);
                
                color.a = max(color.a, 0.05);

                return color;
            }
            ENDCG
        }

        Pass
        {
            Tags { "LightMode" = "ShadowCaster" }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };
            
            struct v2f
            {
                float4 pos : SV_POSITION;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }
            
            float4 frag(v2f i) : SV_Target
            {
                return float4(0, 0, 0, 0);
            }
            ENDCG
        }
    }
    Fallback "VertexLit"
}