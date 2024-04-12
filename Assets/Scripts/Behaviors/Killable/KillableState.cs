using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class KillableState : BehaviorState {
    private LayerMask _prevLayerMask;
    private Moveable _moveable;
    private List<GridCell> _gridCells;
    private MoveableRoutine _moveableRoutine;

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
        _moveable = behaviorRoutine.Behavior as Moveable;
        _moveableRoutine = behaviorRoutine as MoveableRoutine;
        _prevLayerMask = CameraController.Instance.HitMask;
        CameraController.Instance.HitMask = ConstantValues.GridMask;
        GridManager.Instance.Activate();
        //_gridCells = Pathfinder.ActivateCells(_moveable.Monster.OccupiedCell, _moveableRoutine.TempMovement);
        // Debug.Log(_moveable.Monster);
        GridManager.Instance.HoverAction += ShowPath;
        GridManager.Instance.ClickAction += ChoosePath;
    }
    public void Deactivate() {
        CameraController.Instance.HitMask = _prevLayerMask;
        GridManager.Instance.Deactivate();
        foreach(GridCell cell in _gridCells) {
            cell.HideCell();
        }
        GridManager.Instance.HoverAction -= ShowPath;
        GridManager.Instance.ClickAction -= ChoosePath;
    }

    private void ShowPath(GridCell cell) {
        //_moveable.Monster.ShowPath(cell);
    }

    private void ChoosePath(GridCell cell) {
        //_moveableRoutine.TempMovement -= _moveable.Monster.LengthOfPath;
        //_moveable.Monster.ChoosePath(cell);
        Deactivate();
        _moveable.ExecuteBehavior();
    }
}