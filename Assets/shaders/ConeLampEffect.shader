Shader "A/ConeLampEffect"
{
    Properties
    {
        [HDR]_Color("Color(RGB)", Color) = (1,1,1,1)
        _NoiseTexture("Noise Texture", 2D) = "white" {}
        _NoiseOffsetSpeed("Noise Offset Speed", Float) = 1
        _NoiseTilling("Noise Tilling(XY)", Vector) = (1,1,1,1) 
        _SmoothStepMin("SmoothStep Min", Float) = 0
        _SmoothStepMax("SmoothStep Max", Float) = 1
        _FadeOffset("Fade Offset", Float) = 0
        _FadeFactor("Fade Factor", Float) = 1
        _Expand("Expand", Float) = 1
        [Enum(UnityEngine.Rendering.CullMode)]_CullMode("CullMode", Float) = 0
        [Enum(UnityEngine.Rendering.BlendMode)]_BlendMode("BlendMode", Float) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }

        Pass
        {
            Blend SrcAlpha [_BlendMode]
            Cull [_CullMode]
            
            CGPROGRAM
            fixed3 _Color;
            sampler2D _NoiseTexture;
            float _NoiseOffsetSpeed;
            float2 _NoiseTilling;
            float _SmoothStepMin;
            float _SmoothStepMax;
            float _FadeOffset;
            float _FadeFactor;
            float _Expand;
            
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "A_Tools.cginc"
            
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal: NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 normal: NORMAL;
                float3 worldPos: TEXCOORD1;
            };
            
            v2f vert (appdata v)
            {
                v2f o;
                
                float3 worldNormal = UnityObjectToWorldNormal(v.normal);
                float3 worldPos = mul(unity_ObjectToWorld, v.vertex);
                float3 expand = worldNormal * _Expand * v.uv.x;
                worldPos = worldPos + expand;
                float3 objectPos = mul(unity_WorldToObject, float4(worldPos, 1));
                
                o.vertex = UnityObjectToClipPos(objectPos);
                o.worldPos = worldPos;
                o.normal = worldNormal;
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float3 worldView = normalize(_WorldSpaceCameraPos - i.worldPos);
                float noise = tex2D(_NoiseTexture, i.uv * _NoiseTilling + float2(_Time.y * _NoiseOffsetSpeed, 0 
                ));
                noise = 0.8 * noise;
                float noise2 = tex2D(_NoiseTexture, i.uv * _NoiseTilling * 0.8 + float2(0.8 * _Time.y * _NoiseOffsetSpeed, 0.5 
                ));;
                noise = noise + noise2;

                // 左右边缘
                float lrMask = dot(worldView, i.normal);
                lrMask = smoothstep(lrMask, _SmoothStepMin, _SmoothStepMax);
                lrMask = clamp(lrMask, 0, 1);

                // 上边缘
                float upMask0 = 1 - i.uv.x;
                float upMask = upMask0 - _FadeOffset;
                upMask = upMask * _FadeFactor;
                upMask = clamp(upMask, 0, upMask0);
                
                fixed4 finalColor;
                finalColor.rgb = _Color * noise + 0.2 * _Color;
                finalColor.a = (noise+0.3) * lrMask * upMask * 1.5;
                return OutputTestColor(finalColor);
            }
            ENDCG
        }
    }
}
