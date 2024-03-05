using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

[RequireComponent(typeof(Outline))]
public class Selectable : MonoBehaviour
{
    [SerializeField]
    private GameObject _uiObject; //Will be a part of the object prefab, so will be added with it

    private WorldUI _uiScript;
    private Outline _outline;

    public event Action<GameObject> ClickAction;
    public event Action<GameObject> HoverAction;
    public event Action SelectionAction;

    // Start is called before the first frame update
    void Start()
    {
        _outline = GetComponent<Outline>();
        _outline.enabled = false;

        //Lord this is ugly
        //I hate this
        //Maybe there's a way to separate these two things, but I've been brainstorming for hours
        //And I just need something
        //Fuck cohesion
        _uiScript = _uiObject.GetComponent<WorldUI>();
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    public void Activate() {
        CameraController.Instance.ClickAction += Click;
        CameraController.Instance.HoverAction += Hover;
        ClickHandler.Instance.Deactivate();
        _uiScript.Activate();
    }

    public void Deactivate() {
        CameraController.Instance.ClickAction -= Click;
        CameraController.Instance.HoverAction -= Hover;
        ClickHandler.Instance.Activate();
        _uiScript.Deactivate();
    }
    //These two probably repeative, and might be better to just go directly to CameraController action
    //But like having a middle man for now just in case there needs to be additional functionality and filtering
    public void Click(GameObject gO) {
        ClickAction?.Invoke(gO);
    }
    public void Hover(GameObject gO) {
        HoverAction?.Invoke(gO);
    }

    public void Select() {
        Activate();
    }
    
    public void Select(GameObject gameObject) {
        Debug.Log(gameObject + "is selected");
        if (!this.gameObject.Equals(GameObjectHelper.GetParentGameObject(gameObject))) {
            return;
        }
        SelectionAction.Invoke();
    }

    public void HoverSelect() {
        _outline.enabled = true;
    }
    public void HoverDeselect() {
        Debug.Log("Is this call??>>>>>>>>>>");
        _outline.enabled = false;
    }
}
