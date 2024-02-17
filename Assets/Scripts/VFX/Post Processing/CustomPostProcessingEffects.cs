using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace CustomPostProcessingEffects {
    [Serializable]
    [PostProcess(typeof(ExternalLightingPassRenderer), PostProcessEvent.BeforeTransparent, "Custom/ExternalLighting")]
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
}