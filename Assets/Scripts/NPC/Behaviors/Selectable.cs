using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

[RequireComponent(typeof(Outline))]
public class Selectable : MonoBehaviour
{
    
    private Outline _outline;
    public event Action<GameObject> ClickAction;
    public event Action<GameObject> HoverAction;
    public static Action<GameObject> SelectedUI;
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
    
    public void Select(GameObject gameObject) {
        Debug.Log(gameObject + "is selected");
        if (!this.gameObject.Equals(GameObjectHelper.GetParentGameObject(gameObject))) {
            return;
        }
        SelectionAction.Invoke();
        SelectedUI?.Invoke(gameObject);
    }

    public void HoverSelect() {
        _outline.enabled = true;
    }
    public void HoverDeselect() {
        Debug.Log("Is this call??>>>>>>>>>>");
        _outline.enabled = false;
    }
}
