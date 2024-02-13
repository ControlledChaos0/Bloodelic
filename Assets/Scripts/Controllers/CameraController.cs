using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEditor;
using System;

public class CameraController : Singleton<CameraController>
{
    [SerializeField]
    private Camera mainCamera;
    [SerializeField]
    private CameraMachine cameraMachine;
    [SerializeField]
    private float cameraSensitivity = 0.5f;
    [SerializeField]
    private float zoomSensitivity = 0.5f;
    [SerializeField]
    private float panSensitivity = 5f;
    [SerializeField]
    private float distanceFrom = 5.0f;
    [SerializeField]
    private GameObject corner1;
    [SerializeField]
    private GameObject corner2;

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
    private bool _rotateCamera;
    private bool _panCamera;
    private LayerMask _hitMask;

    private Vector3 cornerMaxPos;
    private Vector3 cornerMinPos;

    public Camera MainCamera {
        get => mainCamera;
        private set => mainCamera = value;
    }
    public RaycastHit ClosestHit {
        get => _closestHit;
        private set => _closestHit = value;
    }
    public LayerMask HitMask {
        get => _hitMask;
        set => _hitMask = value;
    }

    public event Action<GameObject> ClickAction;
    public event Action<GameObject> HoverAction;

    private void Awake() {
        InitializeSingleton();
        _cinemachineBrain = mainCamera.GetComponent<CinemachineBrain>();
        SeparateCameraObject();
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
        
    }
    private void OnDisable() {
        
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

        _panCamera = false;
        _rotateCamera = false;

        cornerMaxPos = new Vector3(Mathf.Max(corner1.transform.position.x, corner2.transform.position.x), 
                                            Mathf.Max(corner1.transform.position.y, corner2.transform.position.y), 
                                            Mathf.Max(corner1.transform.position.z, corner2.transform.position.z));
        cornerMinPos = new Vector3(Mathf.Min(corner1.transform.position.x, corner2.transform.position.x), 
                                            Mathf.Min(corner1.transform.position.y, corner2.transform.position.y), 
                                            Mathf.Min(corner1.transform.position.z, corner2.transform.position.z));
    }

    public void StartRotateCamera() {
        if (_panCamera) {
            return;
        }
        InputController.Instance.MouseMove += RotateCamera;
        _rotateCamera = true;
    }
    public void StopRotateCamera() {
        InputController.Instance.MouseMove -= RotateCamera;
        _rotateCamera = false;
    }
    public void RotateCamera(Vector2 mouseDelta) {
        _rotX -= mouseDelta.y * cameraSensitivity;
        _rotY += mouseDelta.x * cameraSensitivity;
        _rotX = Mathf.Clamp(_rotX, 0f, 90f);
        //Debug.Log(_rotX);
    }

    public void StartPanCamera() {
        if (_rotateCamera) {
            return;
        }
        InputController.Instance.MouseMove += PanCamera;
        _panCamera = true;
    }
    public void StopPanCamera() {
        InputController.Instance.MouseMove -= PanCamera;
        _panCamera = false;
    }
    public void PanCamera(Vector2 mouseDelta) {
        _currentPos += ((-_lookAt.up * mouseDelta.y) + (-_lookAt.right * mouseDelta.x)) * panSensitivity / 100f;

        // Limit camera to the bounds given by corner1 and corner2
        _currentPos.x = _currentPos.x > cornerMaxPos.x ? cornerMaxPos.x : _currentPos.x;
        _currentPos.y = _currentPos.y > cornerMaxPos.y ? cornerMaxPos.y : _currentPos.y;
        _currentPos.z = _currentPos.z > cornerMaxPos.z ? cornerMaxPos.z : _currentPos.z;

        _currentPos.x = _currentPos.x < cornerMinPos.x ? cornerMinPos.x : _currentPos.x;
        _currentPos.y = _currentPos.y < cornerMinPos.y ? cornerMinPos.y : _currentPos.y;
        _currentPos.z = _currentPos.z < cornerMinPos.z ? cornerMinPos.z : _currentPos.z;
    }

    public void ZoomCamera(float zoom) {
        distanceFrom += zoomSensitivity * (-zoom / 120f);
        distanceFrom = Mathf.Clamp(distanceFrom, 1f, 20f);
    }

    public void Hover(Vector2 screenPos) {
        Ray cameraRay = MainCamera.ScreenPointToRay(screenPos);
        _closestHit = SetHit(cameraRay);
        if (_closestHit.Equals(new RaycastHit())) {
            return;
        }
        HoverAction?.Invoke(_closestHit.transform.gameObject);
    }

    private RaycastHit SetHit(Ray ray) {
        RaycastHit[] cameraRayHits = Physics.RaycastAll(ray, Mathf.Infinity, _hitMask);
        float closestDistance = Mathf.Infinity;
        RaycastHit hit = new();
        foreach (RaycastHit cameraRayHit in cameraRayHits) {
            float angle = Vector3.Angle(ray.direction, cameraRayHit.transform.up);
            //Debug.Log($"Angle: {angle}, Game Object: {cameraRayHit.transform.gameObject}");
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
        GameObject hitGO = _closestHit.transform.gameObject;
        ClickAction?.Invoke(hitGO);
        
        Debug.Log(hitGO);
    }

    private void SeparateCameraObject() {
        transform.GetChild(0).parent = null;
    }
}
