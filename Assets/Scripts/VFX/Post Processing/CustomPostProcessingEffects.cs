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
            internal static readonly int ShadowTex = Shader.PropertyToID("_ShadowTex");
        }

        public override void Render(PostProcessRenderContext context)
        {
            var command = context.command;
            command.BeginSample("External Light Pass");

            var sheet = context.propertySheets.Get(Shader.Find(ExternalLightingPass.ShaderName));

            if (ExternalLightInterceptor.ShadowMap)
            {
                sheet.properties.SetTexture(ShaderIDs.ShadowTex, ExternalLightInterceptor.ShadowMap);
            }

            sheet.properties.SetMatrix(ShaderIDs.InverseView, context.camera.cameraToWorldMatrix);
            // Debug.Log(context.camera.cameraToWorldMatrix);
            
            context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);

            command.EndSample("External Light Pass");
        }
    }
}