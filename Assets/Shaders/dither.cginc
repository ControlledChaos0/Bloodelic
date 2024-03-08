#ifndef __DITHER_CGINC
#define __DITHER_CGINC

float Smooth(float x) {
    float temp = -2 * x + 2;
    return x < 0.5 ? (2 * x * x) : (1 - temp * temp / 2);
}

void CustomDither_float(float3 objPosition, float2 pixelPosition, float4x4 packedData, out float dither) {

    const float ditherNearDistance = packedData[0].x;
    const float ditherFarDistance = packedData[0].y;
    const float minDitherFraction = packedData[0].z;
    const float zoomFactor = packedData[1].x;
    const float zoomStrength = packedData[1].y;
    const float zoomSensitivity = packedData[1].z;
    const float3 camPosition = packedData[2].xyz;
    const float3 camDir = packedData[3].xyz;

    const float3 c2o = camPosition.xyz - objPosition.xyz;
    const float3 distToCamPlane = dot(c2o, -camDir.xyz);

    const float dither_weights[] = { 
        0, 32, 8, 40, 2, 34, 10, 42,
        48, 16, 56, 24, 50, 18, 58, 26,
        12, 44, 4, 36, 14, 46, 6, 38,
        60, 28, 52, 20, 62, 30, 54, 22,
        3, 35, 11, 43, 1, 33, 9, 41,
        51, 19, 59, 27, 49, 17, 57, 25,
        15, 47, 7, 39, 13, 45, 5, 37,
        63, 31, 55, 23, 61, 29, 53, 21
    };

#define DITHERIDX(a, b) ((trunc(a) % 8) * 8 + (trunc(b) % 8))

    // pi/2
    float len = distToCamPlane + tan(1.5707963 * zoomSensitivity * (min(zoomFactor, 1) - 1)) * zoomStrength;
    
    dither = saturate((clamp(len, ditherNearDistance, ditherFarDistance) - ditherNearDistance) / (ditherFarDistance - ditherNearDistance));
    dither = Smooth(dither) * (1 - minDitherFraction) + minDitherFraction;

    // dither = 1; // saturate(clamp(len, ditherNearDistance, ditherFarDistance) - ditherNearDistance);
    dither = dither > (dither_weights[DITHERIDX(pixelPosition.x, pixelPosition.y)] / 64);
}

#endif