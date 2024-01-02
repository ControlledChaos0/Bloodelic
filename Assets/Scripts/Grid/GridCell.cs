using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GridCell : MonoBehaviour
{
    [SerializeField]
    private GameObject _objectInThisGridSpace = null;
    [SerializeField]
    private Collider _collider;

    public GridCellPosition Position {
        get => _position;
        set => _position = value;
    }
    public Collider Collider {
        get;
        private set;
    }
    public GameObject IsOccupied {
        get {
            return _objectInThisGridSpace;
        }
        set => _isOccupied = value;
    }
    public float g;
    public float h;
    private GridCell[] _neighbors = new GridCell[4];
    private GridCellPosition _position;
    private GridCell _pathTo;
    private bool _isOccupied;

    public void TurnSurroundingBlue() {
        GridManager.LevelGrid.TurnSurroundingBlue(this);
    }
    public float FindHeuristic(GridCell target) {
        h = Vector3.Distance(Position.Position, target.Position.Position);
        return h;
    }
}
