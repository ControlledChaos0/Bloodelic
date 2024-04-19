using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using Unity.Profiling;
using UnityEngine;

public class Selectable : MonoBehaviour
{
    [SerializeField]
    private GameObject _modelObject;
    [SerializeField]
    private GameObject _uiObject; //Will be a part of the object prefab, so will be added with it

    private WorldUI _uiScript;
    private Outline _outline;
    private Color _defaultOutlineColor;
    private BehaviorController _behaviorController;

    public Color DefaultOutlineColor {
        get { return _defaultOutlineColor;}
    }
    public GameObject ModelObject {
        get => _modelObject;
    }
    public GameObject UIObject {
        get => _uiObject;
    }
    public WorldUI UIScript {
        get => _uiScript;
    }
    public BehaviorController GetBehaviorController {
        get => _behaviorController;
    }

    public event Action ClickAction;
    public event Action HoverAction;
    public event Action UnhoverAction;

    // Start is called before the first frame update
    void Start()
    {
        _outline = _modelObject.GetComponent<Outline>();
        if (_outline == null) {
            _outline = _modelObject.AddComponent<Outline>();
        }
        _defaultOutlineColor = _outline.OutlineColor;
        _outline.enabled = false;

        _uiScript = _uiObject.GetComponent<WorldUI>();
        _behaviorController = GetComponent<BehaviorController>();

        StartSubobjects();
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    public void Activate() {
    }

    public void Deactivate() {
    }
    //These two probably repeative, and might be better to just go directly to CameraController action
    //But like having a middle man for now just in case there needs to be additional functionality and filterin
    public void ClickUI() {
        SelectState tempState = SelectStateMachine.Instance.currentState;
        if (typeof(SearchSelectState).Equals(tempState.GetType())) {
            SelectStateMachine.Instance.StartSelectable(this);
        }
    }

    public void HoverSelect() {
        _outline.enabled = true;
        //_uiObject.SetActive(true);
        HoverAction?.Invoke();
    }
    public void HoverDeselect() {
        Debug.Log("Is this call??>>>>>>>>>>");
        _outline.enabled = false;
        //_uiObject.SetActive(false);
        UnhoverAction?.Invoke();
    }

    public void ChangeOutlineColor(Color color) {
        _outline.OutlineColor = color;
    }
    public void ChangeOutlineColor() {
        ChangeOutlineColor(DefaultOutlineColor);
    }

    private void StartSubobjects() {
        _behaviorController.InitializeBehaviors();
        _uiScript.AddButtons(_behaviorController.Behaviors);
    }
}
