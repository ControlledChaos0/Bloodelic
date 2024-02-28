using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Outline))]
public class Selectable : MonoBehaviour
{
    private Outline _outline;
    public event Action Selection;
    // Start is called before the first frame update
    void Start()
    {
        _outline = GetComponent<Outline>();
        _outline.enabled = false;
        //CameraController.Instance.ClickAction -= Select;
        //CameraController.Instance.ClickAction += Select;
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable() {
        if (CameraController.Instance != null) {
            //CameraController.Instance.ClickAction += Select;
        }
    }
    private void OnDisable() {
        //CameraController.Instance.ClickAction -= Select;
    }

    public void Select() {
        // if (!this.gameObject.Equals(GameObjectHelper.GetParentGameObject(gameObject))) {
        //     return;
        // }
        Selection.Invoke();
    }

    public void HoverSelect() {
        // if (!this.gameObject.Equals(GameObjectHelper.GetParentGameObject(gameObject))) {
        //     return;
        // }
        _outline.enabled = true;
    }
    public void HoverDeselect() {
        // if (!this.gameObject.Equals(GameObjectHelper.GetParentGameObject(gameObject))) {
        //     return;
        // }
        Debug.Log("Is this call??>>>>>>>>>>");
        _outline.enabled = false;
    }
}
