#include "Lighting.cginc"
#include "UnityPBSLighting.cginc"

// Sourced from https://www.alanzucconi.com/2017/07/15/improving-the-rainbow-2/
//  For iridescence/diffraction
#include "spectral.cginc"

// Anthony's lighting library
//  some commonly used lighting functions and models

// ---------------------------------------------------
// Lambert
// ---------------------------------------------------
inline fixed3 Lambert(float3 N, float3 L, float atten = 1)
{
    float3 lambert = saturate(dot(N, L));
    return lambert * atten * _LightColor0.xyz;
}

// ---------------------------------------------------
// SSS (Subsurface Scattering)
// ---------------------------------------------------
fixed4 _SSSAmbient;
float _SSSDistortion;
float _SSSScale;
float _SSSPower;
float _SSSAttenuation;
float _SSSThickness;
sampler2D _SSSThicknessMap;
uniform float thickness;

float3 SSS(float3 N, float3 L, float3 V)
{
    float3 H = normalize(L + N * _SSSDistortion);
    float VdotH = pow(saturate(dot(V, -H)), _SSSPower) * _SSSScale;
    #ifndef SSS_THICKNESS_MAP
        thickness = _SSSThickness;
    #endif
    float3 I = _SSSAttenuation * (VdotH + _SSSAmbient) * thickness;
    
    return I;
}

// Lighting function
inline fixed4 LightingSSS(SurfaceOutputStandard s, fixed3 viewDir, UnityGI gi)
{
    // PBR
    fixed4 pbr = LightingStandard(s, viewDir, gi);

    float3 L = gi.light.dir;
    float3 V = viewDir;
    float3 N = s.Normal;

    // Translucent lighting
    float3 sss = gi.light.color * SSS(N, L, V);

    pbr.rgb = saturate(pbr.rgb + sss); // +saturate(max(fresnel, specular)));

    // Attenuation
    half3 attenRGB = gi.light.color / max(_LightColor0.rgb, 0.0001);
    half atten = max(attenRGB.r, max(attenRGB.g, attenRGB.b));
    
    pbr.rgb *= atten;
    
    return pbr;
}

void LightingSSS_GI(SurfaceOutputStandard s, UnityGIInput data, inout UnityGI gi)
{
    LightingStandard_GI(s, data, gi);
}

// ---------------------------------------------------
// Specular Blinn Phong
// ---------------------------------------------------
fixed4 _SpecularColor;
float _SpecularPower;
float _SpecularStrength;

float3 Specular(float3 N, float3 L, float3 V)
{
            // Blinn-Phong
    float3 H = normalize(V + L);
    float NdotH = max(0, dot(N, H));
    float specular = pow(NdotH, _SpecularPower) * _SpecularStrength;
    return specular * _SpecularColor;
}

// ---------------------------------------------------
// Fresnel
// Fresnel equation: I = (1 - N dot V)^power * strength
// ---------------------------------------------------
fixed4 _FresnelColor;
float _FresnelPower, _FresnelStrength;

float Fresnel(float3 N, float3 V, bool inverted = false)
{
    float rim = 1.0 - saturate(dot(N, V));
    rim = saturate(pow(rim, _FresnelPower)) * _FresnelStrength;
    rim = max(rim, 0); // No negatives
    
    if (inverted)
    {
        return (1 - rim) * _FresnelColor;
    }
    
    return rim;
}

float3 FresnelColor(float3 N, float3 V, bool inverted = false)
{
    return Fresnel(N, V, inverted) * _FresnelColor;
}

//------------------------------------------------------
// Iridescence
//------------------------------------------------------
// Diffraction
float _DiffDistance;
int _DiffOrder;
fixed _DiffAlpha;

float3 Iridescence(float3 lightDir, float3 viewDir, float3 tangent)
{
    float3 iri = 0;
    
    float3 L = lightDir;
    float3 V = viewDir;
    float T = tangent;
    float d = _DiffDistance;

    fixed cos_ThetaL = dot(L, T);
    fixed cos_ThetaV = dot(V, T);
    fixed u = abs(cos_ThetaL - cos_ThetaV);

    if (u == 0)
        return 0;

    // Diffraction
    for (int i = 1; i <= _DiffOrder; i++)
    {
        float w = u * d / i;
        iri += spectral_zucconi6(w);
    }
    iri = saturate(iri) * _DiffAlpha;
    return iri;
}