using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[ExecuteAlways, RequireComponent(typeof(Light)), RequireComponent(typeof(Camera))]
// Modified from: https://shahriyarshahrabi.medium.com/custom-shadow-mapping-in-unity-c42a81e1bbf8
// Apparently there's an even older hack without the blit https://interplayoflight.wordpress.com/2015/07/03/adventures-in-postprocessing-with-unity/
// Slightly simpler example: https://github.com/Gaxil/Unity-InteriorMapping/blob/master/Assets/Scripts/SetShadowMapAsGlobalTexture.cs, similar to the old hack
// https://forum.unity.com/threads/custom-shadow-mapping-to-mask-out-rain-effects-from-the-top-down.1444237/ ??????????????????????

public class ExternalLightInterceptor : MonoBehaviour
{
    public static Matrix4x4 LightInfo {
        get {
            if (instance == null || instance.lightReference == null) return Matrix4x4.zero;
            else {
                var transform = instance.transform;
                Vector3 position = transform.position;
                Vector3 forward = transform.forward;
                
                var light = instance.lightReference;
                Color color = light.color;
                
                // matrix constructor is column major, this agrees with HLSL convention
                return Matrix4x4.Transpose(new Matrix4x4(
                    new Vector4(position.x, position.y, position.z, 0),
                    new Vector4(forward.x, forward.y, forward.z, 0),
                    new Vector4(color.r, color.g, color.b, 0),
                    new Vector4(light.spotAngle, light.range, light.intensity, light.shadowStrength)
                ));
            }
        }
    }
    
    public static Matrix4x4 LightMatrix {
        get {
            if (instance == null || instance.lightReference == null) return Matrix4x4.zero;
            else {
                return instance.transform.worldToLocalMatrix;
            }
        }
    }

    public static Vector3 TextureScale
    {
        get
        {
            if (instance == null || instance.lightReference == null) return Vector4.zero;
            // https://github.com/gkjohnson/unity-custom-shadow-experiments/blob/30947d05326289fc91ac6e22557db2e42de35068/Assets/Scripts/CustomShadows.cs#L123
            Vector4 size = Vector4.zero;
            size.y = instance.cameraReference.orthographicSize * 2;
            size.x = instance.cameraReference.aspect * size.y;
            size.z = instance.cameraReference.farClipPlane;
            size.w = 1.0f / 1024;
            return size;
        }
    }

    public static Texture2D Cookie
    {
        get
        {
            if (instance == null || instance.lightReference == null) return Texture2D.blackTexture;
            return instance.cookie;
        }
    }

    public static RenderTexture DepthBufferStandin
    {
        get
        {
            return instance.cameraDeposit;
        }
    }

    private static ExternalLightInterceptor instance;
    
    // public just for seeing in inspector...
    public RenderTexture cameraDeposit;

    private Light lightReference;
    private Camera cameraReference;

    public Shader depthToColorShader;

    public Texture2D cookie;

    void OnEnable()
    {
        instance = this;
        lightReference = GetComponent<Light>();
        cameraReference = GetComponent<Camera>();
        
        // https://forum.unity.com/threads/find-worldpos-from-light-depth-texture.869572/
        // format issue corrected by the goat BGolus
        if (cameraDeposit == null) cameraDeposit = new RenderTexture(1024, 1024, 24, RenderTextureFormat.RGFloat);
        cameraDeposit.filterMode = FilterMode.Point;

        cameraReference.targetTexture = cameraDeposit;
        cameraReference.depthTextureMode = DepthTextureMode.Depth;

        depthToColorShader = Shader.Find("Hidden/CustomShadows/Depth");
    }

    private void Update()
    {
        cameraReference.RenderWithShader(depthToColorShader, "");
    }

    private void OnDisable()
    {
        instance = null;
        cameraDeposit.Release();
        cameraDeposit = null;
    }
}
