using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using UnityEngine;

public class GridManager : Singleton<GridManager>
{
    public event Action<GridCell> HoverAction;
    public event Action<GridCell> ClickAction;

    private void Awake() {
        InitializeSingleton();
    }
    private void Start() {
        //CameraController.Instance.HoverAction += HoverGrid;
        //CameraController.Instance.ClickAction += ClickGrid;
    }
    private void Update() {
        
    }
    private void OnEnable() {

    }
    private void OnDisable() {
        
    }
    private void OnDestroy() {
        //CameraController.Instance.HoverAction -= HoverGrid;
        //CameraController.Instance.ClickAction -= ClickGrid;
    }

    private void HoverGrid(GameObject gameObject) {
        GridCell gridCell = gameObject.GetComponent<GridCell>();
        if (gridCell == null) {
            return;
        }
        HoverAction?.Invoke(gridCell);
    }
    private void ClickGrid(GameObject gameObject) {
        GridCell gridCell = gameObject.GetComponent<GridCell>();
        if (gridCell == null) {
            return;
        }
        ClickAction?.Invoke(gridCell);
    }
}
