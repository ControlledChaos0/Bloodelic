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
    private Vector2 _mouseDelta;
    private bool _moveCamera;

    private void Awake() {
        _cameraControls = inputActions.FindActionMap("MainControls");

        _moveCameraAction = _cameraControls.FindAction("MoveCamera");
        _checkMoveCameraAction = _cameraControls.FindAction("CheckMoveCamera");

        _checkMoveCameraAction.performed += OnCheckMoveCameraPerformed;
        _checkMoveCameraAction.canceled += OnCheckMoveCameraCanceled;
        _moveCamera = false;
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
        _mouseDelta = _moveCamera ? inputValue.Get<Vector2>() : Vector2.zero;
        cameraController.RotateCamera(_mouseDelta);
    }

    private void OnCheckMoveCameraPerformed(InputAction.CallbackContext context) {
        _moveCamera = true;
        Debug.Log("Performed");
    }

    private void OnCheckMoveCameraCanceled(InputAction.CallbackContext context) {
        _moveCamera = false;
        Debug.Log("Canceled");
    }
}
