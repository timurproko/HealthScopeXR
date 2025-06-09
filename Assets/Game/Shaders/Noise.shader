Shader "Custom/URP/SemiSphereFadeVR_Background"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
        _Alpha("Overall Opacity", Range(0,1)) = 1
        _Falloff("Fade Radius", Range(0.01, 2)) = 1.0
        _ClipY("Clip Height From Top", Range(-1, 1)) = 0.0
        _InnerContrast("Center Contrast", Range(1, 10)) = 1.0
    }

    SubShader
    {
        // ✅ Render early in transparent queue to draw behind other transparents
        Tags { "RenderType"="Transparent" "Queue"="Transparent-100" }
        LOD 100
        ZWrite Off           // Don’t write to depth
        ZTest LEqual         // Respect existing depth (draw behind if needed)
        Cull Front           // Simulate being inside the sphere
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            Name "FadeClipContrastVR"
            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog
            #pragma multi_compile_instancing
            #pragma multi_compile _ _STEREO_MULTIVIEW_ON
            #pragma multi_compile _ _XR_PASS_ENABLED

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 localPos : TEXCOORD0;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            float4 _Color;
            float _Alpha;
            float _Falloff;
            float _ClipY;
            float _InnerContrast;

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                UNITY_SETUP_INSTANCE_ID(IN);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

                OUT.localPos = IN.positionOS.xyz;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS); // ✅ XR compatible
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                // ✂️ Clip top portion — keep below _ClipY
                if (IN.localPos.y > _ClipY)
                    discard;

                // 🌐 Radial fade in XZ plane, with contrast
                float radialDist = length(IN.localPos.xz);
                float fade = saturate(1.0 - pow(radialDist / _Falloff, _InnerContrast));

                float finalAlpha = _Color.a * _Alpha * fade;
                return float4(_Color.rgb, finalAlpha);
            }

            ENDHLSL
        }
    }

    FallBack Off
}
