Shader "Custom/Heart"
{
    Properties
    {
        _Albedo ("Base Color", 2D) = "white" {}
        _Normal ("Normal Map", 2D) = "bump" {}
        _Roughness ("Roughness Map", 2D) = "black" {}
        _Metallic ("Metallic Map", 2D) = "black" {}

        _ClipXMin ("Clip X Min", Range(0, 1)) = 0.0
        _ClipXMax ("Clip X Max", Range(0, 1)) = 1.0
        _ClipX ("Clip X Value", Range(0, 1)) = 1.0
        _InvertX ("Invert X Clip", Float) = 0.0

        _ClipYMin ("Clip Y Min", Range(0, 1)) = 0.0
        _ClipYMax ("Clip Y Max", Range(0, 1)) = 1.0
        _ClipY ("Clip Y Value", Range(0, 1)) = 1.0
        _InvertY ("Invert Y Clip", Float) = 0.0

        _ClipZMin ("Clip Z Min", Range(0, 1)) = 0.0
        _ClipZMax ("Clip Z Max", Range(0, 1)) = 1.0
        _ClipZ ("Clip Z Value", Range(0, 1)) = 1.0
        _InvertZ ("Invert Z Clip", Float) = 0.0

        _WorldScale ("World Scale", Vector) = (1, 1, 1, 1)
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "Cull"="Off" }

        Pass
        {
            Cull Off
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #pragma multi_compile_fog
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_Albedo); SAMPLER(sampler_Albedo);
            TEXTURE2D(_Normal); SAMPLER(sampler_Normal);
            TEXTURE2D(_Roughness); SAMPLER(sampler_Roughness);
            TEXTURE2D(_Metallic); SAMPLER(sampler_Metallic);

            float _ClipXMin, _ClipXMax, _ClipX, _InvertX;
            float _ClipYMin, _ClipYMax, _ClipY, _InvertY;
            float _ClipZMin, _ClipZMax, _ClipZ, _InvertZ;
            float3 _WorldScale;

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float4 tangentOS : TANGENT;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 viewDir : TEXCOORD0;
                float3x3 TBN : TEXCOORD1;
                float2 uv : TEXCOORD4;
                float3 normLocal : TEXCOORD5;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                UNITY_SETUP_INSTANCE_ID(IN);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

                float3 worldPos = TransformObjectToWorld(IN.positionOS.xyz);
                OUT.positionHCS = TransformWorldToHClip(worldPos);
                OUT.uv = IN.uv;

                float3 scaledPos = IN.positionOS.xyz / _WorldScale;
                OUT.normLocal = saturate(scaledPos + 0.5);

                float3 normalWS = TransformObjectToWorldNormal(IN.normalOS);
                float3 tangentWS = normalize(TransformObjectToWorldDir(IN.tangentOS.xyz));
                float3 bitangentWS = cross(normalWS, tangentWS) * IN.tangentOS.w;
                OUT.TBN = float3x3(tangentWS, bitangentWS, normalWS);
                OUT.viewDir = normalize(_WorldSpaceCameraPos - worldPos);
                return OUT;
            }

            float4 frag(Varyings IN) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);

                float3 normLocal = IN.normLocal;

                float clipXValue = lerp(_ClipXMin, _ClipXMax, _ClipX);
                float clipYValue = lerp(_ClipYMin, _ClipYMax, _ClipY);
                float clipZValue = lerp(_ClipZMin, _ClipZMax, _ClipZ);

                if ((_InvertX > 0.5 && normLocal.x < clipXValue) || (_InvertX <= 0.5 && normLocal.x > clipXValue)) discard;
                if ((_InvertY > 0.5 && normLocal.y < clipYValue) || (_InvertY <= 0.5 && normLocal.y > clipYValue)) discard;
                if ((_InvertZ > 0.5 && normLocal.z < clipZValue) || (_InvertZ <= 0.5 && normLocal.z > clipZValue)) discard;

                float3 albedo = SAMPLE_TEXTURE2D(_Albedo, sampler_Albedo, IN.uv).rgb;
                float roughness = SAMPLE_TEXTURE2D(_Roughness, sampler_Roughness, IN.uv).r;
                float metallic = SAMPLE_TEXTURE2D(_Metallic, sampler_Metallic, IN.uv).r;

                float3 N = normalize(mul(IN.TBN, UnpackNormal(SAMPLE_TEXTURE2D(_Normal, sampler_Normal, IN.uv))));
                float3 V = normalize(IN.viewDir);
                float3 R = reflect(-V, N);

                float3 F0 = lerp(float3(0.04, 0.04, 0.04), albedo, metallic);

                float3 fresnel = F0 + (1.0 - F0) * pow(1.0 - saturate(dot(N, V)), 5.0);
                float gloss = 1.0 - roughness;
                float fresnelFactor = saturate(dot(N, V));
                float3 specular = fresnel * gloss * fresnelFactor;

                float3 color = albedo + specular;
                return float4(color, 1.0);
            }
            ENDHLSL
        }
    }
    FallBack Off
}