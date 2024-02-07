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
            TEXTURE2D_SAMPLER2D(_CameraGBufferTexture2, sampler_CameraGBufferTexture2);
            TEXTURE2D_SAMPLER2D(_CameraDepthTexture, sampler_CameraDepthTexture);
            TEXTURE2D_SAMPLER2D(_CameraDepthNormalsTexture, sampler_CameraDepthNormalsTexture);
            float4x4 unity_CameraInvProjection; // for some reason StdLib does not include this guy
            
            // for some reason UNITY_MATRIX_I_V is just the identity matrix, maybe because of the orthogonal projection...
            float4x4 _InverseView;

            // https://xibanya.github.io/UnityShaderViewer/Library/BuiltinShaders/CGIncludes/UnityShaderVariables.html
            float4x4 unity_WorldToShadow[4];
            float4 _LightSplitsNear;
            float4 _LightSplitsFar;

            sampler2D _ShadowTex;
        
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
                float far = _ProjectionParams.z;
                float2 orthoSize = unity_OrthoParams.xy;
                float isOrtho = unity_OrthoParams.w; // 0: perspective, 1: orthographic
        
                // Vertex ID -> clip space vertex position
                float x = (vertexID != 1) ? -1 : 3;
                float y = (vertexID == 2) ? -3 : 1;
                float3 vpos = float3(x, y, 1.0);
        
                // Perspective: view space vertex position of the far plane
                float3 rayPers = mul(unity_CameraInvProjection, vpos.xyzz * far).xyz;
        
                // Orthographic: view space vertex position
                float3 rayOrtho = float3(orthoSize * vpos.xy, 0);
        
                Varyings o;
                o.position = float4(vpos.x, -vpos.y, 1, 1);
                o.texcoord = (vpos.xy + 1) / 2;
                o.ray = lerp(rayPers, rayOrtho, isOrtho);
                return o;
            }
        
            float3 ComputeViewSpacePosition(Varyings input)
            {
                // Render settings
                float near = _ProjectionParams.y;
                float far = _ProjectionParams.z;
                float isOrtho = unity_OrthoParams.w; // 0: perspective, 1: orthographic
        
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
                float3 vposPers = input.ray * Linear01Depth(z);
        
                // Orthographic: linear depth (with reverse-Z support)
                #if defined(UNITY_REVERSED_Z)
                float depthOrtho = -lerp(far, near, z);
                #else
                float depthOrtho = -lerp(near, far, z);
                #endif
        
                // Orthographic: view space position
                float3 vposOrtho = float3(input.ray.xy, depthOrtho);
        
                // Result: view space position
                return lerp(vposPers, vposOrtho, isOrtho) * mask;
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

            // Took some digging... https://github.com/Unity-Technologies/Graphics/blob/master/com.unity.postprocessing/PostProcessing/Shaders/Builtins/ScreenSpaceReflections.hlsl
            // float3 GetViewSpacePosition(float2 uv)
            // {
            //     float depth = _CameraDepthTexture.SampleLevel(sampler_CameraDepthTexture, UnityStereoTransformScreenSpaceTex(uv), 0).r;
            //     float4 result = float4(float2(2.0 * uv - 1.0) * float2(unity_CameraInvProjection[0][0], unity_CameraInvProjection[1][1]),
            //                         depth * unity_CameraInvProjection[2][2] + unity_CameraInvProjection[2][3],
            //                         _ZBufferParams . z * depth + _ZBufferParams . w); // Use _ZBufferParams as it accounts for 0...1 depth value range
            //     return result.xyz / result.w;
            // }

            float4 Frag(Varyings i) : SV_Target
            {
                float3 pos_vs_depth = ComputeViewSpacePosition(i);
                float3 position_viewSpace = pos_vs_depth.xyz;
                // float depth = pos_vs_depth.w;

                float4 position_worldSpace = mul(_InverseView, position_viewSpace);

            //     float3 shadowCoord0 = mul(unity_WorldToShadow[0], position_worldSpace).xyz; 
            //     float3 shadowCoord1 = mul(unity_WorldToShadow[1], position_worldSpace).xyz;
            //     float3 shadowCoord2 = mul(unity_WorldToShadow[2], position_worldSpace).xyz;
            //     float3 shadowCoord3 = mul(unity_WorldToShadow[3], position_worldSpace).xyz;
            
            // #if defined(UNITY_REVERSED_Z)
            //     float vDepth = -depth;
            // #else
            //     float vDepth = depth;
            // #endif

            //     float4 near = float4 (vDepth >= _LightSplitsNear); 
            //     float4 far = float4 (vDepth < _LightSplitsFar);
            //     float4 weights = near * far;

            //     float3 coord = 
            //         shadowCoord1 * weights.x + 	// case: Cascaded one
            //         shadowCoord2 * weights.y + 	// case: Cascaded two
            //         shadowCoord3 * weights.z + 	// case: Cascaded three
            //         shadowCoord0 * weights.w; 	// case: Cascaded four
            //     // float4 position_shadow = mul(unity_WorldToShadow, position_worldSpace);

            //     float4 shadowSample = tex2D(_ShadowTex, i.texcoord.xy);
            //     float shadowMask = shadowSample.g;

                // // sample each shadow cascade: https://docs.unity3d.com/Manual/SL-UnityShaderVariables.html
                // for (int cascade = 0; cascade < 3; cascade = cascade + 1) {
                //     float4x4 cascade_WorldToShadow = unity_WorldToShadow[cascade];
                // }
                
                return VisualizePosition(i, position_worldSpace);
                // return _MainTex.Sample(sampler_MainTex, i.texcoord.xy);
            }
            
            ENDHLSL
        }
    }
}
