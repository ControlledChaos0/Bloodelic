using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMovement : MonoBehaviour
{
    [SerializeField]
    private InputActionAsset inputActions;
    private InputActionMap cameraControls;
    private InputAction moveCameraAction;
    private InputAction checkMoveCameraAction;
    private Vector2 mouseDelta;
    private bool moveCamera;

    private void Awake() {
        cameraControls = inputActions.FindActionMap("MainControls");

        moveCameraAction = cameraControls.FindAction("MoveCamera");
        checkMoveCameraAction = cameraControls.FindAction("CheckMoveCamera");

        checkMoveCameraAction.performed += OnCheckMoveCameraPerformed;
        checkMoveCameraAction.canceled += OnCheckMoveCameraCanceled;
        moveCamera = false;
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
        mouseDelta = moveCamera ? inputValue.Get<Vector2>() : Vector2.zero;
    }

    private void OnCheckMoveCameraPerformed(InputAction.CallbackContext context) {
        moveCamera = true;
        Debug.Log("Performed");
    }

    private void OnCheckMoveCameraCanceled(InputAction.CallbackContext context) {
        moveCamera = false;
        Debug.Log("Canceled");
    }
}
