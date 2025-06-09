Shader "Custom/ScanningPulseStereoBlend"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _PulseColor ("Pulse Color", Color) = (0.3, 0.6, 1.0, 1)
        _PulseSpeed ("Pulse Speed", Float) = 2.0
        _PulseStrength ("Pulse Strength", Range(0,2)) = 1.0
        _ScanLineSpeed ("Scan Line Speed", Float) = 1.0
        _ScanLineWidth ("Scan Line Width", Range(0, 1)) = 0.05
        _BlendStrength ("Blend With Image", Range(0, 1)) = 1.0
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _PulseColor;
            float _PulseSpeed;
            float _PulseStrength;
            float _ScanLineSpeed;
            float _ScanLineWidth;
            float _BlendStrength;

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

            v2f vert(appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);

                float4 texColor = tex2D(_MainTex, i.uv);

                // Pulsing glow
                float pulse = 0.5 + 0.5 * sin(_Time.y * _PulseSpeed);
                float4 glow = _PulseColor * pulse * _PulseStrength;

                // Horizontal scan wave
                float scanLine = smoothstep(0.0, _ScanLineWidth, abs(frac(i.uv.y + _Time.y * _ScanLineSpeed) - 0.5));
                glow += _PulseColor * scanLine;

                // Blend final effect
                float4 finalColor = lerp(texColor, texColor + glow, _BlendStrength);
                return finalColor;
            }
            ENDCG
        }
    }
}
