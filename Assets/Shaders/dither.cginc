
void BloodelicDither_float(float4 objPosition, float4 camPosition, float4 pixelPosition, float4 packedData, float2 uv, out float dither) {
    const float ditherNearDistance = packedData.x;
    const float ditherFarDistance = packedData.y;
    const float minDitherFraction = packedData.z;
    const float zoomFactor = packedData.w;

    dither = packedData.y;

    const float3 c2o = camPosition.xyz - objPosition.xyz;
    const float origLen = length(c2o);

    float len = origLen;

    if (len < ditherNearDistance) {
        dither = minDitherFraction;
    } else if (len < ditherFarDistance) {
        dither = (len - ditherNearDistance) / (ditherFarDistance - ditherNearDistance);
    } else {
        dither = 1;
    }

}