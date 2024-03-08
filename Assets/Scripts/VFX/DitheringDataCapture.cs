using UnityEngine;

public class DitheringDataCapture : MonoBehaviour
{
    [SerializeField, Tooltip("Distance when dithering reaches maximum (most transparent)"), Range(0, 50)]
    private float ditherNearDistance;
    [SerializeField, Tooltip("Distance when dithering starts kicking in (in world units)"), Range(0, 50)]
    private float ditherFarDistance;
    [SerializeField, Tooltip("Minimum transparency fraction for the dithering"), Range(0, 1)]
    private float minDitherFraction;
    
    [SerializeField, Tooltip("Distance added per unit of zooming (tan-scaled, see dither.cginc)"), Range(0, 50)]
    private float zoomStrength;

    [SerializeField, Tooltip("Lower = less zooming + more gradual; Higher = more zooming + increased zooming sensitivity when zoomed in / out greatly"), Range(0.2f, 1)]
    private float zoomSensitivity;
    
    private void Start()
    {
        RefreshGlobalShaderVariables();
    }

    void Update()
    {
        RefreshGlobalShaderVariables();
    }

    private static readonly int DithDatID = Shader.PropertyToID("_BloodelicDitheringDataPacked");

    void RefreshGlobalShaderVariables()
    {
        float zoom = CameraController.DistanceFrom / CameraController.DistanceFromInitial;
        var cpos = CameraController.Instance.CameraTransform.position;
        var cdir = CameraController.Instance.CameraTransform.forward;

        Shader.SetGlobalMatrix(DithDatID, Matrix4x4.Transpose(new Matrix4x4(
            new Vector4(ditherNearDistance, ditherFarDistance, minDitherFraction, 0),
            new Vector4(zoom, zoomStrength, zoomSensitivity, 0),
            new Vector4(cpos.x, cpos.y, cpos.z, 0),
            new Vector4(cdir.x, cdir.y, cdir.z, 0)
            )));
    }
}
