using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace CustomPostProcessingEffects {
    [Serializable]
    [PostProcess(typeof(ExternalLightingPassRenderer), PostProcessEvent.BeforeTransparent, "Custom/ExternalLighting")]
    public sealed class ExternalLightingPass : PostProcessEffectSettings
    {
        public static string ShaderName = "Custom/ExternalLighting";
    }

    public sealed class ExternalLightingPassRenderer : PostProcessEffectRenderer<ExternalLightingPass>
    {
        private static Shader _shader = Shader.Find("Custom/VHSEffect");
        
        public override void Render(PostProcessRenderContext context)
        {
            var sheet = context.propertySheets.Get(settings.postProcessingShader);
            context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
        }
    }
}