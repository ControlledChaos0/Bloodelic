// Modified based on https://docs.unity3d.com/Packages/com.unity.postprocessing@3.4/manual/Writing-Custom-Effects.html#hlsl-source-code
// and https://github.com/keijiro/DepthInverseProjection/blob/6b6d72d8178dbecdc29b037a175b95ea364bdbaa/Assets/InverseProjection/Resources/InverseProjection.shader

Shader "Hidden/Custom/ExternalLighting"
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
            
            #pragma vertex Vertex
            #pragma fragment Frag
            #define EXCLUDE_FAR_PLANE
            
            // System built-in variables
            TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
            // TEXTURE2D_SAMPLER2D(_CameraGBufferTexture2, sampler_CameraGBufferTexture2);
            TEXTURE2D_SAMPLER2D(_CameraDepthTexture, sampler_CameraDepthTexture);
            // TEXTURE2D_SAMPLER2D(_CameraDepthNormalsTexture, sampler_CameraDepthNormalsTexture);
            float4x4 unity_CameraInvProjection; // for some reason StdLib does not include this guy
            
            // for some reason UNITY_MATRIX_I_V is just the identity matrix, maybe because of the orthogonal projection...
            float4x4 _InverseView;
            float4x4 _LightMatrix;
            float3 _CameraOrthoDirection;
            sampler2D _ExternalLightCaptureHD;
            float4 _ShadowTexScale;
            sampler2D _Cookie;

            // These do not work because Unity doesn't want them to...
            // https://xibanya.github.io/UnityShaderViewer/Library/BuiltinShaders/CGIncludes/UnityShaderVariables.html
            // float4x4 unity_WorldToShadow[4];
            // float4 _LightSplitsNear;
            // float4 _LightSplitsFar;
            float4x4 _LightIntercepterData;
        
            struct Varyings
            {
                float4 position : SV_Position;
                float2 texcoord : TEXCOORD0;
                float3 ray : TEXCOORD1;
            };
        
            // Vertex shader that procedurally outputs a full screen triangle
            Varyings Vertex(uint vertexID : SV_VertexID)
            {
                // Render settings
                // float far = _ProjectionParams.z;
                float2 orthoSize = unity_OrthoParams.xy;
                // float isOrtho = unity_OrthoParams.w; // 0: perspective, 1: orthographic
        
                // Vertex ID -> clip space vertex position
                float x = (vertexID != 1) ? -1 : 3;
                float y = (vertexID == 2) ? -3 : 1;
                float3 vpos = float3(x, y, 1.0);
        
                // Perspective: view space vertex position of the far plane
                // float3 rayPers = mul(unity_CameraInvProjection, vpos.xyzz * far).xyz;
        
                // Orthographic: view space vertex position
                float3 rayOrtho = float3(orthoSize * vpos.xy, 0);
        
                Varyings o;
                o.position = float4(vpos.x, -vpos.y, 1, 1);
                o.texcoord = (vpos.xy + 1) / 2;
                // o.ray = lerp(rayPers, rayOrtho, isOrtho);
                o.ray = rayOrtho;
                return o;
            }
        
            float4 ComputeViewSpacePosition(Varyings input)
            {
                // Render settings
                float near = _ProjectionParams.y;
                float far = _ProjectionParams.z;
                // float isOrtho = unity_OrthoParams.w; // 0: perspective, 1: orthographic
        
                // Z buffer sample
                float z = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, input.texcoord);
        
                // Far plane exclusion
                #if !defined(EXCLUDE_FAR_PLANE)
                float mask = 1;
                #elif defined(UNITY_REVERSED_Z)
                float mask = z > 0;
                #else
                float mask = z < 1;
                #endif
        
                // Perspective: view space position = ray * depth
                // float depthPers = Linear01Depth(z);
                // float3 vposPers = input.ray;
        
                // Orthographic: linear depth (with reverse-Z support)
                #if defined(UNITY_REVERSED_Z)
                float depthOrtho = -lerp(far, near, z);
                #else
                float depthOrtho = -lerp(near, far, z);
                #endif
        
                // Orthographic: view space position
                float3 vposOrtho = float3(input.ray.xy, depthOrtho);
        
                // Result: view space position
                // return float4(lerp(vposPers, vposOrtho, isOrtho) * mask, lerp(depthPers, depthOrtho, isOrtho));
                return float4(vposOrtho * mask, depthOrtho);
            }

            int RayCone(
                float4 light_wpos, float3 light_wdir, float light_angle_deg,
                float4 ray_wpos, float3 ray_wdir, float ray_depth,
                out float4 hit_1, out float4 hit_2
            ) {
                hit_1 = 0;
                hit_2 = 0;

                // (1) https://lousodrome.net/blog/light/2017/01/03/intersection-of-a-ray-and-a-cone/
                // (2) https://doi.org/10.1016/B978-0-12-543457-7.50039-5
                
                // 1. Solve intersection between ray and the implicit surface of a tip-to-tip double cone (hourglass)
                
                float cos_cone = cos(radians(light_angle_deg));
                float cos2 = cos_cone * cos_cone;

                float dotDirs = dot(light_wdir, ray_wdir);

                float3 l2c = ray_wpos.xyz - light_wpos.xyz;

                float l2cDotLdir = dot(l2c, light_wdir);

                float a = dotDirs * dotDirs - cos2;
                float b = 2 * (dotDirs * l2cDotLdir - dot(ray_wdir, l2c) * cos2);
                float c = l2cDotLdir * l2cDotLdir - dot(l2c, l2c) * cos2;

                float four_a_c = 4 * a * c;
                float characteristic = b * b - four_a_c;

                if (characteristic <= 0) { // 1 hit case (char = 0) is too rare and i'll just pretend it doesn't exist
                    hit_1 = 0;
                    hit_2 = 0;
                    return 0;
                }

                // from here on it's always 2 hits to the double-cone 
                float inv_two_a = 1 / (2 * a);
                float sqrt_characteristic = sqrt(characteristic);
                
                // qualitative descriptions about the scenario
                float3 l2cN = normalize(l2c);
                bool viewingAgainstLight = dot(light_wdir, ray_wdir) > cos_cone;
                bool in_cone = dot(l2cN, light_wdir) > cos_cone;

                float t1 = (-b - sqrt_characteristic) * inv_two_a;
                float t2 = (-b + sqrt_characteristic) * inv_two_a;

                float3 p1 = ray_wpos.xyz + t1 * ray_wdir;
                float3 p2 = ray_wpos.xyz + t2 * ray_wdir;
                
                if (viewingAgainstLight) {
                    // take section from object all the way to the near clip plane
                    if (in_cone) {
                        hit_1 = ray_wpos + float4(ray_wdir, 0) * ray_depth;
                        hit_2 = ray_wpos;
                        return 2;
                    } else {
                        hit_1 = ray_wpos + float4(ray_wdir, 0) * ray_depth;
                        hit_2 = float4(p2, 1);
                        return 2;
                    }
                }

                bool p1_correct = t1 > 0 && dot(p1 - light_wpos.xyz, light_wdir) > 0;
                bool p2_correct = t2 > 0 && dot(p2 - light_wpos.xyz, light_wdir) > 0;
                
                // if (in_cone && (!p1_correct) && (!p2_correct)) {
                //     hit_1 = 1;
                //     hit_2 = 0;
                //     return 2;
                // }
                
                // float4 debug_colors[4] = {
                //     float4(0, 0, 0, 0),
                //     float4(1, 0, 0, 0),
                //     float4(0, 1, 0, 0),
                //     float4(0, 0, 1, 0),
                // };

                int state_code = p1_correct + p2_correct * 2;
                // hit_1 = debug_colors[state_code];

                // ugly patch, burn this with fire q-q
                if (state_code == 0 || state_code == 2) return 0;

                if (state_code == 1) {
                    p2 = ray_wpos.xyz;
                    p1 = p1;
                }

                hit_1 = float4(p1, 1);
                hit_2 = float4(p2, 1);

                return 2;
            }
        
            half4 VisualizePosition(Varyings input, float3 pos)
            {
                const float grid = 1;
                const float width = 3;
        
                pos *= grid;
        
                // Detect borders with using derivatives.
                float3 fw = fwidth(pos);
                half3 bc = saturate(width - abs(1 - 2 * frac(pos)) / fw);
        
                // Frequency filter
                half3 f1 = smoothstep(1 / grid, 2 / grid, fw);
                half3 f2 = smoothstep(2 / grid, 4 / grid, fw);
                bc = lerp(lerp(bc, 0.5, f1), 0, f2);
        
                // Blend with the source color.
                half4 c = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.texcoord);
                c.rgb = SRGBToLinear(lerp(LinearToSRGB(c.rgb), bc, 0.5));
        
                return c;
            }

            bool customShadow(float4 p) { // world space position p
                float4 light_space = mul(_LightMatrix, p);
                float2 light_uv = light_space.xy; // + stepOffset * 0.2;
                light_uv += _ShadowTexScale.xy / 2;
                light_uv /= _ShadowTexScale.xy;
                float mockDepth = light_space.z / _ShadowTexScale.z;

                // float2 light_offset = lightSpaceNorm * _ShadowTexScale.w;
                float sDepth = tex2D(_ExternalLightCaptureHD, light_uv).r;
                bool isShadow = (mockDepth - sDepth) > 0.0005; // shadow bias to prevent self-intersection
                return !isShadow;
            }

            float4 Frag(Varyings i) : SV_Target
            {
                float4 scene_col = _MainTex.Sample(sampler_MainTex, i.texcoord.xy);

                float4 pos_vs_depth = ComputeViewSpacePosition(i);
                float3 position_viewSpace = pos_vs_depth.xyz;
                float depth = pos_vs_depth.w;

                // cull background
                if (depth < -500) {
                    return scene_col;
                }

                float4 position_worldSpace = mul(_InverseView, float4(position_viewSpace, 1));

            #if defined(UNITY_REVERSED_Z)
                float vDepth = -depth;
            #else
                float vDepth = depth;
            #endif
                
                float4 light_position = float4(_LightIntercepterData[0].xyz, 1);
                float4 light_direction = float4(_LightIntercepterData[1].xyz, 0); // assume pre-normalized
                float3 light_color = _LightIntercepterData[2].xyz;

                float light_angle = _LightIntercepterData[3][0];
                float light_range = _LightIntercepterData[3][1];
                float light_intensity = _LightIntercepterData[3][2];
                float light_shadowStrength = _LightIntercepterData[3][3];

                float4 hit_1, hit_2;

                // inverse raycast from object to camera
                int hit_count = RayCone(
                    light_position, light_direction, light_angle / 2,
                    position_worldSpace, -_CameraOrthoDirection, vDepth,
                    hit_1, hit_2
                );

                if (hit_count < 2) {
                    return scene_col;
                }

                return hit_2;

                float4 acc = 0;
                float4 acc_ = 0;

                float3 step_Dir = hit_2.xyz - hit_1.xyz;
                float ray_max_len = length(step_Dir);
                step_Dir = normalize(step_Dir);
                float stepOffset = frac(19994 * sin(position_worldSpace.x * 74901 + scene_col.r * 0.1));

                float curr_len = 0;

                int inv_lod = 0; // LOD decreases as ray progresses
                bool last_is_shadow = 0;
                int step;

                for (step = 0; step < 16; step++) {

                    // in world units!
                    #define base_step_size 0.3

                    float step_size = (1 << (inv_lod / 4)) * base_step_size;
                    float4 p = hit_1 + float4(step_Dir, 0) * curr_len; // + stepOffset * step_size * 0.3;

                    // float4 p = lerp(hit_1, hit_2, step / (max_steps - 1.0));
                    
                    if (curr_len > ray_max_len) {
                        break;
                    }

                    bool isShadow = customShadow(p);
                    if (!isShadow) {
                        acc += ray_max_len / 16;
                    }

                    // float4 p_ = hit_2 + float4(step_Dir, 0) * curr_len;

                    // float4 light_space_ = mul(_LightMatrix, p_);
                    // float2 light_uv_ = light_space_.xy; // + stepOffset * 0.2;
                    // light_uv_ += _ShadowTexScale.xy / 2;
                    // light_uv_ /= _ShadowTexScale.xy;
                    // float4 lightSpacePos_ = mul(_LightMatrix, p_);
                    // float mockDepth_ = lightSpacePos_.z / _ShadowTexScale.z;

                    // // float2 light_offset = lightSpaceNorm * _ShadowTexScale.w;
                    // float sDepth_ = tex2D(_ExternalLightCaptureHD, light_uv_).r;
                    // bool isShadow_ = (sDepth_ - mockDepth_) < 0.1; // shadow bias to prevent self-intersection

                    // if (!isShadow_) {
                    //     acc_ = 0;
                    //     // acc_ += 1 * stepDist; // lerp(tex2D(_Cookie, light_uv), 1, 0.1)
                    // }

                    if (last_is_shadow != isShadow) {
                        inv_lod = max(inv_lod-2, 0);
                    }
                    last_is_shadow = isShadow;

                    stepOffset = frac(19994 * sin(p.x * 74901 + scene_col.r * 0.1)); // please switch to better hashing

                    inv_lod += 1;
                    
                    curr_len += step_size;
                }

                // left-over segment assumed to be all light
                // acc += max(ray_max_len - curr_len, 0);
                // acc_ += max(ray_max_len - curr_len, 0);

                // return hit_1;
                return acc * light_intensity; // scene_col + curr_len * light_intensity * float4(light_color, 1);

                // return float4(_CameraOrthoDirection, 1);
                // return _MainTex.Sample(sampler_MainTex, i.texcoord.xy);

            #undef POST_PROC_DISCARD
            }
            
            ENDHLSL
        }
    }
}
