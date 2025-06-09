Shader "Custom/ContourDetection"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Threshold ("Edge Threshold", Range(0, 1)) = 0.2
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            float _Threshold;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            v2f vert (appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_OUTPUT(v2f, o);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
                float2 texel = _MainTex_TexelSize.xy;

                float3 c00 = tex2D(_MainTex, i.uv + texel * float2(-1, -1)).rgb;
                float3 c10 = tex2D(_MainTex, i.uv + texel * float2( 0, -1)).rgb;
                float3 c20 = tex2D(_MainTex, i.uv + texel * float2( 1, -1)).rgb;
                float3 c01 = tex2D(_MainTex, i.uv + texel * float2(-1,  0)).rgb;
                float3 c21 = tex2D(_MainTex, i.uv + texel * float2( 1,  0)).rgb;
                float3 c02 = tex2D(_MainTex, i.uv + texel * float2(-1,  1)).rgb;
                float3 c12 = tex2D(_MainTex, i.uv + texel * float2( 0,  1)).rgb;
                float3 c22 = tex2D(_MainTex, i.uv + texel * float2( 1,  1)).rgb;

                float3 sobelX = -c00 - 2.0 * c01 - c02 + c20 + 2.0 * c21 + c22;
                float3 sobelY = -c00 - 2.0 * c10 - c20 + c02 + 2.0 * c12 + c22;

                float edge = length(sobelX + sobelY);
                return edge > _Threshold ? float4(1,1,1,1) : float4(0,0,0,1);
            }
            ENDCG
        }
    }
}
