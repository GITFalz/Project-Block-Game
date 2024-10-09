Shader "Custom/TextureArrayShader"
{
    Properties
    {
        _MainTex("Albedo", 2DArray) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma exclude_renderers gles xbox360 ps3

            // Define the texture array
            UNITY_DECLARE_TEX2DARRAY(_MainTex);

            // Vertex input structure
            struct appdata
            {
                float4 vertex : POSITION;
                float3 texcoord : TEXCOORD0; // UVs and array index
            };

            // Vertex output structure
            struct v2f
            {
                float4 pos : POSITION;
                float2 uv_MainTex : TEXCOORD0;
                float arrayIndex : TEXCOORD1;
            };

            // Vertex shader function
            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv_MainTex = v.texcoord.xy; // UVs
                o.arrayIndex = v.texcoord.z;   // Array index
                return o;
            }

            // Fragment shader function
            half4 frag(v2f i) : SV_Target
            {
                // Sample the texture array
                half4 c = UNITY_SAMPLE_TEX2DARRAY(_MainTex, float3(i.uv_MainTex, i.arrayIndex));
                return c;
            }
            ENDCG
        }
    }
    Fallback "VertexLit"
}
