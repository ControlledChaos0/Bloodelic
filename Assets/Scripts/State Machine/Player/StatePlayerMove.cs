using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatePlayerMove : State
{
    public bool canSwitch = true;
    private GridCell _gridCell;
    private GridPath _gridPath;
    private LayerMask _prevLayerMask;

    public override void EnterState(StateMachine sm) {
        PlayerTurnMachine ptm = sm as PlayerTurnMachine;
        _prevLayerMask = CameraController.Instance.HitMask;
        CameraController.Instance.HitMask = ConstantValues.GridMask;
        GridManager.Instance.HoverAction += ptm.monster.ShowPath;
        GridManager.Instance.ClickAction += ptm.monster.ChoosePath;
    }
    public override void UpdateState(StateMachine sm) {

    }
    public override void ExitState(StateMachine sm) {
        PlayerTurnMachine ptm = sm as PlayerTurnMachine;
        CameraController.Instance.HitMask = _prevLayerMask;
        // canSwitch = false;
        GridManager.Instance.HoverAction -= ptm.monster.ShowPath;
        GridManager.Instance.ClickAction -= ptm.monster.ChoosePath;
    }
}
