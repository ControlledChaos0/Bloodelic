using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ThrowSelect : BehaviorState {
    private LayerMask _prevLayerMask;
    private Throwable _throwable;
    private List<GridCell> _gridCells;
    private ThrowableRoutine _throwableRoutine;

    public override void EnterState()
    {
        Activate();
    }
    public override void UpdateState()
    {
        
    }
    public override void ExitState()
    {
        Deactivate();
    }

    public void Activate() {
        _throwable = behaviorRoutine.Behavior as Throwable;
        _throwableRoutine = behaviorRoutine as ThrowableRoutine;
        _prevLayerMask = CameraController.Instance.HitMask;
        CameraController.Instance.HitMask = ConstantValues.GridMask;
        GridManager.Instance.Activate();
        SmallObject smallObj = _throwable.GetComponent<SmallObject>();
        _gridCells = Pathfinder.ActivateCells(smallObj.OccupiedCell, _throwableRoutine.SearchRange);
        // Debug.Log(_moveable.Monster);
        GridManager.Instance.HoverAction += ShowPath;
        GridManager.Instance.ClickAction += ChooseTarget;
    }
    public void Deactivate() {
        CameraController.Instance.HitMask = _prevLayerMask;
        GridManager.Instance.Deactivate();
        foreach(GridCell cell in _gridCells) {
            cell.HideCell();
        }
        GridManager.Instance.HoverAction -= ShowPath;
        GridManager.Instance.ClickAction -= ChooseTarget;
        stateMachine.CurrUI.gameObject.SetActive(false);
    }

    public virtual GridPath FindPathWithThrowRange(GridCell target, GridCell start, int range)
    {
        Pathfinder.moveLimit = range;
        Pathfinder.entity = null;
        return Pathfinder.FindPath(start, target, true);
    }
    
    private GridPath currPosPath;

    
    private void ShowPath(GridCell cell)
    {
        SmallObject smallObj = _throwable.GetComponent<SmallObject>();
        if (smallObj == null)
        {
            return;
        }
        
        if (cell.Equals(smallObj.OccupiedCell)) {
            return;
        }
        
        // Find next path and store to temporary
        GridPath nextPath = FindPathWithThrowRange(cell, smallObj.OccupiedCell, _throwable.Range);
        // Skip here so we maintain visuals of the previous path
        if (nextPath == null)
        {
            return;
        }    
        
        //currPosPath?.HidePath();
        currPosPath?.RevertColor();
        currPosPath = nextPath;
        currPosPath.TurnBlue();
    }

    private void ChooseTarget(GridCell cell) {
        // Set up throw curve cells
        _throwable.startCell = _throwable.Object.OccupiedCell;
        _throwable.targetCell = cell;
        
        Deactivate();
        _throwable.ExecuteBehavior();
    }
}