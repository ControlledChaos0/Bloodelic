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
    private InputAction _clickAction;
    private Vector2 _mouseDelta;
    private float _pressTime;
    private float _clickTime;
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
        _clickAction = _cameraControls.FindAction("Click");

        int holdIndex = _checkMoveCameraAction.interactions.IndexOf("Hold(duration=");
        _pressTime = float.Parse(_checkMoveCameraAction.interactions.Substring(holdIndex + 14, 3));

        _clickAction.started += OnClickStarted;
        _clickAction.canceled += OnClickCanceled;

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

    private void OnClickStarted(InputAction.CallbackContext context) {
        _clickTime = Time.time;
    }

    private void OnClickCanceled(InputAction.CallbackContext context) {
        if (Time.time - _clickTime < _pressTime) {
            Debug.Log("Just a click!!!");
        }
    }

    private void OnCheckMoveCameraPerformed(InputAction.CallbackContext context) {
        _moveCamera = true;
        Debug.Log("Check Move Camera Performed");
    }

    private void OnCheckMoveCameraCanceled(InputAction.CallbackContext context) {
        _moveCamera = false;
        Debug.Log("Check Move Camera Canceled");
    }
    private void OnCheckPanCameraPerformed(InputAction.CallbackContext context) {
        _panCamera = true;
    }
    private void OnCheckPanCameraCanceled(InputAction.CallbackContext context) {
        _panCamera = false;
    }
}
