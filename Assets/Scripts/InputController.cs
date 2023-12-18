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
    private InputAction _rightClickAction;
    private InputAction _leftClickAction;
    private InputAction _cursorAction;
    private Vector2 _mouseDelta;
    private Vector2 _screenPosition;
    private float _pressTime;
    private float _clickTime;
    private bool _moveCamera;
    private bool _panCamera;

    private void Awake() {
        _cameraControls = inputActions.FindActionMap("MainControls");

        _moveCameraAction = _cameraControls.FindAction("MoveCamera");
        _checkMoveCameraAction = _cameraControls.FindAction("CheckMoveCamera");
        _checkPanCameraAction = _cameraControls.FindAction("CheckPanCamera");
        _rightClickAction = _cameraControls.FindAction("RightClick");
        _leftClickAction = _cameraControls.FindAction("LeftClick");
        _cursorAction = _cameraControls.FindAction("Cursor");

        _checkMoveCameraAction.performed += OnCheckMoveCameraPerformed;
        _checkMoveCameraAction.canceled += OnCheckMoveCameraCanceled;
        _checkPanCameraAction.performed += OnCheckPanCameraPerformed;
        _checkPanCameraAction.canceled += OnCheckPanCameraCanceled;

        int holdIndex = _checkMoveCameraAction.interactions.IndexOf("Hold(duration=");
        _pressTime = float.Parse(_checkMoveCameraAction.interactions.Substring(holdIndex + 14, 3));

        _rightClickAction.started += OnRightClickStarted;
        _rightClickAction.canceled += OnRightClickCanceled;
        _leftClickAction.started += OnLeftClickStarted;
        _leftClickAction.canceled += OnLeftClickCanceled;

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


    private void OnCursor(InputValue inputValue) {
        _screenPosition = inputValue.Get<Vector2>();
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

    private void OnRightClickStarted(InputAction.CallbackContext context) {
        _clickTime = Time.time;
    }

    private void OnRightClickCanceled(InputAction.CallbackContext context) {
        if (Time.time - _clickTime >= _pressTime) {
            return;
        }
        Debug.Log("Just a click!!!");
    }

    private void OnLeftClickStarted(InputAction.CallbackContext context) {
        
    }

    private void OnLeftClickCanceled(InputAction.CallbackContext context) {
        Ray cameraRay = cameraController.MainCamera.ScreenPointToRay(_screenPosition);
        RaycastHit[] cameraRayHits = Physics.RaycastAll(cameraRay, Mathf.Infinity, 1 << 3);
        int position = -1;
        float closestDistance = Mathf.Infinity;
        int step = 0;
        foreach (RaycastHit cameraRayHit in cameraRayHits) {
            float angle = Vector3.Angle(cameraRay.direction, cameraRayHit.transform.up);
            Debug.Log($"Angle: {angle}, Game Object: {cameraRayHit.transform.gameObject}");
            if (angle >= 90f && cameraRayHit.distance < closestDistance) {
                position = step;
                closestDistance = cameraRayHit.distance;
            }
            step++;
        }
        Debug.Log($"Position: {position}");
        if (position != -1) {
            GameObject gridCell = cameraRayHits[position].transform.gameObject;
            Renderer cellRenderer = gridCell.gameObject.transform.GetChild(0).GetComponent<Renderer>();
            if (cellRenderer.isVisible) {
                gridCell.GetComponent<GridCell>().TurnSurroundingBlue();
            }
            Debug.Log(cameraRayHits[position].transform.gameObject);
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
