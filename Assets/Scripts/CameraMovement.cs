using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMovement : MonoBehaviour
{
    [SerializeField]
    private InputActionAsset inputActions;
    private InputActionMap cameraControls;
    private Vector2 mouseDelta;
    private bool moveCamera;

    private void Awake() {
        cameraControls = inputActions.FindActionMap("MainControls");
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
        
    }

    private void OnDisable() {
        
    }



    private void OnMoveCamera(InputValue inputValue) {
        mouseDelta = inputValue.Get<Vector2>();
    }

    private void OnCheckMoveCamera(InputValue inputValue) {
        float checkMouse = inputValue.Get<float>();
        if (checkMouse >= InputSystem.settings.defaultHoldTime) {
            Debug.Log("Button Held");
        }
    }
}
