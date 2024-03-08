using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatePlayerIdle : State
{
    public override void EnterState() {
        PlayerTurnMachine ptm = stateMachine as PlayerTurnMachine;
        //flesh this out to bring up UI and stuff, but only when all that's implemented lmao
        //CameraController.Instance.ClickAction += ptm.monster.Select;
    }
    public override void UpdateState() {

    }
    public override void ExitState() {
        PlayerTurnMachine ptm = stateMachine as PlayerTurnMachine;
        //CameraController.Instance.ClickAction -= ptm.monster.Select;
    }
    private void SwitchToMove() {
        stateMachine.ChangeState((stateMachine as PlayerTurnMachine).moveState);
    }
}
