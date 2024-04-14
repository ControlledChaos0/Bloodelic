using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

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

        _path = new();
        foreach (GridCell gridCell in p) {
            _path.AddLast(gridCell);
        }
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

    public GridCell GetFront(){
        ArgumentNullExceptionUse.ThrowIfNull(_path);
        if (_path.First == null) {
            return null;
        }
        return _path.First.Value;
    }

    public GridCell PopFront() {
        GridCell gridCell = GetFront();
        if (gridCell == null) {
            return null;
        }
        _path.RemoveFirst();
        return gridCell;
    }

    public void TurnBlue() {
        foreach (GridCell gridCell in _path) {
            gridCell.TurnBlue();
        }
    }
    public void RevertColor() {
        foreach (GridCell gridCell in _path) {
            gridCell.RevertColor();
        }
    }
    public void ShowPath()
    {
        foreach (GridCell gridCell in _path) {
            gridCell.ShowCell();
        }
    }
    public void HidePath()
    {
        foreach (GridCell gridCell in _path) {
            gridCell.HideCell();
        }
    }
}
