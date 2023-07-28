using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    
    [SerializeField]
    private Camera mainCamera;
    [SerializeField]
    private float cameraSensitivity = 0.5f;
    [SerializeField]
    private float zoomSensitivity = 0.5f;
    [SerializeField]
    private float panSensitivity = 0.5f;
    [SerializeField]
    private float distanceFrom = 5.0f;

    private CinemachineBrain _cinemachineBrain;
    private CinemachineVirtualCamera _cinemachineCam;
    private Transform _lookAt;
    private Vector3 _oldRot;
    private Vector3 _defaultPos;
    private Vector3 _currentPos;
    private Vector3 _panMove;
    private float _rotX;
    private float _rotY;

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
        _currentPos += ((_lookAt.up * mouseDelta.y) + (_lookAt.right * mouseDelta.x)) * panSensitivity;

    }

    public void ZoomCamera(float zoom) {
        distanceFrom += zoomSensitivity * (-zoom / 120f);
        distanceFrom = Mathf.Clamp(distanceFrom, 1f, 20f);
    }
}
