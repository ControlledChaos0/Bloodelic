using UnityEngine;

public class DitheringDataCapture : MonoBehaviour
{
    [SerializeField]
    private bool updateStatics;

    [SerializeField, Tooltip("Distance when dithering reaches maximum (most transparent)")]
    private float ditherNearDistance;
    [SerializeField, Tooltip("Distance when dithering starts kicking in (in world units)")]
    private float ditherFarDistance;
    [SerializeField, Tooltip("Minimum transparency fraction for the dithering"), Range(0, 1)]
    private float minDitherFraction;
    
    private void Start()
    {
        RefreshGlobalShaderVariables();
    }

    void Update()
    {
        if (updateStatics)
        {
            RefreshGlobalShaderVariables();
        }
    }

    private static readonly int DithDatID = Shader.PropertyToID("_BloodelicDitheringDataPacked");

    void RefreshGlobalShaderVariables()
    {
        float zoom = CameraController.DistanceFrom / CameraController.DistanceFromInitial;
        
        Shader.SetGlobalVector(DithDatID, new Vector4(ditherNearDistance, ditherFarDistance, minDitherFraction, zoom));
    }
}
