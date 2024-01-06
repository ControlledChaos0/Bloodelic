using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

[Serializable]
public class GridCell : MonoBehaviour
{
    //Most of these are serialized explicitly so that these values will be saved when grid is created in editor
    [SerializeField]
    private GameObject _objectInThisGridSpace = null;
    [SerializeField]
    private Collider _collider;
    [SerializeField]
    private GridCellPosition _position;
    [SerializeReference]
    private GridCell[] _neighbors = new GridCell[4];

    public GridCell[] Neighbors {
        get => _neighbors;
        set => _neighbors = value;
    }
    public GridCell PathTo {
        get => _pathTo;
        set => _pathTo = value;
    }
    public GridCellPosition Position {
        get => _position;
        set => _position = value;
    }
    public Collider Collider {
        get;
        private set;
    }
    public float F => G + H;
    public float G;
    public float H;
    private GridCell _pathTo;
    private Renderer _renderer;
    private bool _isOccupied;

    private void Start() {
        _renderer = gameObject.transform.GetChild(0).GetComponent<Renderer>();
    }

    public void TurnBlue() {
        _renderer.material.color = Color.blue;
    }
    public void TurnSurroundingBlue() {
        //GridManager.LevelGrid.TurnSurroundingBlue(this);

        TurnBlue();
        foreach (GridCell connectedGridCell in Neighbors) {
            if (connectedGridCell != null) {
                connectedGridCell.TurnBlue();
            }
        }
    }
    public void SetF(float g, float h) {
        G = g;
        H = h;
    }
    public float FindHeuristic(GridCell target) {
        H = Vector3.Distance(Position.Position, target.Position.Position);
        return H;
    }
}
