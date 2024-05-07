using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditor;
using UnityEngine;

[Serializable]
public class GridCell : MonoBehaviour, IPublisher<Occupant, GridCell>
{
    //Most of these are serialized explicitly so that these values will be saved when grid is created in editor
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
    private Color _savedColor;
    private bool _isOccupied;
    
    public MovementEvent ItemMoved = new MovementEvent();

    private void Start() {
        _renderer = gameObject.transform.GetChild(0).GetComponent<Renderer>();
        _savedColor = _renderer.material.color;
        //_collider.enabled = false;
        //GridManager.OnCellOccupantChanged += OnCellOccupantChanged;
    }

    private void OnDestroy()
    {
        //GridManager.OnCellOccupantChanged -= OnCellOccupantChanged;
    }

    public void TurnBlue() {
        _renderer.material.color = Color.red;
    }
    public void ChangeColor(Color color) {
        _renderer.material.color = color;
    }
    public void SaveColor() {
        if (_renderer == null) {
            _renderer = gameObject.transform.GetChild(0).GetComponent<Renderer>();
        }
        _savedColor = _renderer.material.color;
    }
    public void RevertColor() {
        _renderer.material.color = _savedColor;
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
    public void ShowCell()
    {
        _renderer.enabled = true;
    }
    public void HideCell()
    {
        _renderer.enabled = false;
    }
    public bool IsShowing() {
        return _renderer.enabled;
    }
    public void SetF(float g, float h) {
        G = g;
        H = h;
    }
    public float FindHeuristic(GridCell target) {
        H = Vector3.Distance(Position.Position, target.Position.Position);
        return H;
    }
    
    #region Occupant

    [SerializeField]
    private Occupant _blockOccupant;
    [SerializeField]
    private Occupant[] _nonblockOccupants = new Occupant[4];
    public Occupant BlockOccupant => _blockOccupant;
    public Occupant[] NoBlockOccupants => _nonblockOccupants;
    
    public void SetOccupant(Occupant occupant)
    {
        if (occupant.BlockCells) {

        } else {

        }
        Debug.Log("SetOccupant called");
        // Log error if cell is occupied
        if (_occupant != null)
        {
            Debug.LogError("Attempting to occupy cell " + name + " occupied by " + _occupant.name);
            return;
        }

        _occupant = occupant;

        Publish(_occupant, this);
        
        // For human occupants, also set wall neighbors to occupied
        List<GridCell> wallNeighbors = GetWallNeighbors();
        foreach (var n in wallNeighbors)
        {
            n._occupant = occupant;
        }
    }
    
    public void Unoccupy(bool alsoUnoccupyWallNeighbors = true)
    {
        if (alsoUnoccupyWallNeighbors)
        {
            List<GridCell> wallNeighbors = GetWallNeighbors();
            foreach (var n in wallNeighbors)
            {
                n.Unoccupy(false);
            }
        }
        _occupant = null;
    }
    

    // A cell is considered occupied when:
    //  - An entity occupies it
    //  - A LARGE object occupies it
    public bool IsOccupied()
    {
        // @Add objects stuff
        return _occupant != null;
    }
    
    public List<GridCell> GetWallNeighbors()
    {
        List<GridCell> wallNeighbors = new List<GridCell>();

        foreach (var n in _neighbors)
        {
            if (n != null && n._position.Position.y > _position.Position.y)
            {
                wallNeighbors.Add(n);                    
            }
        }
        
        return wallNeighbors;
    }

    public bool IsWall()
    {
        return Position.PositionE != GridCellPositionEnum.BOTTOM;
    }

    #endregion
    
    #region Debug
        #if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (IsOccupied())
            {
                Handles.color = Color.red;
                string debugString = string.Format("Occupant: {0}", _blockOccupant.name);
                Handles.Label(transform.position + Vector3.up * 0.5f, debugString);
                Handles.color = Color.white;
            }
        }
        #endif
    #endregion

    #region Publisher

    public void Publish(Occupant e, GridCell g) {
        ItemMoved?.Invoke(e, g);
    }

    #endregion
}
