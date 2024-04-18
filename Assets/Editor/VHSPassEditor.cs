using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering.PostProcessing;
using UnityEngine;

// https://docs.unity3d.com/Packages/com.unity.postprocessing@3.0/manual/Writing-Custom-Effects.html
[PostProcessEditor(typeof(VHSPass))]
public class VHSPassEditor : PostProcessEffectEditor<VHSPass>
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