Shader "Anthony/DiscoFloorQuick"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _EmissiveTex ("Emissive", 2D) = "white" {}
        _PulseRangeSpeed ("Pulse Range (XY), Speed (Z)", Vector) = (.25, 1, 1, 0)
        
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0


        struct Input
        {
            float2 uv_EmissiveTex;
        };

        sampler2D _EmissiveTex;
        float4 _PulseRangeSpeed;
        fixed4 _Color;
        half _Glossiness;
        half _Metallic;

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            float3 emissiveColor = tex2D(_EmissiveTex, IN.uv_EmissiveTex) * _Color;
            emissiveColor *= lerp(_PulseRangeSpeed.x, _PulseRangeSpeed.y, cos(_Time.y * _PulseRangeSpeed.z) * 0.5 + 0.5);
            o.Emission = emissiveColor;
            
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = 1;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
