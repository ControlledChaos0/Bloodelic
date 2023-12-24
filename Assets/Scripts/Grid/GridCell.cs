using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private GridCellPosition _position;
    private bool _isOccupied;

    // Start is called before the first frame update
    void Start()
    {
        //IsOccupied = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TurnSurroundingBlue() {
        GridManager.LevelGrid.TurnSurroundingBlue(this);
    }
}
