Shader "Custom/VHSEffect"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _SecondaryTex ("Secondary Texture", 2D) = "white" {}
        _NoiseAmount("Noise Amount", Range(0.0, 1.0)) = 0.5
        _VignetteIntensity("Vignette Intensity", Range(0.0, 10.0)) = 1.0
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
            sampler2D _SecondaryTex;
            float4 _MainTex_ST;

            float _NoiseAmount;
            float _VignetteIntensity;


            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            float noise(float2 p)
            {
                float s = tex2D(_SecondaryTex, float2(1.0, 2.0 * cos(_Time.y)) * _Time.y * 8.0 + p * 1.0).r;
                s *= s * _NoiseAmount;
                return s;
            }

            float onOff(float a, float b, float c)
            {
                return step(c, sin(_Time.y + a * cos(_Time.y * b)));
            }

            float ramp(float y, float start, float end)
            {
                float inside = step(start, y) - step(end, y);
                float fact = (y - start) / (end - start) * inside;
                return (1.0 - fact) * inside;
            }

            float stripes(float2 uv)
            {
                float noi = noise(uv * float2(0.5, 1.0) + float2(1.0, 3.0));
                return ramp(fmod(uv.y * 2.0 + _Time.y / 2.0 + sin(_Time.y + sin(_Time.y * 0.63)), 1.0), 0.5, 0.6) * noi;
            }

            float3 getVideo(float2 uv)
            {
                float2 look = uv;
                float window = 1.0 / (1.0 + 20.0 * (look.y - fmod(_Time.y / 4.0, 1.0)) * (look.y - fmod(_Time.y / 4.0, 1.0)));
    


                float3 video = tex2D(_MainTex, look).rgb;
                return video;
            }

            float2 screenDistort(float2 uv)
            {
                uv -= float2(0.5, 0.5);
                uv = uv*1.2*(1./1.2+2.*uv.x*uv.x*uv.y*uv.y);
                uv += float2(0.5, 0.5);
                return uv;
            }

            float4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;
                uv = screenDistort(uv);
                float3 video = getVideo(uv);
                float vigAmt = _VignetteIntensity + 0.3 * sin(_Time.y + 5.0 * cos(_Time.y * 5.0));
                float vignette = (1.0 - vigAmt * (uv.y - 0.5) * (uv.y - 0.5)) * (1.0 - vigAmt * (uv.x - 0.5) * (uv.x - 0.5));
                video += stripes(uv);
                video += noise(uv * 2.0) / 2.0;
                video *= vignette;

                return float4(video, 1.0);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
