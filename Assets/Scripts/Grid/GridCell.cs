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
    public float F => g + h;
    public float g;
    public float h;
    private GridCell _pathTo;
    private bool _isOccupied;

    public void TurnSurroundingBlue() {
        //GridManager.LevelGrid.TurnSurroundingBlue(this);

        gameObject.transform.GetChild(0).GetComponent<Renderer>().material.color = Color.blue;
        foreach (GridCell connectedGridCell in Neighbors) {
            connectedGridCell.gameObject.transform.GetChild(0).GetComponent<Renderer>().material.color = Color.blue;
        }
    }
    public float FindHeuristic(GridCell target) {
        h = Vector3.Distance(Position.Position, target.Position.Position);
        return h;
    }
}
