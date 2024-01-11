using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State : IGameState
{
    private StateMachine _stateMachine;

    public void StartState(StateMachine sm) {
        _stateMachine = sm;
        EnterState();
    }
    public virtual void EnterState() {

    }
    public virtual void UpdateState() {

    }
    public virtual void ExitState() {

    }
}
