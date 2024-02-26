using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selectable : MonoBehaviour
{
    public Action Selection;
    public void Select(GameObject gameObject) {
        if (!this.gameObject.Equals(GameObjectHelper.GetParentGameObject(gameObject))) {
            return;
        }
        Selection.Invoke();
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
        CameraController.Instance.ClickAction += Select;
    }
    private void OnDisable() {
        CameraController.Instance.ClickAction -= Select;
    }
}
