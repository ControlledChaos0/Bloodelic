Shader "Custom/VHSEffect1"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _VignetteIntensity("Vignette Intensity", Range(0.0, 4.0)) = 3.0
        //_contrast("Contrast Intensity", Range(1.0, 1.2)) = 1.0
        _StripesDensity("Stripes Density", Range(1.0, 10.0)) = 5.0
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
            float4 _MainTex_TexelSize;
            float _VignetteIntensity;
            float _StripesDensity;
            //float _contrast;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float4 rgbOffset(float2 uv, sampler2D tex)
            {
                float3 color;
                color.r = tex2D(tex, uv + float2(0.003, 0)).x;
                color.g = tex2D(tex, uv).y;
                color.b = tex2D(tex, uv - float2(0.003, 0)).z;
                return float4(color,0.0);
            }

            float GoldNoise(float2 xy, float seed)
            {
                return frac(sin(dot(xy * seed, float2(12.9898, 78.233))) * 43758.5453);
            }


            float2 Tracking(float speed, float offset, float jitter, float2 fragCoord, float2 resolution, float time)
            {
                float t = 1.0 - fmod(time, speed) / speed;
                float trackingStart = fmod(t * resolution.y, resolution.y);
                float trackingJitter = GoldNoise(float2(5000.0, 5000.0), 10.0 + frac(time)) * jitter;
    
                trackingStart += trackingJitter;
    
                float2 uv = (fragCoord.y > trackingStart) ? (fragCoord + float2(offset, 0)) / resolution : fragCoord / resolution;

                return uv;
            }


            float4 WarpBottom(float height, float offset, float jitterExtent, float2 uv, sampler2D tex, float2 resolution, float time)
            {
                float uvHeight = height / resolution.y;
                if (uv.y > uvHeight)
                    return tex2D(tex, uv);
        
                float t = uv.y / uvHeight;
    
                float offsetUV = t * (offset / resolution.x);
                float jitterUV = (GoldNoise(float2(500.0, 500.0), frac(time)) * jitterExtent) / resolution.x; 
    
                uv = float2(uv.x - offsetUV - jitterUV, uv.y);
    
                float4 pixel = tex2D(tex, uv);
    
                return pixel * t;
            }

            float4 WhiteNoise(float lineThickness, float opacity, float4 pixel, float2 fragCoord, float2 resolution, float time)
            {
                if (GoldNoise(float2(600.0, 500.0), frac(time) * 10.0) > 0.97)
                {
                    float lineStart = floor(GoldNoise(float2(800.0, 50.0), frac(time)) * resolution.y);
                    float lineEnd = floor(lineStart + lineThickness);
        
                    if (floor(fragCoord.y) >= lineStart && floor(fragCoord.y) < lineEnd)
                    {
                        float frequency = GoldNoise(float2(850.0, 50.0), frac(time)) * 3.0 + 1.0;
                        float offset = GoldNoise(float2(900.0, 51.0), frac(time));            
                        float x = floor(fragCoord.x) / floor(resolution.x) + offset;
                        float white = pow(cos(3.14159265 * frac(x * frequency) / 2.0), 10.0) * opacity;
                        float grit = GoldNoise(float2(floor(fragCoord.x / 3.0), 800.0), frac(time));
                        white = max(white - grit * 0.3, 0.0);
                        
                        return pixel + white;
                    }
                }
    
                return pixel;
            }


            //float3 AdjustContrast(float3 color)
            //{
            //    float3 midpoint = float3(0.5, 0.5, 0.5);
            //    float3 adjustedColor = (color - midpoint) * _contrast + midpoint;
            //    adjustedColor = clamp(adjustedColor, 0.0, 1.0);
            //    return adjustedColor;
            //}


            float4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;
                float2 trackedUV = Tracking(8.0, 8.0, 20.0, i.vertex.xy,_ScreenParams.xy, _Time.y);
                trackedUV.y = 1.0 - trackedUV.y;
                //float2 wavedUV = Wave(70.0, 1.0, i.vertex.xy, trackedUV, _ScreenParams.xy,  _Time.y);
                float4 output = rgbOffset(trackedUV, _MainTex);

                //vignette
                float vignetteIntensity = _VignetteIntensity + 0.1 * sin(_Time.y);
                float vignette = (1.0 - vignetteIntensity * (uv.y - 0.5) * (uv.y - 0.5)) * (1.0 - vignetteIntensity * (uv.x - 0.5) * (uv.x - 0.5));
                output.rgb *= vignette; 

                output = WhiteNoise(6.0, 0.3, output, i.vertex.xy, _ScreenParams.xy, _Time.y);

                //output = AdjustContrast(output);
                output.rgb *= 1.0 + 0.1 * sin(10.0 * _Time.y + uv.y * _StripesDensity * 100.0);
                output.rgb *= 0.99 + 0.01 * sin(110.0 * _Time.y);

                return output;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
