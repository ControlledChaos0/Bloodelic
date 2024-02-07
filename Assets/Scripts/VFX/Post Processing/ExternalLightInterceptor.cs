using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[ExecuteAlways]
// Modified from: https://shahriyarshahrabi.medium.com/custom-shadow-mapping-in-unity-c42a81e1bbf8
public class ExternalLightInterceptor : MonoBehaviour
{
    public static RenderTexture ShadowMap
    {
        get
        {
            return instance?.shadowMap;
        }
    }

    private static ExternalLightInterceptor instance;
    private CommandBuffer cb;
    
    private RenderTexture shadowMap;
    
    void OnEnable()
    {
        instance = this;
        
        cb = new CommandBuffer
        {
            name = "ExternalLightCapture"
        };

        RenderTargetIdentifier shadowmap = BuiltinRenderTextureType.CurrentActive;
        shadowMap = new RenderTexture(Camera.main.pixelWidth, Camera.main.pixelHeight, 16, RenderTextureFormat.ARGB32);
        shadowMap.filterMode = FilterMode.Point;

        // Change shadow sampling mode for m_Light's shadowmap.
        cb.SetShadowSamplingMode(shadowmap, ShadowSamplingMode.RawDepth);

        // The shadowmap values can now be sampled normally - copy it to a different render texture.
        var id = new RenderTargetIdentifier(shadowMap);
        cb.Blit(shadowmap, id);
      
        // cb.SetGlobalTexture("_ExternalLightCapture", id);

        Light m_Light = this.GetComponent<Light>();
        // Execute after the shadowmap has been filled.
        m_Light.AddCommandBuffer(LightEvent.AfterShadowMap, cb);
    }
}
