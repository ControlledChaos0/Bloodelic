using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEditor;

public class CameraController : MonoBehaviour
{
    private static readonly object padlock = new();
    [SerializeField]
    private Camera mainCamera;
    [SerializeField]
    private float cameraSensitivity = 0.5f;
    [SerializeField]
    private float zoomSensitivity = 0.5f;
    [SerializeField]
    private float panSensitivity = 5f;
    [SerializeField]
    private float distanceFrom = 5.0f;

    private static CameraController _instance;
    private CinemachineBrain _cinemachineBrain;
    private CinemachineVirtualCamera _cinemachineCam;
    private RaycastHit _closestHit;
    private Transform _lookAt;
    private Vector3 _oldRot;
    private Vector3 _defaultPos;
    private Vector3 _currentPos;
    private Vector3 _panMove;
    private float _rotX;
    private float _rotY;
    private int _hitMask;

    public static CameraController CameraControllerInstance {
        get {
            lock (padlock) {
                if (_instance == null) {
                    _instance = new();
                }
                return _instance;
            }
        }
    }
    public Camera MainCamera {
        get => mainCamera;
        private set => mainCamera = value;
    }
    public RaycastHit ClosestHit {
        get => _closestHit;
        private set => _closestHit = value;
    }
    public int HitMask {
        get => _hitMask;
        set => _hitMask = value;
    }

    private void Awake() {
        _cinemachineBrain = mainCamera.GetComponent<CinemachineBrain>();
    }

    // Start is called before the first frame update
    void Start()
    {
        SetCamera();
    }

    private void LateUpdate() {
        _cinemachineCam.m_Lens.OrthographicSize = distanceFrom;

        _lookAt.rotation = Quaternion.Euler(_rotX, _rotY, 0);
        _lookAt.position = _currentPos;
    }
    private void OnEnable() {
        InputController.InputControllerInstance.click += ScreenClick;
        //InputController.InputControllerInstance.hover += Hover;
    }
    private void OnDisable() {
        InputController.InputControllerInstance.click -= ScreenClick;
        //InputController.InputControllerInstance.hover -= Hover;
        Clear();
    }

    private void SetCamera() {
        _cinemachineCam = _cinemachineBrain.ActiveVirtualCamera.VirtualCameraGameObject.GetComponent<CinemachineVirtualCamera>();
        _lookAt = _cinemachineCam.gameObject.transform.parent;

        _defaultPos = new Vector3(_lookAt.position.x, _lookAt.position.y, _lookAt.position.z);
        _currentPos = new Vector3(_lookAt.position.x, _lookAt.position.y, _lookAt.position.z);

        _oldRot = _cinemachineCam.VirtualCameraGameObject.transform.rotation.eulerAngles;
        _rotX = _cinemachineCam.VirtualCameraGameObject.transform.rotation.eulerAngles.x;
        _rotY = _cinemachineCam.VirtualCameraGameObject.transform.rotation.eulerAngles.y;
        distanceFrom = _cinemachineCam.m_Lens.OrthographicSize;
    }

    public void RotateCamera(Vector2 mouseDelta) {
        _rotX -= mouseDelta.y * cameraSensitivity;
        _rotY += mouseDelta.x * cameraSensitivity;
        _rotX = Mathf.Clamp(_rotX, 0f, 90f);
        //Debug.Log(_rotX);
    }

    public void PanCamera(Vector2 mouseDelta) {
        _currentPos += ((-_lookAt.up * mouseDelta.y) + (-_lookAt.right * mouseDelta.x)) * panSensitivity / 100f;

    }

    public void ZoomCamera(float zoom) {
        distanceFrom += zoomSensitivity * (-zoom / 120f);
        distanceFrom = Mathf.Clamp(distanceFrom, 1f, 20f);
    }

    public void Hover() {
        _closestHit = SetHit();
    }
    private RaycastHit SetHit() {
        Ray cameraRay = MainCamera.ScreenPointToRay(InputController.InputControllerInstance.screenPosition);
        RaycastHit[] cameraRayHits = Physics.RaycastAll(cameraRay, Mathf.Infinity, _hitMask);
        float closestDistance = Mathf.Infinity;
        RaycastHit hit = new();
        foreach (RaycastHit cameraRayHit in cameraRayHits) {
            float angle = Vector3.Angle(cameraRay.direction, cameraRayHit.transform.up);
            Debug.Log($"Angle: {angle}, Game Object: {cameraRayHit.transform.gameObject}");
            if (angle >= 90f && cameraRayHit.distance < closestDistance) {
                hit = cameraRayHit;
                closestDistance = cameraRayHit.distance;
            }
        }
        return hit;
    }

    public void ScreenClick() {
        if (_closestHit.Equals(new RaycastHit())) {
            return;
        }

        GameObject gameObject = _closestHit.transform.gameObject;
        Renderer cellRenderer = gameObject.transform.GetChild(0).GetComponent<Renderer>();
        if (cellRenderer.isVisible) {
            GridCell gridCell = gameObject.GetComponent<GridCell>();
            //gridCell.TurnSurroundingBlue();
            GridPath path = Pathfinder.FindPath(Entity.testCell, gridCell);
            path.TurnBlue();
            
        }
        Debug.Log(_closestHit.transform.gameObject);
    }

    private void Clear() {
        _instance = null;
    }
}
