using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputController : Singleton<InputController>
{
    [SerializeField]
    private InputActionAsset inputActions;
    
    private InputActionMap _mainControls;
    private InputAction _scrollAction;
    private InputAction _rightClickAction;
    private InputAction _leftClickAction;
    private InputAction _middleClickAction;
    private InputAction _cursorAction;
    private InputAction _deltaCursorAction;
    private InputAction _cancelAction;
    private Vector2 _mouseDelta;
    private Vector2 _screenPosition;
    private float _scrollDelta;
    private float _pressTime;
    private float _middleClickTime;
    private float _rightClickTime;
    private float _leftClickTime;
    private bool _moveCamera;
    private bool _panCamera;
    //Events
    public event Action MiddleClick;
    public event Action MiddleHold;
    public event Action MiddleCancel;
    public event Action RightClick;
    public event Action RightHold;
    public event Action RightCancel;
    public event Action LeftClick;
    public event Action LeftHold;
    public event Action Cancel;
    public event Action<float> Scroll;
    public event Action<Vector2> Hover;
    public event Action<Vector2> MouseMove;

    private void Awake() {
        InitializeSingleton();
        _mainControls = inputActions.FindActionMap("MainControls");

        _scrollAction = _mainControls.FindAction("Scroll");
        _middleClickAction = _mainControls.FindAction("MiddleClick");
        _rightClickAction = _mainControls.FindAction("RightClick");
        _leftClickAction = _mainControls.FindAction("LeftClick");
        _cursorAction = _mainControls.FindAction("Cursor");
        _deltaCursorAction = _mainControls.FindAction("DeltaCursor");
        _cancelAction = _mainControls.FindAction("Cancel");

        int holdIndex = _middleClickAction.interactions.IndexOf("Hold(duration=");
        _pressTime = float.Parse(_middleClickAction.interactions.Substring(holdIndex + 14, 3));

        _middleClickAction.started += OnMiddleClickStarted;
        _middleClickAction.performed += OnMiddleClickPerformed;
        _middleClickAction.canceled += OnMiddleClickCanceled;
        _rightClickAction.started += OnRightClickStarted;
        _rightClickAction.performed += OnRightClickPerformed;
        _rightClickAction.canceled += OnRightClickCanceled;
        _leftClickAction.started += OnLeftClickStarted;
        _leftClickAction.canceled += OnLeftClickCanceled;
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
    private void OnDeltaCursor(InputValue inputValue) {
        _mouseDelta = inputValue.Get<Vector2>();
        MouseMove?.Invoke(_mouseDelta);
        OnHover(_mouseDelta.magnitude);
    }
    private void OnScroll(InputValue inputValue) {
        _scrollDelta = inputValue.Get<float>();
        Scroll?.Invoke(_scrollDelta);
        OnHover(_scrollDelta);
    }
    private void OnCancel(InputValue inputValue) {
        Cancel.Invoke();
    }
    private void OnMiddleClickStarted(InputAction.CallbackContext context) {
        _middleClickTime = Time.time;
    }
    private void OnMiddleClickPerformed(InputAction.CallbackContext context) {
        MiddleHold?.Invoke();
        Debug.Log("Middle hold!");
    }
    private void OnMiddleClickCanceled(InputAction.CallbackContext context) {
        if (Time.time - _middleClickTime >= _pressTime) {
            Debug.Log("Stop middle hold!");
            MiddleCancel?.Invoke();
            return;
        }
        MiddleClick?.Invoke();
        Debug.Log("Just a middle click!!!");
    }
    private void OnRightClickStarted(InputAction.CallbackContext context) {
        _rightClickTime = Time.time;
    }
    private void OnRightClickPerformed(InputAction.CallbackContext context) {
        RightHold?.Invoke();
    }
    private void OnRightClickCanceled(InputAction.CallbackContext context) {
        if (Time.time - _rightClickTime >= _pressTime) {
            RightCancel?.Invoke();
            return;
        }
        RightClick?.Invoke();
        Debug.Log("Just a right click!!!");
    }
    private void OnLeftClickStarted(InputAction.CallbackContext context) {
        _leftClickTime = Time.time;
    }
    private void OnLeftClickCanceled(InputAction.CallbackContext context) {
        if (Time.time - _leftClickTime >= _pressTime) {
            return;
        }
        Debug.Log("Just a click!!!");
        LeftClick?.Invoke();
    }
    private void OnHover(float d) {
        if (d != 0f)
            Hover?.Invoke(_screenPosition);
    }
}
