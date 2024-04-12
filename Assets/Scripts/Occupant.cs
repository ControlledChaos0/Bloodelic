using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Occupant : MonoBehaviour
{
    [SerializeField]
    private GridCell _occupiedCell;

    public GridCell OccupiedCell {
        get => _occupiedCell;
        set => _occupiedCell = value;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetOccupation(GridCell cell)
    {
        if (cell == null) { return; }
        _occupiedCell.Unoccupy();
        _occupiedCell = cell;
        // Set occupant for occupied cell
        _occupiedCell.SetOccupant(this);
    }
}
