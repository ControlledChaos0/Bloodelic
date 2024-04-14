// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Custom/Ditherable"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _AlphaClip ("Alpha Clip", Range(0, 1)) = 0.5
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows alphatest:_AlphaClip

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        #include "./dither.cginc"

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
            // https://docs.unity3d.com/Manual/SL-SurfaceShaders.html
            // float3 viewDir;
            float3 worldPos;
            float4 screenPos;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
        float4x4 _BloodelicDitheringDataPacked;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            float dither1, dither2;

            float4 objectOrigin = mul(unity_ObjectToWorld, float4(0.0,0.0,0.0,1.0) );

            // call shader node code directly
            // i love Unity shader lab <3
            CustomDither_float(IN.worldPos, IN.screenPos * _ScreenParams.xy, _BloodelicDitheringDataPacked, dither1);
            CustomDither_float(objectOrigin, IN.screenPos * _ScreenParams.xy, _BloodelicDitheringDataPacked, dither2);

            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = min(dither1, dither2);
        }
        ENDCG
    }
    FallBack "Diffuse"
}
