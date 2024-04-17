using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[ExecuteInEditMode]
public class ApplyVHSEffect : MonoBehaviour
{
    public Material vhsEffectMaterial; 

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (vhsEffectMaterial != null)
        {
            Graphics.Blit(src, dest, vhsEffectMaterial);
        }
        else
        {
            Graphics.Blit(src, dest);
        }
    }
}