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

#region Shell Renderer Static Parameters
    [Header("Shell Renderer Static Parameters")]

    [SerializeField, Tooltip("Update stylization parameters per frame; DO NOT SET IN SHIPPED GAME!")]
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

    [SerializeField, SaveDuringPlay]
    private Texture2D spikeHeightMap;

    [SerializeField, SaveDuringPlay, Range(0.0f, 1.0f)]
    private float spikeHeightMapCutoff;

    [SerializeField, SaveDuringPlay, Range(0.01f, 3.0f)]
    private float distanceAttenuation = 1.0f;

    //[SerializeField, SaveDuringPlay, Range(0.0f, 10.0f)]
    //private float thickness = 1.0f;

    [SerializeField, SaveDuringPlay, Range(0.0f, 0.01f)]
    private float shellDroop = 1.0f;

    [SerializeField, SaveDuringPlay, Range(0.0f, 100.0f)]
    private float shellSpecularSharpness;

    [SerializeField, SaveDuringPlay, Range(0.0f, 5.0f)]
    private float shellSpecularAmount;

    [SerializeField, SaveDuringPlay]
    private Color bodyColor;

    [SerializeField, SaveDuringPlay]
    private Color spikeTipColor;

    [SerializeField, SaveDuringPlay, Range(0.1f, 10.0f)]
    private float animationSpeed;

    #endregion

#region Shell Renderer Dynamic Parameters
    [Header("Shell Renderer Dynamic Parameters")]

    private float animationTime = 0.0f;

    public bool viewShells = true;
#endregion

#region Shell Animation Parameters
    [Header("Shell Animation Parameters")]

    [SerializeField, SaveDuringPlay, Range(0.1f, 300.0f)]
    private float positionResponsiveness;

    [SerializeField, SaveDuringPlay, Range(0.0f, 0.25f), Tooltip("Smaller = stiffer swinging animation")]
    private float positionMaxDelta;

    [SerializeField, SaveDuringPlay, Range(0.1f, 300.0f)]
    private float rotationResponsiveness;
    
    private TransformAccessArray shellTransformAccesses; // NativeArray that optimizes Transform object access, see animation job definition below
    #endregion

    private Material shellMaterial;
    private Transform[] shellTransforms;
    private MeshRenderer[] shellRenderers;
    private JobHandle currentAnimationJob;
    private JobHandle currentBufferSyncJob;

    NativeArray<Vector3> shellPositions;
    NativeArray<Quaternion> shellRotations;

    void OnEnable() {
        shellMaterial = new Material(shellMaterialPrototype);
        shellTransforms = new Transform[shellCount];
        shellRenderers = new MeshRenderer[shellCount];

        for (int i = 0; i < shellCount; ++i) {
            GameObject currShell = new("__shell_temp_obj_" + i.ToString(), typeof(MeshFilter), typeof(MeshRenderer));
            shellTransforms[i] = currShell.transform;

            currShell.transform.position = bodyMesh.position;
            currShell.transform.rotation = bodyMesh.rotation;

            currShell.GetComponent<MeshFilter>().mesh = bodyMesh.GetComponent<MeshFilter>().mesh;

            MeshRenderer currRend = currShell.GetComponent<MeshRenderer>();
            currRend.material = shellMaterial;
            currRend.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

            shellRenderers[i] = currRend;
        }

        shellTransformAccesses = new TransformAccessArray(shellTransforms);

        // memory management taken from https://catlikecoding.com/unity/tutorials/basics/jobs/
        shellPositions = new NativeArray<Vector3>(shellCount, Allocator.Persistent);
        shellRotations = new NativeArray<Quaternion>(shellCount, Allocator.Persistent);

        UpdateStaticParameters();
    }

    void FixedUpdate() { // AT: optionally use Update instead
        if (updateStatics) { 
            UpdateStaticParameters();
        }
        UpdateDynamicParameters(Time.deltaTime);
    }

    private void Update() {
        SyncAnimationBufferJob fetchTransforms = new()
        {
            anchorPos = bodyMesh.transform.position,
            anchorRotation = bodyMesh.transform.rotation,
            Count = shellCount,
            lastPosition = shellPositions,
            lastRotation = shellRotations,
        };

        ShellAnimationJob anim = new()
        {
            lastPosition = shellPositions,
            lastRotation = shellRotations,
            positionLerpFactor = positionResponsiveness * Time.deltaTime,
            rotationLerpFactor = rotationResponsiveness * Time.deltaTime,
            masDistSquared = positionMaxDelta * positionMaxDelta,
        };

        currentBufferSyncJob = fetchTransforms.Schedule(shellTransformAccesses);
        currentAnimationJob = anim.Schedule(shellTransformAccesses, currentBufferSyncJob);
    }

    private void LateUpdate()
    {
        currentAnimationJob.Complete(); // give it enough time to complete
    }

    void OnDisable() {
        shellTransformAccesses.Dispose();

        // AT: technically not needed as shells are not saved

        //for (int i = 0; i < shellTransforms.Length; ++i)
        //{
        //    Destroy(shellRenderers[i].gameObject);
        //}
        //shellTransforms = null;
        //shellRenderers = null;
    }

    void UpdateStaticParameters()
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
                .SetFloatParam("_ShellDroop", shellDroop)
                //.SetFloatParam("_OcclusionBias", occlusionBias)
                //.SetFloatParam("_NoiseMin", noiseMin)
                //.SetFloatParam("_NoiseMax", noiseMax)
                .SetFloatParam("_SpikeDensity", spikeDensity)
                .SetFloatParam("_SpikeCutoffMin", spikeCutoffMin)
                .SetFloatParam("_SpikeCutoffMax", spikeCutoffMax)
                .SetFloatParam("_SpikeHeightMapCutoff", spikeHeightMapCutoff)
                .SetFloatParam("_SpikeShapeStylizationFactor", spikeShapeStylizationFactor)
                .SetFloatParam("_ShellSpecularSharpness", shellSpecularSharpness)
                .SetFloatParam("_ShellSpecularAmount", shellSpecularAmount)
                .SetFloatParam("_SpikeShadowSmoothnessFactor", spikeShadowSmoothnessFactor)
                .SetVectorParam("_BodyColor", bodyColor)
                .SetVectorParam("_SpikeTipColor", spikeTipColor)
                .SetTextureParam("_SpikeHeightMap", spikeHeightMap);
        });
    }

    void UpdateDynamicParameters(float deltaTime)
    {
        animationTime += (Mathf.Sin(Time.time) + 2) / 3 * deltaTime * animationSpeed;

        if (animationTime > Mathf.PI * 2)
        {
            animationTime = 0;
        }

        ForEachShellDo((Transform shellTransform, MeshRenderer shellRenderer, int idx) =>
        {
            shellRenderer.material
                .SetFloatParam("_AnimationTime", animationTime);

            shellRenderer.enabled = viewShells;
        });
    }

    struct ShellAnimationJob : IJobParallelForTransform // https://realerichu.medium.com/improve-performance-with-c-job-system-and-burst-compiler-in-unity-eecd2a69dbc8
    {
        [ReadOnly] public NativeArray<Vector3> lastPosition;
        [ReadOnly] public NativeArray<Quaternion> lastRotation;
        [ReadOnly] public float positionLerpFactor;
        [ReadOnly] public float rotationLerpFactor;
        [ReadOnly] public float masDistSquared;

        public void Execute(int index, TransformAccess transform)
        {
            int prev = index - 1;
            if (prev < 0) prev = 0;

            Vector3 thisToLast = (lastPosition[prev] - transform.localPosition);
            float sqrDist = thisToLast.sqrMagnitude;

            if (sqrDist > masDistSquared)
            {
                transform.localPosition = lastPosition[prev] - thisToLast.normalized * masDistSquared;
            } else
            {
                transform.localPosition = (1 - positionLerpFactor) * transform.localPosition + positionLerpFactor * lastPosition[prev];
            }

            transform.localRotation = Quaternion.SlerpUnclamped(transform.localRotation, lastRotation[prev], rotationLerpFactor);
        }
    }
    
    struct SyncAnimationBufferJob : IJobParallelForTransform
    {
        [ReadOnly] public Vector3 anchorPos;
        [ReadOnly] public Quaternion anchorRotation;
        [ReadOnly] public int Count;
        [WriteOnly, NativeDisableParallelForRestriction] public NativeArray<Vector3> lastPosition;
        [WriteOnly, NativeDisableParallelForRestriction] public NativeArray<Quaternion> lastRotation;

        public void Execute(int index, TransformAccess transform)
        {
            int Idx = (index + 1) % Count;

            if (Idx == 0)
            {
                lastPosition[Idx] = anchorPos;
                lastRotation[Idx] = anchorRotation;
            }
            else
            {
                lastPosition[Idx] = transform.position;
                lastRotation[Idx] = transform.rotation;
            }
        }
    }

    void ForEachShellDo(Action<Transform, MeshRenderer, int> func)
    {
        for (int i = 0; i < shellCount; ++i)
            func(shellTransforms[i], shellRenderers[i], i);
    }
}
