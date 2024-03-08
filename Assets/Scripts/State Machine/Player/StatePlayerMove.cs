using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatePlayerMove : State
{
    public bool canSwitch = true;
    private GridCell _gridCell;
    private GridPath _gridPath;
    private LayerMask _prevLayerMask;

    public override void EnterState() {
        PlayerTurnMachine ptm = stateMachine as PlayerTurnMachine;
        _prevLayerMask = CameraController.Instance.HitMask;
        CameraController.Instance.HitMask = ConstantValues.GridMask;
        GridManager.Instance.HoverAction += ptm.monster.ShowPath;
        GridManager.Instance.ClickAction += ptm.monster.ChoosePath;
    }
    public override void UpdateState() {

    }
    public override void ExitState() {
        PlayerTurnMachine ptm = stateMachine as PlayerTurnMachine;
        CameraController.Instance.HitMask = _prevLayerMask;
        // canSwitch = false;
        GridManager.Instance.HoverAction -= ptm.monster.ShowPath;
        GridManager.Instance.ClickAction -= ptm.monster.ChoosePath;
    }
}
