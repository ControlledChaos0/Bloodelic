using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    
    [SerializeField]
    private Camera mainCamera;
    [SerializeField]
    private float cameraSensitivity = 1f;

    private CinemachineBrain _cinemachineBrain;
    private ICinemachineCamera _cinemachineCam;
    private Transform _lookAt;

    private float _rotX;
    private float _rotY;

    private void Awake() {
        _cinemachineBrain = mainCamera.GetComponent<CinemachineBrain>();
        _cinemachineCam = _cinemachineBrain.ActiveVirtualCamera;
        _lookAt = _cinemachineCam.LookAt;

        _rotX = _cinemachineCam.VirtualCameraGameObject.transform.rotation.eulerAngles.x;
        _rotY = _cinemachineCam.VirtualCameraGameObject.transform.rotation.eulerAngles.y;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void LateUpdate() {
        Quaternion newQ = Quaternion.Euler(_rotX, _rotY, 0);
        //transform.RotateAround(_lookAt.position, Vector3.up, _);
    }

    public void RotateCamera(Vector2 mouseDelta) {
        _rotX += mouseDelta.y * cameraSensitivity;
        _rotY -= mouseDelta.x * cameraSensitivity;
        Mathf.Clamp(_rotY, 0f, 90f);
    }
}
