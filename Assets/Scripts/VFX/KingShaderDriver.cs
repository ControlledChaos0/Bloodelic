// Modified based on https://github.com/GarrettGunnell/Shell-Texturing/blob/main/Assets/SimpleShell.cs

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Cinemachine;
using Unity.Collections;

/* Burst jobs */
using Unity.Burst;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Jobs;

#pragma warning disable CS3009

public class KingShaderDriver : MonoBehaviour
{
    #region References
    [Header("References")]

    [SerializeField] private Transform bodyMesh;
    [SerializeField] private Material shellMaterialPrototype;
    #endregion

    #region Shell Renderer Parameters
    [Header("Shell Renderer Parameters")]

    [SerializeField, Tooltip("Update stylization parameters per frame")]
    private bool updateStatics = true;

    // TODO: SaveDuringPlay doesn't actually work???? :/

    // These variables and what they do are explained on the shader code side of things
    // You can see below (line 70) which shader uniforms match up with these variables
    [SerializeField, SaveDuringPlay, Range(1, 256)]
    private int shellCount = 16;

    [SerializeField, SaveDuringPlay, Range(0.0f, 1.0f)]
    private float shellLength = 0.15f;

    [SerializeField, SaveDuringPlay, Range(1, 50)]
    private float spikeDensity;

    [SerializeField, SaveDuringPlay, Range(0.0f, 0.5f), Tooltip("Higher value = skinnier spike base")]
    private float spikeCutoffMin;

    [SerializeField, SaveDuringPlay, Range(0.0f, 1.0f), Tooltip("Higher value = skinnier spike tip")]
    private float spikeCutoffMax;

    [SerializeField, SaveDuringPlay, Range(0.0f, 3.0f), Tooltip("Higher value = puffier spikes")]
    private float spikeShapeStylizationFactor;

    [SerializeField, SaveDuringPlay, Range(0.0f, 1.0f), Tooltip("Higher value = smoother lighting on spikes")]
    private float spikeShadowSmoothnessFactor;

    [SerializeField, SaveDuringPlay, Range(0.01f, 3.0f)]
    private float distanceAttenuation = 1.0f;

    //[SerializeField, SaveDuringPlay, Range(0.0f, 10.0f)]
    //private float thickness = 1.0f;

    [SerializeField, SaveDuringPlay, Range(0.0f, 10.0f)]
    private float curvature = 1.0f;

    [SerializeField, SaveDuringPlay, Range(0.0f, 100.0f)]
    private float shellSpecularSharpness;

    [SerializeField, SaveDuringPlay, Range(0.0f, 5.0f)]
    private float shellSpecularAmount;

    [SerializeField, SaveDuringPlay]
    private Color bodyColor;

    [SerializeField, SaveDuringPlay]
    private Color spikeTipColor;

    [SerializeField, SaveDuringPlay]
    private Texture2D spikeUvTex;

    [SerializeField, SaveDuringPlay, Range(0.1f, 10.0f)]
    private float animationSpeed;

    [SerializeField, SaveDuringPlay, Range(0.0f, 1.0f)]
    private float fuzzFixFactor;

    #endregion

    #region Shell Animation Parameters
    [Header("Shell Animation Parameters")]

    [SerializeField, SaveDuringPlay, Range(0.1f, 10.0f)]
    private float positionResponsiveness;

    [SerializeField, SaveDuringPlay, Range(0.1f, 10.0f)]
    private float rotationResponsiveness;
    
    private struct ShellAnimationState
    {
        float3 location;
        float4 rotation;
    }

    private NativeArray<ShellAnimationState> shellAnimationBuffer;
    private TransformAccessArray shellTransformAccesses; // NativeArray that optimizes Transform object access, see animation job definition below
    #endregion

    private Material shellMaterial;
    private Transform[] shellTransforms;
    private MeshRenderer[] shellRenderers;
    private float animationTime = 0.0f;

    void OnEnable() {

        shellMaterial = new Material(shellMaterialPrototype);
        shellTransforms = new Transform[shellCount];
        shellTransformAccesses = new TransformAccessArray(shellTransforms);
        shellRenderers = new MeshRenderer[shellCount];


        // memory management taken from https://catlikecoding.com/unity/tutorials/basics/jobs/
        shellAnimationBuffer = new NativeArray<ShellAnimationState>(shellCount, Allocator.Persistent);

        for (int i = 0; i < shellCount; ++i) {
            GameObject currShell = new("__shell_temp_obj_" + i.ToString(), typeof(MeshFilter), typeof(MeshRenderer));
            shellTransforms[i] = currShell.transform;

            currShell.GetComponent<MeshFilter>().mesh = bodyMesh.GetComponent<MeshFilter>().mesh;

            MeshRenderer currRend = currShell.GetComponent<MeshRenderer>();
            currRend.material = shellMaterial;
            shellRenderers[i] = currRend;
            currRend.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off; // body mesh should cast shadow, not the shells. hopefully saving some performance
        }

        UpdateStatics();
    }

    void FixedUpdate() { // AT: optionally use Update instead
        if (updateStatics) { 
            UpdateStatics();
        }
        UpdateDynamics(Time.deltaTime);
        SyncAllShellTransforms(Time.deltaTime);
    }

    void OnDisable() {
        shellAnimationBuffer.Dispose();
        shellTransformAccesses.Dispose();

        // AT: technically not needed as shells are not saved

        //for (int i = 0; i < shellTransforms.Length; ++i)
        //{
        //    Destroy(shellRenderers[i].gameObject);
        //}
        //shellTransforms = null;
        //shellRenderers = null;
    }

    void UpdateStatics()
    {
        ForEachShellDo((Transform shellTransform, MeshRenderer shellRenderer, int idx) =>
        {
            shellRenderer.material
                .SetIntParam("_ShellCount", shellCount)
                .SetIntParam("_ShellIndex", idx)
                .SetFloatParam("_ShellLength", shellLength)
                //.SetFloatParam("_Density", density)
                // .SetFloatParam("_Thickness", thickness)
                // .SetFloatParam("_Attenuation", occlusionAttenuation)
                .SetFloatParam("_ShellDistanceAttenuation", distanceAttenuation)
                .SetFloatParam("_Curvature", curvature)
                //.SetFloatParam("_OcclusionBias", occlusionBias)
                //.SetFloatParam("_NoiseMin", noiseMin)
                //.SetFloatParam("_NoiseMax", noiseMax)
                .SetFloatParam("_SpikeDensity", spikeDensity)
                .SetFloatParam("_SpikeCutoffMin", spikeCutoffMin)
                .SetFloatParam("_SpikeCutoffMax", spikeCutoffMax)
                .SetFloatParam("_SpikeShapeStylizationFactor", spikeShapeStylizationFactor)
                .SetFloatParam("_ShellSpecularSharpness", shellSpecularSharpness)
                .SetFloatParam("_ShellSpecularAmount", shellSpecularAmount)
                .SetFloatParam("_SpikeShadowSmoothnessFactor", spikeShadowSmoothnessFactor)
                .SetFloatParam("_FuzzFixFactor", fuzzFixFactor)
                .SetVectorParam("_BodyColor", bodyColor)
                .SetVectorParam("_SpikeTipColor", spikeTipColor)
                .SetTextureParam("_SpikeUv", spikeUvTex);
        });
    }

    void UpdateDynamics(float deltaTime)
    {
        animationTime += deltaTime * animationSpeed;

        if (animationTime > Mathf.PI * 2)
        {
            animationTime = 0;
        }

        ForEachShellDo((Transform shellTransform, MeshRenderer shellRenderer, int idx) =>
        {
            shellRenderer.material
                .SetFloatParam("_AnimationTime", animationTime);
        });
    }

    void SyncAllShellTransforms(float deltaTime)
    {
        ShellAnimationJob job = new()
        {
            // pass buffers to job
        };

        var scheduledJob = job.Schedule(shellTransformAccesses);
        scheduledJob.Complete(); // ... wait until
    }

    // defined here 
    struct ShellAnimationJob : IJobParallelForTransform // https://realerichu.medium.com/improve-performance-with-c-job-system-and-burst-compiler-in-unity-eecd2a69dbc8
    {
        public void Execute(int index, TransformAccess transform)
        {
            // TODO
        }
    }

    void ForEachShellDo(Action<Transform, MeshRenderer, int> func)
    {
        for (int i = 0; i < shellCount; ++i)
            func(shellTransforms[i], shellRenderers[i], i);
    }
}
