Shader "Anthony/TriplanarSurface"
{
    Properties
    {
        [Header(Albedo)]
        _TopColor ("Top Color", Color) = (1,1,1,1)
        [NoScaleOffset] _TopTex ("Top Texture", 2D) = "white" {}
        _TopTiling ("Top Scale Offset", Vector) = (1, 1, 0, 0)
        
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        
        [Header(Normals)]
        [Toggle(NORMALS)] UseNormals ("Normals?", Int) = 1
        _BumpTex ("Normal Tex", 2D) = "bump" {}
        _BumpStrength ("Normal Strength", Float) = 1
        _BumpOpacity ("Normal Opacity", Range(0, 1)) = 0.5
        
        [Header(Gradient)] [Space]
        [Toggle(GRADIENT)] _UseGradient ("Use Vertical Gradient?", Int) = 0
        _GradientTop ("Gradient Top Color", Color) = (1,1,1,1)
        _GradientBottom ("Gradient Bottom Color", Color) = (0, 0, 0, 1)
        _GradientHeightRange ("Gradient Height (Min, Max)", Vector) = (0, 5, 0, 0)
        
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows
        #pragma shader_feature GRADIENT
        #pragma shader_feature NORMALS
        
        #pragma target 4.0

        struct Input
        {
            float2 uv_MainTex;
            float3 worldPos;
            float3 worldNormal; INTERNAL_DATA
        };
        
        sampler2D _TopTex;
        fixed4 _TopColor;
        fixed4 _TopTiling;
        
        half _Glossiness;
        half _Metallic;

        float4 _GradientTop;
        float4 _GradientBottom;
        float4 _GradientHeightRange;

        sampler2D _BumpTex;
        float _BumpStrength;
        float _BumpOpacity;

        float inverselerp(float a, float b, float v) 
        {
	        return (v - a) / (b - a);
        }

        void surf (Input i, inout SurfaceOutputStandard o)
        {
            float2 topUV = i.worldPos.xz * _TopTiling.xy + _TopTiling.zw;
            float2 frontUV = i.worldPos.xy * _TopTiling.xy + _TopTiling.zw;
            float2 sideUV = i.worldPos.yz *  _TopTiling.xy + _TopTiling.zw;
            
            fixed4 topColor     = tex2D(_TopTex, topUV) * abs(i.worldNormal.y);
            fixed4 frontColor   = tex2D(_TopTex, frontUV) * abs(i.worldNormal.z);
            fixed4 sideColor    = tex2D(_TopTex, sideUV) * abs(i.worldNormal.x);

            fixed4 color = topColor + frontColor + sideColor;
            
            #ifdef GRADIENT
                float4 gradient =
                    lerp(_GradientBottom, _GradientTop, saturate(inverselerp(_GradientHeightRange.x, _GradientHeightRange.y, i.worldPos.y)));
                color *= gradient;
                    
            #else
                color *= _TopColor;
            #endif

            // Normals
            #ifdef NORMALS
            fixed3 N = WorldNormalVector(i, o.Normal);
            float3 blendNormal = saturate(pow(N * 1.4, 4));
            float3 worldNormal = pow(abs(blendNormal), 1);

            fixed3 topNormal = UnpackScaleNormal(tex2D(_BumpTex, topUV), _BumpStrength);
            fixed3 frontNormal = UnpackScaleNormal(tex2D(_BumpTex, frontUV), _BumpStrength);
            fixed3 sideNormal = UnpackScaleNormal(tex2D(_BumpTex, sideUV), _BumpStrength);
            fixed3 blendedNormals = topNormal * worldNormal.y + sideNormal * worldNormal.x + frontNormal * worldNormal.z;
            blendedNormals = normalize(blendedNormals);
            
            o.Normal = blendedNormals;
            #else
            o.Normal = WorldNormalVector(i, o.Normal);

            #endif
            
            o.Albedo = color;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = 1;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
