using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    
    [SerializeField]
    private Camera mainCamera;
    [SerializeField]
    private Transform lookAt;
    [SerializeField]
    private float cameraSensitivity = 0.5f;
    [SerializeField]
    private float zoomSensitivity = 0.5f;
    [SerializeField]
    private float distanceFrom = 5.0f;

    private CinemachineBrain _cinemachineBrain;
    private CinemachineVirtualCamera _cinemachineCam;
    private Vector3 _oldRot;

    private float _rotX;
    private float _rotY;

    private void Awake() {
        _cinemachineBrain = mainCamera.GetComponent<CinemachineBrain>();
    }

    // Start is called before the first frame update
    void Start()
    {
        ChangeCamera();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void LateUpdate() {
        _cinemachineCam.m_Lens.OrthographicSize = distanceFrom;

        Vector3 newRot =  new Vector3(_rotX, _rotY, 0);
        Vector3 dir = newRot - _oldRot;
        Vector3 camPos = _cinemachineCam.VirtualCameraGameObject.transform.position;
        Vector3 lookToCam = camPos - lookAt.transform.position;
        Vector3 upCross = Vector3.Cross(lookToCam, Vector3.up);
        _cinemachineCam.VirtualCameraGameObject.transform.RotateAround(lookAt.position, upCross, dir.x);
        _cinemachineCam.VirtualCameraGameObject.transform.RotateAround(lookAt.position, Vector3.up, dir.y);

        _cinemachineCam.VirtualCameraGameObject.transform.LookAt(lookAt);
        _oldRot = newRot;
    }

    private void ChangeCamera() {
        _cinemachineCam = _cinemachineBrain.ActiveVirtualCamera.VirtualCameraGameObject.GetComponent<CinemachineVirtualCamera>();
        _oldRot = _cinemachineCam.VirtualCameraGameObject.transform.rotation.eulerAngles;
        _rotX = _cinemachineCam.VirtualCameraGameObject.transform.rotation.eulerAngles.x;
        _rotY = _cinemachineCam.VirtualCameraGameObject.transform.rotation.eulerAngles.y;
        distanceFrom = _cinemachineCam.m_Lens.OrthographicSize;

        Debug.Log(_rotX);
        //_cinemachineCam.VirtualCameraGameObject.transform.position = (_cinemachineCam.VirtualCameraGameObject.transform.position - lookAt.position).normalized * distanceFrom;
    }

    public void RotateCamera(Vector2 mouseDelta) {
        _rotX -= mouseDelta.y * cameraSensitivity;
        _rotY += mouseDelta.x * cameraSensitivity;
        _rotX = Mathf.Clamp(_rotX, 0f, 90f);
        Debug.Log(_rotX);
    }

    public void ZoomCamera(float zoom) {
        distanceFrom += zoomSensitivity * (-zoom / 120f);
        distanceFrom = Mathf.Clamp(distanceFrom, 1f, 20f);
    }
}
