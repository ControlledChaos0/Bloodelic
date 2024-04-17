using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEditor.Rendering.PostProcessing;

[Serializable]
[PostProcess(typeof(ExternalLightingPassRenderer), PostProcessEvent.BeforeStack, "Custom/ExternalLighting")]
public sealed class ExternalLightingPass : PostProcessEffectSettings
{
    public static string ShaderName = "Hidden/Custom/ExternalLighting";
}

// Formatting and general stylistics from:
// ... https://github.com/keijiro/DepthInverseProjection/blob/6b6d72d8178dbecdc29b037a175b95ea364bdbaa/Assets/InverseProjection/InverseProjection.cs
public sealed class ExternalLightingPassRenderer : PostProcessEffectRenderer<ExternalLightingPass>
{
    static class ShaderIDs
    {
        internal static readonly int InverseView = Shader.PropertyToID("_InverseView");
        internal static readonly int CameraOrthoDirection = Shader.PropertyToID("_CameraOrthoDirection");
        internal static readonly int LightInfo = Shader.PropertyToID("_LightIntercepterData");
        internal static readonly int LightMatrix = Shader.PropertyToID("_LightMatrix");
        internal static readonly int ExternalLightCaptureHD = Shader.PropertyToID("_ExternalLightCaptureHD"); // there was originally a LD as well
        internal static readonly int Cookie = Shader.PropertyToID("_Cookie");
    }

    public override void Render(PostProcessRenderContext context)
    {
        var command = context.command;
        command.BeginSample("External Light Pass");

        var sheet = context.propertySheets.Get(Shader.Find(ExternalLightingPass.ShaderName));

        // if (ExternalLightInterceptor.ShadowMap)
        // {
        //     sheet.properties.SetTexture(ShaderIDs.ShadowTex, ExternalLightInterceptor.ShadowMap);
        // }

        sheet.properties.SetMatrix(ShaderIDs.InverseView, context.camera.cameraToWorldMatrix);
        // this can probably be extracted form the view matrix but i'm getting lazy lol
        sheet.properties.SetVector(ShaderIDs.CameraOrthoDirection, context.camera.transform.forward);
        sheet.properties.SetMatrix(ShaderIDs.LightInfo, ExternalLightInterceptor.LightInfo);
        sheet.properties.SetMatrix(ShaderIDs.LightMatrix, ExternalLightInterceptor.LightMatrix);
        // sheet.properties.SetVector("_ShadowTexScale", ExternalLightInterceptor.TextureScale);
        sheet.properties.SetTexture(ShaderIDs.ExternalLightCaptureHD, ExternalLightInterceptor.DepthBufferStandin);
        sheet.properties.SetTexture(ShaderIDs.Cookie, ExternalLightInterceptor.Cookie);

        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);

        command.EndSample("External Light Pass");
    }

    public override DepthTextureMode GetCameraFlags()
    {
        return DepthTextureMode.DepthNormals;
    }
}

[Serializable]
[PostProcess(typeof(VHSRenderer), PostProcessEvent.BeforeStack, "Custom/VHS")]
public sealed class VHSPass : PostProcessEffectSettings
{
    public static string ShaderName = "Hidden/Custom/VHS";
    
    [Range(0, 4)]
    public FloatParameter VignetteIntensity = new FloatParameter(){ value = 2.0f };
    [Range(1, 10)]
    public FloatParameter StripesDensity = new FloatParameter(){ value = 2.0f };
    [Range(1, 15)]
    public FloatParameter TrackingSpeed = new FloatParameter(){ value = 2.0f };
    [Range(0, 6)]
    public FloatParameter RedBlueoffset = new FloatParameter(){ value = 2.0f };
    
    
    // Properties
    // {
    //     _MainTex ("Texture", 2D) = "white" {}
    //     _VignetteIntensity("Vignette Intensity", Range(0.0, 4.0)) = 2.0
    //     //_contrast("Contrast Intensity", Range(1.0, 1.2)) = 1.0
    //     _StripesDensity("Stripes Density", Range(1.0, 10.0)) = 5.0
    //     _TrackingSpeed("Tracking Speed", Range(1.0, 15.0)) = 7.0
    //     _RedBlueOffset("Red Blue Offset", Range(0.0, 6.0)) = 3.0
    // }
}

// Formatting and general stylistics from:
// ... https://github.com/keijiro/DepthInverseProjection/blob/6b6d72d8178dbecdc29b037a175b95ea364bdbaa/Assets/InverseProjection/InverseProjection.cs
public sealed class VHSRenderer : PostProcessEffectRenderer<VHSPass>
{
    static class ShaderIDs
    {
        internal static readonly int VignetteIntensity = Shader.PropertyToID("_VignetteIntensity");
        internal static readonly int StripesDensity = Shader.PropertyToID("_StripesDensity");
        internal static readonly int TrackingSpeed = Shader.PropertyToID("_TrackingSpeed");
        internal static readonly int RedBlueoffset = Shader.PropertyToID("_RedBlueOffset");
    }
    
    public override void Render(PostProcessRenderContext context)
    {
        var command = context.command;
        command.BeginSample("VHS Pass");

        var sheet = context.propertySheets.Get(Shader.Find(VHSPass.ShaderName));

        sheet.properties.SetFloat(ShaderIDs.VignetteIntensity, settings.VignetteIntensity);
        sheet.properties.SetFloat(ShaderIDs.StripesDensity, settings.StripesDensity);
        sheet.properties.SetFloat(ShaderIDs.TrackingSpeed, settings.TrackingSpeed);
        sheet.properties.SetFloat(ShaderIDs.RedBlueoffset, settings.RedBlueoffset);

        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);

        command.EndSample("VHS Pass");
    }

    public override DepthTextureMode GetCameraFlags()
    {
        return DepthTextureMode.None;
    }
}

// https://docs.unity3d.com/Packages/com.unity.postprocessing@3.0/manual/Writing-Custom-Effects.html
[PostProcessEditor(typeof(VHSPass))]
public sealed class VHSPassEditor : PostProcessEffectEditor<VHSPass>
{
    SerializedParameterOverride m_VI;
    SerializedParameterOverride m_SD;
    SerializedParameterOverride m_TS;
    SerializedParameterOverride m_RBO;

    public override void OnEnable()
    {
        m_VI = FindParameterOverride(x => x.VignetteIntensity);
        m_SD = FindParameterOverride(x => x.StripesDensity);
        m_TS = FindParameterOverride(x => x.TrackingSpeed);
        m_RBO = FindParameterOverride(x => x.RedBlueoffset);
    }

    public override void OnInspectorGUI()
    {
        PropertyField(m_VI);
        PropertyField(m_SD);
        PropertyField(m_TS);
        PropertyField(m_RBO);
    }
}