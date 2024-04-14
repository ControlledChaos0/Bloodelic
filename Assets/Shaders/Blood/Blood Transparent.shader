Shader "Anthony/Blood Transparent"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        [HDR] _SpecularColor ("Specular Color", Color) = (0, 0, 0, 0)
    }
    SubShader
    {
        Tags 
        { 
            "RenderType" = "Transparent"
            "IgnoreProjector" = "True"
            "Queue" = "Transparent" 
        }
        LOD 200

        Blend SrcAlpha One
        ZWrite Off

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf StandardSpecular fullforwardshadows alpha vertex:vert
        #pragma target 4.5

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
            float4 color;
        };

        half _Glossiness;
        fixed4 _SpecularColor;
        fixed4 _Color;

        void vert(inout appdata_full v, out Input o)
        {
            UNITY_INITIALIZE_OUTPUT(Input, o);
            o.color = v.color;
        }

        void surf (Input IN, inout SurfaceOutputStandardSpecular o)
        {
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * IN.color;
            o.Albedo = c.rgb;
            // Metallic and smoothness come from slider variables
            o.Specular = _SpecularColor * c.r;
            o.Smoothness = _Glossiness;
            o.Alpha = c.r;
        }
        ENDCG
    }
    FallBack "Transparent/VertexLit"
}
