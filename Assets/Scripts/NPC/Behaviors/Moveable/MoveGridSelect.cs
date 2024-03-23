using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveGridSelect : BehaviorState {
    private LayerMask _prevLayerMask;
    private Moveable _moveable;

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
        _prevLayerMask = CameraController.Instance.HitMask;
        CameraController.Instance.HitMask = ConstantValues.GridMask;
        GridManager.Instance.Activate();
        GridManager.Instance.HoverAction += _moveable.Monster.ShowPath;
        GridManager.Instance.ClickAction += _moveable.Monster.ChoosePath;
    }
    public void Deactivate() {
        CameraController.Instance.HitMask = _prevLayerMask;
        GridManager.Instance.Deactivate();
        GridManager.Instance.HoverAction -= _moveable.Monster.ShowPath;
        GridManager.Instance.ClickAction -= _moveable.Monster.ChoosePath;
    }
}