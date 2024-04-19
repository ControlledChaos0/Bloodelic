Shader "Anthony/Dance Floor"
{
    Properties
    {
        [Header(Tiles)] [Space]
        _Tiles ("Tiles", Vector) = (8, 8, 0, 0)
        _NoiseTex ("Noise Texture", 2D) = "white" {}
        
        [Header(Colors)] [Space]
        _ColorBoost ("Color Boost", Float) = 2
        [NoScaleOffset] _GradientTex ("Gradient", 2D) = "white" {}
        _ColorSpeed ("Color Speed", Vector) = (0.1, 0.1, 0, 0)
        
        [Header(Mask)] [Space]
        [NoScaleOffset] _MainTex ("Albedo (RGB)", 2D) = "white" {}
        
        [Header(PBR)] [Space]
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows
        #pragma target 3.5

        struct Input
        {
            float2 uv_MainTex;
        };
        sampler2D _MainTex;
        half _Glossiness;
        half _Metallic;
        fixed _Color;

        fixed4 _Tiles;
        sampler2D _NoiseTex;
        fixed4 _NoiseTex_ST;
        fixed _ColorBoost;

        sampler2D _GradientTex;
        fixed4 _ColorSpeed;

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Tiles
            float2 uv = IN.uv_MainTex;
            uv *= _Tiles.xy;
            uv = floor(uv);
            uv /= _Tiles.xy;

            // Noise
            float2 offset = 0;
            offset.x = _ColorSpeed.x * _SinTime.y;
            offset.y = _ColorSpeed.y * _CosTime.y;
            float noise = tex2D(_NoiseTex, uv * _NoiseTex_ST.xy + offset).r;

            // Color
            float3 color = tex2D(_GradientTex, float2(noise, 0));
            
            // Mask
            float mask = tex2D(_MainTex, IN.uv_MainTex * _Tiles.xy);
            
            o.Emission = color * mask * _ColorBoost;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = 1;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
