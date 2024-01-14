using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatePlayerIdle : State
{
    public override void EnterState(StateMachine sm) {
        PlayerTurnMachine ptm = sm as PlayerTurnMachine;
        //flesh this out to bring up UI and stuff, but only when all that's implemented lmao
        CameraController.Instance.ClickAction += ptm.monster.SelectMonster;
    }
    public override void UpdateState(StateMachine sm) {

    }
    public override void ExitState(StateMachine sm) {
        PlayerTurnMachine ptm = sm as PlayerTurnMachine;
        CameraController.Instance.ClickAction -= ptm.monster.SelectMonster;
    }
    private void SwitchToMove() {
        stateMachine.ChangeState((stateMachine as PlayerTurnMachine).moveState);
    }
}
