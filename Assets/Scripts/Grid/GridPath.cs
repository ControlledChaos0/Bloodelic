using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridPath
{
    public LinkedList<GridCell> Path {
        get => _path;
        set => _path = value;
    }
    public int Count => _path.Count;
    private LinkedList<GridCell> _path;

    public GridPath() : this(new LinkedList<GridCell>()) {

    }
    public GridPath(LinkedList<GridCell> p) {
        ArgumentNullExceptionUse.ThrowIfNull(p);

        _path = p;
    }
    public GridPath(GridCell end) {
        ArgumentNullExceptionUse.ThrowIfNull(end);

        _path = new();

        if (end.PathTo == null) {
            return;
        }

        GridCell current = end;
        Dictionary<GridCell, bool> existance = new();
        
        _path.AddFirst(current);
        existance.Add(current, true);

        while (current.PathTo != null) {
            current = current.PathTo;
            //Ensure there are no loops that get stuck infinitely!
            if (existance.ContainsKey(current)) {
                return;
            }
            _path.AddFirst(current);
            existance.Add(current, true);
        }
    }

    public GridCell PopFront() {
        ArgumentNullExceptionUse.ThrowIfNull(_path);
        GridCell gridCell = _path.First.Value;
        _path.RemoveFirst();
        return gridCell;
    }

    public void TurnBlue() {
        foreach (GridCell gridCell in _path) {
            gridCell.TurnBlue();
        }
    }
}
