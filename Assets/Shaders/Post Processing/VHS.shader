// Modified based on https://docs.unity3d.com/Packages/com.unity.postprocessing@3.4/manual/Writing-Custom-Effects.html#hlsl-source-code
// and https://github.com/keijiro/DepthInverseProjection/blob/6b6d72d8178dbecdc29b037a175b95ea364bdbaa/Assets/InverseProjection/Resources/InverseProjection.shader

Shader "Hidden/Custom/VHS"
{
    SubShader
    {
        Cull Off ZWrite Off ZTest Always
        Pass
        {
            HLSLPROGRAM
                    
            // See https://github.com/Unity-Technologies/Graphics/blob/master/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl
            #include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"
            #include "Packages/com.unity.postprocessing/PostProcessing/Shaders/Colors.hlsl"
            #include "../Helpers/Hash.cginc"
            
            #pragma vertex VertDefault
            #pragma fragment Frag
            
            TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
            // sampler2D _MainTex;
            // float4 _MainTex_TexelSize;
            float _VignetteIntensity;
            float _StripesDensity;
            float _TrackingSpeed;
            float _RedBlueOffset;
        
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

            float4 Frag(VaryingsDefault i) : SV_Target
            {
                float2 uv = i.texcoord.xy;

                #define DEBUGUV(u) return float4(u.x, u.y, 0, 1);

                //tracking line
                float2 trackedUV = Tracking(_TrackingSpeed, 2.0, 10.0, i.vertex.xy);
                
                // trackedUV.y = 1.0 - trackedUV.y;

                //bottom wrap
                float2 warpedUV = WarpBottomUVs(7.0, 50.0, 30.0, 5.0, trackedUV);
                
                //color
                float4 output = 1; // _MainTex.Sample(sampler_MainTex, i.texcoord.xy); // rgbOffset(warpedUV, scene_col);


                // return output;
                output.r = _MainTex.Sample(sampler_MainTex, warpedUV + float2(_RedBlueOffset*0.001, 0)).r;
                output.g = _MainTex.Sample(sampler_MainTex, warpedUV).g;
                output.b = _MainTex.Sample(sampler_MainTex, warpedUV - float2(_RedBlueOffset*0.001, 0)).b;

                //vignette
                float vignetteIntensity = _VignetteIntensity + 0.1 * sin(_Time.y);
                float vignette = (1.0 - vignetteIntensity * (uv.y - 0.5) * (uv.y - 0.5)) * (1.0 - vignetteIntensity * (uv.x - 0.5) * (uv.x - 0.5));
                output.rgb *= vignette; 

                output = WhiteNoise(3.0, 0.3, output, i.texcoord.xy);

                //output = AdjustContrast(output);
                //stripes
                output.rgb *= 1.0 + 0.1 * sin(10.0 * _Time.y + uv.y * _StripesDensity * 100.0);
                output.rgb *= 0.99 + 0.01 * sin(110.0 * _Time.y);

                return output;
            }
            
            ENDHLSL
        }
    }
}
