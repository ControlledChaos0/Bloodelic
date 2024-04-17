Shader "Custom/VHSEffect"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _VignetteIntensity("Vignette Intensity", Range(0.0, 4.0)) = 2.0
        //_contrast("Contrast Intensity", Range(1.0, 1.2)) = 1.0
        _StripesDensity("Stripes Density", Range(1.0, 10.0)) = 5.0
        _TrackingSpeed("Tracking Speed", Range(1.0, 15.0)) = 7.0
        _RedBlueOffset("Red Blue Offset", Range(0.0, 6.0)) = 3.0
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
            float _TrackingSpeed;
            float _RedBlueOffset;
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
                color.r = tex2D(tex, uv + float2(_RedBlueOffset*0.001, 0)).x;
                color.g = tex2D(tex, uv).y;
                color.b = tex2D(tex, uv - float2(_RedBlueOffset*0.001, 0)).z;
                return float4(color,0.0);
            }

            float GoldNoise(float2 xy, float seed)
            {
                return frac(sin(dot(xy * seed, float2(12.9898, 78.233))) * 43758.5453);
            }

            float2 Tracking(float speed, float offset, float jitter, float2 fragCoord)
            {
                float t = 1.0 - fmod(_Time.y, speed) / speed;
                float trackingStart = fmod(t * _ScreenParams.y, _ScreenParams.y);
                float trackingJitter = GoldNoise(float2(5000.0, 5000.0), 10.0 + frac(_Time.y)) * jitter;
                trackingStart += trackingJitter;
                bool isAboveTrackingLine = fragCoord.y > trackingStart;
                float proximity = abs(fragCoord.y - trackingStart) / _ScreenParams.y; 
                float dynamicOffset = offset * (1.0 - smoothstep(0.0, 0.05, proximity)); 
                float2 uv;
                if (isAboveTrackingLine)
                {         
                    uv = (fragCoord + float2(dynamicOffset, 0.0)) / _ScreenParams;
                }
                else
                {
                    uv = (fragCoord - float2(dynamicOffset, 0.0)) / _ScreenParams;
                }
                return uv;
            }

            float2 WarpBottomUVs(float height, float offset, float jitterExtent,float smoothness, float2 uv)
            {
                float uvHeight = height / _ScreenParams.y;
                float edgeSmoothness = smoothness / _ScreenParams.y; 
                float smoothStepFactor = smoothstep(uvHeight - edgeSmoothness, uvHeight, uv.y);

                if (uv.y <= uvHeight)
                {
                    float t = uv.y / uvHeight;
                    float offsetUV = t * (offset / _ScreenParams.x);
                    float jitterUV = (GoldNoise(float2(500.0, 500.0), frac(_Time.y)) * jitterExtent) / _ScreenParams.x; 
                    uv.x -= (offsetUV + jitterUV) * (1.0 - smoothStepFactor);
                }

                return uv;
            }



            //credit to https://www.shadertoy.com/view/sltBWM
            float4 WhiteNoise(float lineThickness, float opacity, float4 pixel, float2 fragCoord)
            {
                if (GoldNoise(float2(600.0, 500.0), frac(_Time.y) * 10.0) > 0.97)
                {
                    float lineStart = floor(GoldNoise(float2(800.0, 50.0), frac(_Time.y)) * _ScreenParams.y);
                    float lineEnd = floor(lineStart + lineThickness);
        
                    if (floor(fragCoord.y) >= lineStart && floor(fragCoord.y) < lineEnd)
                    {
                        float frequency = GoldNoise(float2(850.0, 50.0), frac(_Time.y)) * 3.0 + 1.0;
                        float offset = GoldNoise(float2(900.0, 51.0), frac(_Time.y));            
                        float x = floor(fragCoord.x) / floor(_ScreenParams.x) + offset;
                        float white = pow(cos(3.14159265 * frac(x * frequency) / 2.0), 10.0) * opacity;
                        float grit = GoldNoise(float2(floor(fragCoord.x / 3.0), 800.0), frac(_Time.y));
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
                #define DEBUGUV(uv) return float4((uv).x, (uv).y, 0, 1);
                float2 uv = i.uv;

                //tracking line
                float2 trackedUV = Tracking(_TrackingSpeed, 2.0, 10.0, i.vertex.xy);
                trackedUV.y = 1.0 - trackedUV.y;
                //bottom wrap
                float2 warpedUV = WarpBottomUVs(7.0, 50.0, 30.0, 5.0, trackedUV);
                
                //color
                float4 output = rgbOffset(warpedUV, _MainTex);

                //vignette
                float vignetteIntensity = _VignetteIntensity + 0.1 * sin(_Time.y);
                float vignette = (1.0 - vignetteIntensity * (uv.y - 0.5) * (uv.y - 0.5)) * (1.0 - vignetteIntensity * (uv.x - 0.5) * (uv.x - 0.5));
                output.rgb *= vignette; 

                output = WhiteNoise(3.0, 0.3, output, i.vertex.xy);

                //output = AdjustContrast(output);
                //stripes
                output.rgb *= 1.0 + 0.1 * sin(10.0 * _Time.y + uv.y * _StripesDensity * 100.0);
                output.rgb *= 0.99 + 0.01 * sin(110.0 * _Time.y);

                return output;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
