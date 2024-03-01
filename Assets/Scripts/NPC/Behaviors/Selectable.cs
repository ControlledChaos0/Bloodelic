using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Outline))]
public class Selectable : MonoBehaviour
{
    private Outline _outline;
    public event Action<GameObject> ClickAction;
    public event Action<GameObject> HoverAction;
    public event Action SelectionAction;
    // Start is called before the first frame update
    void Start()
    {
        _outline = GetComponent<Outline>();
        _outline.enabled = false;
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    public void Activate() {
        CameraController.Instance.ClickAction += ClickAction;
        CameraController.Instance.HoverAction += HoverAction;
        ClickHandler.Instance.Deactivate();
    }

    public void Deactivate() {
        CameraController.Instance.ClickAction -= ClickAction;
        CameraController.Instance.HoverAction -= HoverAction;
        ClickHandler.Instance.Activate();
    }

    public void Select() {
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
