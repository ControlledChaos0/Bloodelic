Shader "Anthony/Props/Disco Ball"
{
    Properties
    {
        [Header(PBR)]
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Specular ("Specular", Range(0,1)) = 0.0

        [Space]
        [Header(Specular)] [Space]
        _SpecularColor("Specular Color", Color) = (1,1,1,1)
        _SpecularPower("Specular Power", Float) = 1
        _SpecularStrength("Specular Strength", Range(0, 2)) = .5
        _SpecularFalloff ("Specular Fall Off", Range(0, 0.3)) = 0.2

        [Space]
        [Header(Rim)] [Space]
        _FresnelColor("Rim Color", Color) = (1,1,1,1)
        _FresnelPower("Rim Power", Range(0,10)) = 1
        _FresnelStrength("Rim Strength", Float) = 1
        [Toggle(SHADOWED_RIM)]_ShadowedRim("Rim affected by shadow", float) = 0

        [Space]
        [Header(Diffraction)] [Space]
        _DiffDistance("Grating distance", Range(0, 10000)) = 1600 // nm
        _DiffOrder("Wavelength Multiplles", Range(1,8)) = 2
        _DiffAlpha("Diffraction Alpha", Range(0, 1)) = 0.5
    }
    CGINCLUDE
        // Standard UnitCG + Lighting
        #include "UnityCG.cginc"
        #include "AnthonyLightingLibrary.cginc"

        struct Input
        {
            float2 uv;
            float3 objectSpacePos;
            float3 tangent_input;
        };

        sampler2D _MainTex;
        fixed4 _MainTex_ST;
        half _Glossiness;
        half _Specular;
        fixed4 _Color;

        // Specular
        float _SpecularFalloff;

         void vert(inout appdata_full v, out Input o)
        {
            UNITY_INITIALIZE_OUTPUT(Input, o);

            o.uv = TRANSFORM_TEX(v.texcoord.xy, _MainTex);

             // Compute normalized object-space position for local gradients
            o.objectSpacePos = normalize(v.vertex.xyz)  * 0.5 + 0.5;

            // Get tangent
            float4 p_tangent = mul(unity_ObjectToWorld, v.tangent);
            o.tangent_input = normalize(p_tangent.xyz);
        }
    
    ENDCG

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Disco fullforwardshadows vertex:vert
        #pragma shader_feature SHADOWED_RIM
        #pragma shader_feature _CAMERA_DITHER
        #pragma shader_feature _GLOBAL_BRIGHTNESS

        #pragma target 4.5

        float3 objectSpacePos;
        float3 tangent;

        void surf (Input i, inout SurfaceOutputStandardSpecular o)
        {
            fixed4 c = tex2D (_MainTex, i.uv) * _Color;
            o.Albedo = c.rgb;
            //o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
            o.Specular = _Specular;

            // Used for custom lighting
            objectSpacePos = i.objectSpacePos;
            tangent = i.tangent_input;
        }

        // Lighting
        inline fixed4 LightingDisco(SurfaceOutputStandardSpecular s, fixed3 viewDir, UnityGI gi)
        {
            // PBR
            fixed4 pbr = LightingStandardSpecular(s, viewDir, gi);

            // Lighting variables
            float3 L = gi.light.dir;
            float3 V = viewDir;
            float3 N = s.Normal;

            // Fresnel
            float3 fresnel = FresnelColor(N, V) * objectSpacePos;

            // Specular 2
            float3 R = reflect(normalize(L), N);
            float vDotRefl = dot(V, -R);
            float3 specular = _SpecularColor.rgb * 
                smoothstep(1 - s.Smoothness - _SpecularFalloff * 0.5, 1 - s.Smoothness + _SpecularFalloff * 0.5, vDotRefl);
            specular = pow(specular, _SpecularPower) * _SpecularStrength * _LightColor0;

            // Attenuation
            half3 attenRGB = gi.light.color / max(_LightColor0.rgb, 0.0001);
            half atten = max(attenRGB.r, max(attenRGB.g, attenRGB.b));

            // Rim/Fresnel
        #ifdef SHADOWED_RIM
                pbr.rgb = (pbr + fresnel) * atten;
        #else
                pbr.rgb = pbr + max(fresnel, specular);
        #endif

            // Diffraction/Iridescence
            pbr.rgb += Iridescence(L, V, tangent);
            
            return pbr;
        }

        void LightingDisco_GI(SurfaceOutputStandardSpecular s, UnityGIInput data, inout UnityGI gi) {
            Unity_GlossyEnvironmentData g = UnityGlossyEnvironmentSetup(s.Smoothness, data.worldViewDir, s.Normal, s.Specular);
            gi = UnityGlobalIllumination(data, s.Occlusion, s.Normal, g);
        }

        ENDCG
    }
    FallBack "Diffuse"
}
