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

    public void Activate() {
        CameraController.Instance.HoverAction += HoverGrid;
        CameraController.Instance.ClickAction += ClickGrid;
    }
    public void Deactivate() {
        CameraController.Instance.HoverAction -= HoverGrid;
        CameraController.Instance.ClickAction -= ClickGrid;
    }

    private void HoverGrid(GameObject gameObject) {
        if (gameObject == null) {
            return;
        }
        GridCell gridCell = gameObject.GetComponent<GridCell>();
        if (gridCell == null) {
            return;
        }
        HoverAction?.Invoke(gridCell);
    }
    private void ClickGrid(GameObject gameObject) {
        if (gameObject == null) {
            return;
        }
        GridCell gridCell = gameObject.GetComponent<GridCell>();
        if (gridCell == null) {
            return;
        }
        ClickAction?.Invoke(gridCell);
    }
}
