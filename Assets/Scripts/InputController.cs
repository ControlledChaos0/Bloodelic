using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputController : MonoBehaviour
{
    [SerializeField]
    private InputActionAsset inputActions;
    [SerializeField]
    private CameraController cameraController;
    private InputActionMap _cameraControls;
    private InputAction _moveCameraAction;
    private InputAction _checkMoveCameraAction;
    private InputAction _checkPanCameraAction;
    private Vector2 _mouseDelta;
    private bool _moveCamera;
    private bool _panCamera;

    private void Awake() {
        _cameraControls = inputActions.FindActionMap("MainControls");

        _moveCameraAction = _cameraControls.FindAction("MoveCamera");
        _checkMoveCameraAction = _cameraControls.FindAction("CheckMoveCamera");
        _checkPanCameraAction = _cameraControls.FindAction("CheckPanCamera");

        _checkMoveCameraAction.performed += OnCheckMoveCameraPerformed;
        _checkMoveCameraAction.canceled += OnCheckMoveCameraCanceled;
        _checkPanCameraAction.performed += OnCheckPanCameraPerformed;
        _checkPanCameraAction.canceled += OnCheckPanCameraCanceled;
        _moveCamera = false;
        _panCamera = false;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable() {
        inputActions.FindActionMap("MainControls").Enable();
    }

    private void OnDisable() {
        inputActions.FindActionMap("MainControls").Disable();
    }



    private void OnMoveCamera(InputValue inputValue) {
        _mouseDelta = inputValue.Get<Vector2>();
        if (_moveCamera) {
            cameraController.RotateCamera(_mouseDelta);
        } else if (_panCamera) {
            cameraController.PanCamera(_mouseDelta);
        }
    }

    private void OnZoomCamera(InputValue inputValue) {
        cameraController.ZoomCamera(inputValue.Get<float>());
    }

    private void OnCheckMoveCameraPerformed(InputAction.CallbackContext context) {
        _moveCamera = true;
        Debug.Log("Performed");
    }

    private void OnCheckMoveCameraCanceled(InputAction.CallbackContext context) {
        _moveCamera = false;
        Debug.Log("Canceled");
    }
    private void OnCheckPanCameraPerformed(InputAction.CallbackContext context) {
        _panCamera = true;
    }
    private void OnCheckPanCameraCanceled(InputAction.CallbackContext context) {
        _panCamera = false;
    }
}
